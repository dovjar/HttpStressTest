using HttpStressTest;
using NUnit.Framework;

namespace UnitTests
{
    public class TestCaseStepParseResultTests
    {
        [Test]
        public void ParseStep()
        {
            //arrange
           var step = new TestCaseStep(new TestCaseStepDefinition
            {
                Id="vienas",
                ResponseRegex = "(\"jwt\"\\s*:\\s*\"(?<jwt>[a-zA-Z0-9._]+)\")"
            }); 

            var text=@"
{
""jwt"" : ""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c""
}
";
            //act
            var result = step.ParseFromResponse(text);
            //assert
            Assert.AreEqual("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c", result["jwt"]);
        }
    }
}