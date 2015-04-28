using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    public sealed class TeamCityIntegrationTests
    {
        private static readonly IEnumerable<string> CaseIds = new[]
        {
            "CaseOneSuccesfulTest",
            "TwoSuccesfulTests"
        };
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

        [Test, TestCaseSource("TestResults"), Category("Integration")]
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
                    CreateCmdLineToolDto("20", false),
                    CreateCmdLineToolDto("20", true),
                    CreateCmdLineToolDto("40", false),
                    CreateCmdLineToolDto("40", true),
                    CreateCmdLineToolDto("45", false),
                    CreateCmdLineToolDto("45", true),
                });
        }

        private static CmdLineToolDto CreateCmdLineToolDto(string framework, bool useEnvVar)
        {
            var args = new List<string>
            {
                string.Format(@"net{0}\NUnit.Integration.Mocks.dll", framework)
            };

            var environmentVariables = new Dictionary<string, string>();
            if (useEnvVar)
            {
                environmentVariables.Add("TEAMCITY_PROJECT_NAME", "Test");
            }
            else
            {
                args.Add("--teamcity");
            }

            return new CmdLineToolDto(
                CertType.TestFramework,
                string.Format("NUnit v{0} using {1}", framework, useEnvVar ? "env var" : "--teamcity"),
                string.Format(@"net{0}\nunit-console.exe", framework),
                args.ToArray(),
                environmentVariables,
                CaseIds.Select(CreateCaseDto));
        }

        private static CaseDto CreateCaseDto(string caseId)
        {
            return new CaseDto(caseId, new[] { string.Format("--include={0}", caseId) });
        }
    }
}
