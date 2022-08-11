using HttpStressTest;
using NUnit.Framework;

namespace UnitTests
{
    public class TestCaseStepTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WithoutParametersShouldConstructEmtyArray()
        {
            //arrange

            //act
            var step = new TestCaseStep(new TestCaseStepDefinition
            {
                Id="vienas",
                Get="https://www.delfi.lt"
            }, new TestCaseOptions(":"));
            //assert
            Assert.IsEmpty(step.Parameters);
        }
        [Test]
        public void ShouldExtractParameterName()
        {
            //arrange

            //act
            var step = new TestCaseStep(new TestCaseStepDefinition
            {
                Id="vienas",
                Get="https://www.delfi.lt?query={{param1}}"
            }, new TestCaseOptions(":"));
            //assert
            CollectionAssert.AreEqual(new[]{"param1"}, step.Parameters);
        }
        [Test]
        public void DistinctParameterNames()
        {
            //arrange

            //act
            var step = new TestCaseStep(new TestCaseStepDefinition
            {
                Id="vienas",
                Post="https://www.delfi.lt?query={{param1}}",
                Body="{{param1}}={{param2}}",
            }, new TestCaseOptions(":"));
            //assert
            CollectionAssert.AreEqual(new[]{"param1","param2"}, step.Parameters);
        }
    }
}