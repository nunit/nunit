using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    internal sealed class CertDtoFactory
    {
        private const string TeamCityArg = "--teamcity";
        private const string IncludeArg = "--include";
        private const string WorkersArg = "--workers";
        private const string TeamCityEnvVar = "TEAMCITY_PROJECT_NAME";
        private const string MockTestsAssemblyName = "NUnit.Integration.Mocks.dll";
        private const string NUnitConsoleName = "nunit-console.exe";

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

            var generalCases = CaseLists.GeneralCases.Select(CreateGeneralCreateCaseDto);
            var multithreadingCases = CaseLists.MultithreadingCases.Select(CreateMultithreadingCreateCaseDto);
            var allCases = generalCases.Concat(multithreadingCases);

            return (
                from integrationType in integrationTypes
                select CreateCmdLineToolDto(framework, integrationType, allCases)).SelectMany(i => i);
        }

        private static IEnumerable<CmdLineToolDto> CreateCmdLineToolDto(
            string framework, 
            IntegrationType integrationType, 
            IEnumerable<CaseDto> cases)
        {
            var args = new List<string>
            {
                string.Format(@"net{0}\{1}", framework, MockTestsAssemblyName)
            };

            var environmentVariables = new Dictionary<string, string>();
            string integrationTypeInfo = "unknown integration type";
            switch (integrationType)
            {
                case IntegrationType.CommandArg:
                    args.Add(TeamCityArg);
                    integrationTypeInfo = TeamCityArg;
                    break;

                case IntegrationType.EnvironmentVar:
                    environmentVariables.Add(TeamCityEnvVar, "Test");
                    integrationTypeInfo = "env var";
                    break;
            }

            yield return new CmdLineToolDto(
                CertType.TestFramework,
                string.Format("NUnit for .Net{0} using {1}", framework, integrationTypeInfo),
                string.Format(@"net{0}\{1}", framework, NUnitConsoleName),
                args.ToArray(),
                environmentVariables,
                cases);
        }

        private static CaseDto CreateGeneralCreateCaseDto(string caseId)
        {
            return new CaseDto(caseId, new[] { string.Format("{0}={1}", IncludeArg, caseId) });
        }

        private static CaseDto CreateMultithreadingCreateCaseDto(string caseId, int threadsCount = 20)
        {
            return new CaseDto(caseId, new[] { string.Format("{0}={1}", IncludeArg, caseId), string.Format("{0}={1}", WorkersArg, threadsCount) });
        }
    }
}
