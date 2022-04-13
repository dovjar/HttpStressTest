using CommandLine;
using Newtonsoft.Json;
using Serilog;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.ConsoleUi;

namespace HttpStressTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File($"httpStressTest-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.txt")
                .CreateLogger();

            CommandLine.Parser.Default.ParseArguments<CliOptions>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;
            Console.WriteLine("errors {0}", errs.Count());
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;
            Console.WriteLine("Exit code {0}", result);
        }
        static void RunOptions(CliOptions opts)
        {
            Console.WriteLine(JsonConvert.SerializeObject(opts, Formatting.Indented));
            IEnumerable<Dictionary<string, string>> testData = Array.Empty<Dictionary<string, string>>();
            if (!string.IsNullOrEmpty(opts.DataFile))
            {
                var text = File.ReadAllText(opts.DataFile);
                var headerLine = Csv.CsvReader.ReadFromText(text, new Csv.CsvOptions
                {
                    HeaderMode = Csv.HeaderMode.HeaderAbsent
                })
                    .First();
                var headers = headerLine.Values;
                testData = Csv.CsvReader.ReadFromText(text, new Csv.CsvOptions
                {
                    HeaderMode = Csv.HeaderMode.HeaderPresent,
                    TrimData = true,
                    ReturnEmptyForMissingColumn = true,
                })
                    .Select(line => new Dictionary<string, string>(headers.Select(header => new KeyValuePair<string, string>(header, line[header]))));
            }
            var testCase = new TestCase(JsonConvert.DeserializeObject<TestCaseDefinition>(File.ReadAllText(opts.ScenarioFile)));
            Log.Logger.Information("{@testCase}",testCase);
            //run warmup
            new HttpTestScenario(testCase.WarmUpSteps, testCase.GlobalParameters).ExecuteScenario(new StubIteration());


            // [2] Results aggregation (Or raw measurement collection, see RawDataMeasurementsDemo.cs)
            // Define how data gets aggregated.
            // Dimensions are like GROUP BY keys in SQL
            // Metrics are aggregation functions like COUNT, SUM, etc..
            // Extensive HistogramAggregator demo now WiP
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new ErrorCountMetric())
                .Add(new TransactionsPerSecMetric())
                .Add(new PercentileMetric(0.95, 0.99));

            // Secondary aggregation just to monitor key metrics.
            KpiPrinterAggregator kpiPrinter = new KpiPrinterAggregator(
                TimeSpan.FromSeconds(5),
                new CountMetric(Checkpoint.NotMeassuredCheckpoints),
                new ErrorCountMetric(false),
                new TransactionsPerSecMetric()
            );

            // [3] Execution settings
            // Using StrategyBuilder put defined aggregation, scenario, and execution settings into one object
            StrategyBuilder strategy = new StrategyBuilder()
                .SetAggregator(aggregator, kpiPrinter) // Optional
                .SetScenario(new HttpTestScenarioFactory(testCase,testData.ToArray())) // Required
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(opts.Seconds))) // Optional, but if not provided, execution will never stop - unless running test with RunAsync() and stopping later with CancelAsync(true)
                                                                             //    .SetSpeed(new FixedSpeed(100000)) // Optional (Skip for maximum throughput)
                .SetThreading(new FixedThreadCount(opts.ThreadCount)); // Required



            // [4] Execution
            // All that's left is Build(), run and wait for completion and print out measured results.
            LoadRunnerEngine engine = strategy.Build();
            engine.Run();

            IEnumerable<object> result = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}