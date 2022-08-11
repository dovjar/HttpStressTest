namespace HttpStressTest
{
    public record TestCaseOptions(string HeadersSeparator);
    public class TestCase
    {

        public TestCase(TestCaseDefinition caseDefinition, TestCaseOptions? options=null)
        {
            Steps = caseDefinition.Steps.Select(t=>new TestCaseStep(t, options??new TestCaseOptions(":"))).ToArray();
            WarmUpSteps = caseDefinition.WarmUpSteps.Select(t=>new TestCaseStep(t,options??new TestCaseOptions(":"))).ToArray();
            GlobalParameters = caseDefinition.GlobalParameters;

        }
        public TestCaseStep[] Steps { get; }
        public TestCaseStep[] WarmUpSteps { get; }
        public Dictionary<string, string> GlobalParameters { get; set; }
    }
}