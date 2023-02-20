using CommandLine;

namespace HttpStressTest
{
    public class CliOptions
    {
        [Option('d', "dataFile", Required = false, HelpText = "csv file for test data.")]
        public string? DataFile { get; set; }
        
        [Option('s', "seconds", Required = false, HelpText = "Seconds to run.")]
        public int Seconds { get; set; } = 30;
        
        [Option('t', "thread", Required = false, HelpText = "Thread count - defaults to 20.")]
        public int ThreadCount { get; set; } = 20;

        [Option('c', "scenario", Required = true, HelpText = "json file containing test case.")]
        public string ScenarioFile { get; set; } = "";

        [Option('h', "headersSeparator", Required = false, HelpText = "string to separate headers in test file, default ':'.")]
        public string HeadersSeparator { get; set; } = ":";

    }
}