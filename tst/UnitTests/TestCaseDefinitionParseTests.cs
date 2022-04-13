using HttpStressTest;
using Newtonsoft.Json;
using NUnit.Framework;

namespace UnitTests
{
    public class TestCaseDefinitionParseTests
    {
        [Test]
        public void ParseStep()
        {
            //arrange
            var json = @"{
  ""steps"": [
    {
      ""id"": ""1"",
      ""get"": ""http://www.delfi.lt?query={{param1}}"",
      ""headers"": [],
      ""body"": """",
      ""responseBodyRegex"": """"
    }
  ]
}";
            //act
            var result = JsonConvert.DeserializeObject<TestCaseDefinition>(json);
            //assert
            Assert.AreEqual("1", result.Steps[0].Id);
        }


        [Test]
        public void ParseGlobalParameters()
        {
            //arrange
            var json = @"{
  ""globalParameters"": {
    ""jwt"": ""test""
  }
}";
            //act
            var result = JsonConvert.DeserializeObject<TestCaseDefinition>(json);
            //assert
            Assert.AreEqual("test", result.GlobalParameters["jwt"]);
        }
    }
}