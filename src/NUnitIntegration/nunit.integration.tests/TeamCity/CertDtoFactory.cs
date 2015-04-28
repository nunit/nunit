using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    internal sealed class CertDtoFactory
    {
        private const int MaxThreadsCount = 100;

        public CertDto CreateCert()
        {
            return new CertDto(CreateCmdLineToolDto());
        }

        private static IEnumerable<CmdLineToolDto> CreateCmdLineToolDto()
        {
            var frameworks = new[]
            {
                "20",
                "40",
                "45",
            };

            return (
                from framework in frameworks
                select CreateCmdLineToolDto(framework)).SelectMany(i => i);
        }

        private static IEnumerable<CmdLineToolDto> CreateCmdLineToolDto(string framework)
        {
            var integrationTypes = new[]
            {
                IntegrationType.CommandArg,
                IntegrationType.EnvironmentVar,
            };

            var generalCases = (
                from integrationType in integrationTypes
                select CreateCmdLineToolDto(framework, integrationType, 1, CaseLists.GeneralCases)).SelectMany(i => i);

            var multithreadingCases = (
                from integrationType in integrationTypes
                select CreateCmdLineToolDto(framework, integrationType, 50, CaseLists.GeneralCases)).SelectMany(i => i);

            return generalCases.Concat(multithreadingCases);
        }

        private static IEnumerable<CmdLineToolDto> CreateCmdLineToolDto(string framework, IntegrationType integrationType, int threadsCount, IEnumerable<string> cases)
        {
            var args = new List<string>
            {
                string.Format(@"net{0}\NUnit.Integration.Mocks.dll", framework)
            };

            var environmentVariables = new Dictionary<string, string>();
            string integrationTypeInfo = "unknown integration type";
            switch (integrationType)
            {
                case IntegrationType.CommandArg:
                    args.Add("--teamcity");
                    integrationTypeInfo = "--teamcity";
                    break;

                case IntegrationType.EnvironmentVar:
                    environmentVariables.Add("TEAMCITY_PROJECT_NAME", "Test");
                    integrationTypeInfo = "env var";
                    break;
            }

            if (threadsCount > 1)
            {
                args.Add(string.Format("--workers={0}", threadsCount));
            }

            yield return new CmdLineToolDto(
                CertType.TestFramework,
                string.Format("NUnit for .Net{0} using {1} and {2} treads", framework, integrationTypeInfo, threadsCount),
                string.Format(@"net{0}\nunit-console.exe", framework),
                args.ToArray(),
                environmentVariables,
                cases.Select(CreateCaseDto));
        }

        private static CaseDto CreateCaseDto(string caseId)
        {
            return new CaseDto(caseId, new[] { string.Format("--include={0}", caseId) });
        }
    }
}
