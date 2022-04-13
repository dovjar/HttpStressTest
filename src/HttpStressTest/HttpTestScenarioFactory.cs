using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace HttpStressTest
{
    public class HttpTestScenarioFactory:IScenarioFactory
    {
        private readonly TestCase testCase;
        private readonly Dictionary<string, string>[] testData;

        public HttpTestScenarioFactory(TestCase testCase, Dictionary<string,string>[] testData)
        {
            this.testCase = testCase;
            this.testData = testData;
        }
        public IScenario Create(int threadId)
        {
            var data=testData.Length>0? testData[threadId % testData.Length]: new Dictionary<string,string>();
            data.Upsert(testCase.GlobalParameters);
            return new HttpTestScenario(testCase.Steps,data.ToDictionary(t=>t.Key,t=>t.Value));
        }
    }
}