using System.Linq;
using System.Text;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    public sealed class TeamCityIntegrationTests
    {
        private static readonly CertDto CertData = CreateCertData();

        public static object[] TestResults
        {
            get
            {
                using (ServiceLocator.Root.RegisterExtension(new ServiceLocatorConfigurationExtension()))
                {
                    return ServiceLocator.Root.GetService<ICertEngine>().Run(CertData).Select(i => new[] { string.Format("Case {0}", i), (object)i }).ToArray();
                }
            }
        }

        [Test, TestCaseSource("TestResults")]
        public void Case(string caseName, object testResultObj)
        {
            var result = (TestResultDto)testResultObj;
            if (result.State == TestState.Failed)
            {
                Assert.Fail(CreateDetails(result));
            }

            if (result.State == TestState.Ignored)
            {
                Assert.Ignore(CreateDetails(result));
            }

            if (result.State == TestState.UnknownCase || result.State == TestState.Exception || result.State == TestState.NotImplemented)
            {
                Assert.Inconclusive(CreateDetails(result));
            }

            if (result.State == TestState.Passed)
            {
                Assert.Pass(CreateDetails(result));
            }
        }

        private string CreateDetails(params TestResultDto[] results)
        {            
            var sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendFormat("Case {0}", result);
                sb.AppendLine();
                sb.AppendLine(result.Details);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static CertDto CreateCertData()
        {
            return new CertDto(
                new[] 
                {
                    new CmdLineToolDto(
                        CertType.TestFramework, 
                        "NUnit v2.0", 
                        @"net20\nunit-console.exe",
                        new[] { @"net20\NUnit.Integration.Mocks.dll", "--teamcity" },
                        new[] {
                            new CaseDto("CaseOneSuccesfulTest", new[] { "--include=CaseOneSuccesfulTest" }), 
                            new CaseDto("TwoSuccesfulTests", new[] { "--include=TwoSuccesfulTests" }),
                        })
                });
        }
    }
}
