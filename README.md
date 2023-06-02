# HttpStressTest

Http stress test utility.
Load test engine - Viki.LoadRunner.

## Idea
Tool which can run test case (several http calls) to stress test http service.

## Command line parameters
```cs
[Option('d', "dataFile", Required = false, HelpText = "csv file for feed test data.")]
public string? DataFile { get; set; } 

[Option('s', "seconds", Required = false, HelpText = "Seconds to run.")]
public int Seconds { get; set; } = 30;

[Option('t', "thread", Required = false, HelpText = "Thread count - defaults to 20.")]
public int ThreadCount { get; set; } = 20;

[Option('c', "scenario", Required = true, HelpText = "json file containing test case.")]
public string ScenarioFile { get; set; } = "";
```

## JSON File structure
```cs
//global parameters per test case - i.e. jwt token
public Dictionary<string, string> GlobalParameters { get; set; } = new Dictionary<string, string>();
//warm up steps run before starting test, i.e. get jwt token
public TestCaseStepDefinition[] WarmUpSteps { get; set; } = Array.Empty<TestCaseStepDefinition>(); 
//test case steps - run repeatedly for each thread
public TestCaseStepDefinition[] Steps { get; set; } = Array.Empty<TestCaseStepDefinition>();
```
  
## JSON Step definition
```cs
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
```
# Example
Get JWT token once from authentication service `/auth` endpoint and invoke account service `/personalData` endpoint with given users.

`account_id.csv`
```csv
account_id
123
456
789
```

`personalDataStressTest.json`
```json
{
  "globalParameters": {
   
  },
  "warmUpSteps":[
	{
      "id": "jwt",
      "post": "http://authenticationService/auth",
      "headers": ["accept: application/json","Content-Type: application/json"],
      "body": "{'subject': 'example','password': 'example'}",
      "responseRegex": "(\"jwt\"\\s*:\\s*\"(?<jwt>[a-zA-Z0-9._-]+)\")"
    }
  ],
  "steps": [
    {
      "id": "query",
      "get": "http://accountService/{account_id}/personalData",
      "headers": ["Authorization: bearer {jwt}"],
      "body": ""
    }
  ]
}
```
### CLI
```shell
HttpStressTest.exe -d account_id.csv -c personalDataStressTest.json
```

`Example output:`
```json
[
  {
    "Time (s)": "5",
    "Count: Setup": 1960,
    "Count: Iteration": 1960,
    "Count: TearDown": 1960,
    "Errors: Totals": 0,
    "TPS": 196.20842632503232,
    "95%: Iteration": 42,
    "99%: Iteration": 104
  },
  {
    "Time (s)": "10",           // This entry shows the results of period 5sec- 10sec
    "Count: Setup": 2300,       // During this 5 sec 2300 requests were started
    "Count: Iteration": 2300,
    "Count: TearDown": 2300,    // During this 5 sec 2300 requests were completed
    "Errors: Totals": 0,        // No errors running request in past 5 sec (this does not show response code 500, 404, etc.)
    "TPS": 457.51142152399086,  // Transactions-per.second in last 5 sec (=~ CountSetup / 5)
    "95%: Iteration": 39,       // 95% of transactions during past 5 sec completed in 39ms or less
    "99%: Iteration": 51        // 99% of transactions during past 5 sec completed in 51ms or less
  },
  {
    "Time (s)": "15",
    "Count: Setup": 2445,
    "Count: Iteration": 2445,
    "Count: TearDown": 2445,
    "Errors: Totals": 0,
    "TPS": 485.4809235582586,
    "95%: Iteration": 35,
    "99%: Iteration": 45
  },
  {
    "Time (s)": "20",
    "Count: Setup": 2909,
    "Count: Iteration": 2909,
    "Count: TearDown": 2909,
    "Errors: Totals": 0,
    "TPS": 579.4416839352173,
    "95%: Iteration": 29,
    "99%: Iteration": 35
  },
  {
    "Time (s)": "25",
    "Count: Setup": 2680,
    "Count: Iteration": 2680,
    "Count: TearDown": 2680,
    "Errors: Totals": 0,
    "TPS": 534.4524075415782,
    "95%: Iteration": 32,
    "99%: Iteration": 42
  },
  {
    "Time (s)": "30",
    "Count: Setup": 49,
    "Count: Iteration": 49,
    "Count: TearDown": 49,
    "Errors: Totals": 0,
    "TPS": 390.54840967093503,
    "95%: Iteration": 43,
    "99%: Iteration": 50
  }
]
```

