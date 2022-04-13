namespace HttpStressTest
{
    public class TestCaseDefinition
    {
        public Dictionary<string, string> GlobalParameters { get; set; } = new Dictionary<string, string>();
        public TestCaseStepDefinition[] WarmUpSteps { get; set; } = Array.Empty<TestCaseStepDefinition>(); 
        public TestCaseStepDefinition[] Steps { get; set; } = Array.Empty<TestCaseStepDefinition>();
    }
}