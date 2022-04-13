namespace HttpStressTest
{
    public class TestCase
    {
        public TestCase(TestCaseDefinition caseDefinition)
        {
            Steps = caseDefinition.Steps.Select(t=>new TestCaseStep(t)).ToArray();
            WarmUpSteps = caseDefinition.WarmUpSteps.Select(t=>new TestCaseStep(t)).ToArray();
            GlobalParameters = caseDefinition.GlobalParameters;

        }
        public TestCaseStep[] Steps { get; }
        public TestCaseStep[] WarmUpSteps { get; }
        public Dictionary<string, string> GlobalParameters { get; set; }
    }
}