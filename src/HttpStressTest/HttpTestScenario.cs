using Serilog;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace HttpStressTest
{
    public class HttpTestScenario : IScenario
    {
        private readonly TestCaseStep[] steps;
        private readonly Dictionary<string, string> testData;
        private readonly HttpClient httpClient = new HttpClient();


        public HttpTestScenario(TestCaseStep[] steps, Dictionary<string,string> testData)
        {
            this.steps = steps;
            this.testData = testData;
        }
        public void ScenarioSetup(IIteration context)
        {
            // One time setup for each instance/thread
        }

        public void IterationSetup(IIteration context)
        {
            // Setup before each iteration
        }

        public void ExecuteScenario(IIteration context)
        {
            foreach(var step in steps)
            {
               HttpRequestMessage request=  step.ConstructHttpMessage(testData);
               var response= httpClient.Send(request);
               Log.Logger.Information("{threadId}: {requestUri} {statusCode}",context.ThreadId, request.RequestUri, response.StatusCode);
               
                if (step.AllowedStatuses.Length > 0 && !step.AllowedStatuses.Contains(((int)response.StatusCode)))
                   throw new Exception($"unexpected result from server - status code: {response.StatusCode}");

               if(!string.IsNullOrEmpty(step.ResponseRegex))
                {
                    var bodyTask = response.Content.ReadAsStringAsync();
                    bodyTask.Wait();
                    testData.Upsert( step.ParseFromResponse(bodyTask.Result));
                }
            }
        }

        public void IterationTearDown(IIteration context)
        {
            // Cleanup after each iteration (even if IterationSetup() or ExecuteScenario() fails)
        }

        public void ScenarioTearDown(IIteration context)
        {
            // One time cleanup for each instance/thread
        }
    }
}