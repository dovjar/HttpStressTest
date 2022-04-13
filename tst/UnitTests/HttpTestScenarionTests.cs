using HttpStressTest;
using NUnit.Framework;
using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UnitTests
{
 
    public class WireMockServerLogContext : IDisposable
    {
        private readonly WireMockServer server;
        private readonly int skip;
        public WireMockServerLogContext(WireMockServer server)
        {
            this.server = server;
            skip=this.server.LogEntries.Count();
        }
        public void Dispose()
        {
            foreach(var entry in server.LogEntries.Skip(skip))
                TestContext.WriteLine($"{entry.RequestMessage.Url} - {entry.ResponseMessage.StatusCode} - {entry.ResponseMessage.BodyOriginal}"); 
        }
    }
    
    public class HttpTestScenarionTests
    {
        int port;
        WireMockServer server;
        
        [OneTimeSetUp]
        public void SetupoFixture()
        {
            server = WireMockServer.Start( );
           

            server
              .Given(
                Request.Create().WithPath("/notfound").UsingGet()
              )
              .RespondWith(
                Response.Create()
                  .WithStatusCode(404)
                  .WithHeader("Content-Type", "text/plain")

              );


            server
              .Given(
                Request.Create().WithPath("/path").UsingGet()
              )
              .RespondWith(
                Response.Create()
                  .WithStatusCode(200)
                  .WithHeader("Content-Type", "text/json")
                  .WithBodyAsJson(new {path="test"})

              );

             server
              .Given(
                Request.Create().WithPath("/test").UsingGet()
              )
              .RespondWith(
                Response.Create()
                  .WithStatusCode(200)
                  .WithHeader("Content-Type", "text/json")
                  .WithBodyAsJson(new {status=0})

              );
           
            port = server.Port;
        }

       

        [OneTimeTearDown]
        public void ShutdownServer()
        {
            server.Stop();
        }
        [Test]
        public void CallToServiceWithoutAllowedStatusesSucceeds()
        {
            //arrange
            var testCase = new TestCase(
                                    new TestCaseDefinition
                                    {
                                        Steps = new[]
                                        {
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/notfound"
                                            }
                                        }
                                    });
            var scenario  = new HttpTestScenario(testCase.Steps, new System.Collections.Generic.Dictionary<string, string>());

            //act
            using(var log = new WireMockServerLogContext(server))
                scenario.ExecuteScenario(new StubIteration());
        }
        [Test]
        public void CallToServiceMustFailIfHttpStatusdiffers()
        {
            //arrange
            var testCase = new TestCase(
                                    new TestCaseDefinition
                                    {
                                        Steps = new[]
                                        {
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/notfound",
                                                AllowedStatuses=new[]{200}
                                            }
                                        }
                                    });
            var scenario  = new HttpTestScenario(testCase.Steps, new System.Collections.Generic.Dictionary<string, string>());

            //act
            Assert.Throws<Exception>(() =>
            {
                using(var log = new WireMockServerLogContext(server))
                    scenario.ExecuteScenario(new StubIteration());
            });
        }
        [Test]
        public void CallToNextStepMustFail()
        {
            //arrange
            var testCase = new TestCase(
                                    new TestCaseDefinition
                                    {
                                        Steps = new[]
                                        {
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/path",
                                                AllowedStatuses=new[]{200}
                                            },
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/{{path}}",
                                                AllowedStatuses=new[]{200}
                                            },
                                        }
                                    });
            var scenario  = new HttpTestScenario(testCase.Steps, new System.Collections.Generic.Dictionary<string, string>());

            //act
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
            {
                using(var log = new WireMockServerLogContext(server))
                    scenario.ExecuteScenario(new StubIteration());
            });
        }

        [Test]
        public void CallToNextStepCarriesParameterFromFirstResponse()
        {
            //arrange
            var testCase = new TestCase(
                                    new TestCaseDefinition
                                    {
                                        Steps = new[]
                                        {
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/path",
                                                AllowedStatuses=new[]{200},
                                                ResponseRegex = "(\"path\"\\s*:\\s*\"(?<path>[a-zA-Z0-9._]+)\")"
                                            },
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/{{path}}",
                                                AllowedStatuses=new[]{200}
                                            },
                                        }
                                    });
            var scenario  = new HttpTestScenario(testCase.Steps, new System.Collections.Generic.Dictionary<string, string>());

            //act
            using(var log = new WireMockServerLogContext(server))
                scenario.ExecuteScenario(new StubIteration());
        }

        [Test]
        public void WarmUpScenario()
        {
            //arrange
            var testCase = new TestCase(
                                    new TestCaseDefinition
                                    {
                                        WarmUpSteps = new[]
                                        {
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/path",
                                                AllowedStatuses=new[]{200},
                                                ResponseRegex = "(\"path\"\\s*:\\s*\"(?<path>[a-zA-Z0-9._]+)\")"
                                            }
                                        },
                                        Steps = new[]
                                        {
                                            
                                            new TestCaseStepDefinition
                                            {
                                                Get=$"http://localhost:{port}/{{path}}",
                                                AllowedStatuses=new[]{200}
                                            },
                                        }
                                    });
            new HttpTestScenario(testCase.WarmUpSteps, testCase.GlobalParameters).ExecuteScenario(new StubIteration());
            var scenario  = new HttpTestScenario(testCase.Steps,testCase.GlobalParameters);

            //act
            using(var log = new WireMockServerLogContext(server))
                scenario.ExecuteScenario(new StubIteration());
        }
    }
}