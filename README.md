# HttpStressTest

Http stress test utility.
Load test engine - Viki.LoadRunner.

## idea
to have tool which can run test case ( several http calls) to stress test http service.

## command line parameters

[Option('d', "dataFile", Required = false, HelpText = "csv file for feed test data.")]
public string? DataFile { get; set; } 

[Option('s', "seconds", Required = false, HelpText = "Seconds to run.")]
public int Seconds { get; set; } = 30;

[Option('t', "thread", Required = false, HelpText = "Thread count - defaults to 20.")]
public int ThreadCount { get; set; } = 20;

[Option('c', "scenario", Required = true, HelpText = "json file containing test case.")]
public string ScenarioFile { get; set; } = "";

## json file structure
//global parameters per test case - i.e. jwt token
public Dictionary<string, string> GlobalParameters { get; set; } = new Dictionary<string, string>();
//warm up steps run before starting test, i.e. get jwt token
public TestCaseStepDefinition[] WarmUpSteps { get; set; } = Array.Empty<TestCaseStepDefinition>(); 
//test case steps - run repeatedly for each thread
public TestCaseStepDefinition[] Steps { get; set; } = Array.Empty<TestCaseStepDefinition>();
  
### json step definition
//step id
public string Id { get; set; }="";
//GET uri
public string? Get { get; set; }
//POST uri
public string? Post { get; set; }
//headers to send
public string[] Headers { get; set; }= Array.Empty<string>();
//body to send
public string? Body { get; set; }
//regeq to parse resonse body for parametters
public string? ResponseRegex {get;set;}
//to test response HTTP satatuses
public int[] AllowedStatuses {get;set;}= Array.Empty<int>();
//if true - skips test case step
public bool Skip {get;set;} = false;
  
  
# flow
