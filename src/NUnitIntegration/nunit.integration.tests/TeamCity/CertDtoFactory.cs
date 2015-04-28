using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    internal sealed class CertDtoFactory
    {
        public CertDto CreateCert()
        {
            return new CertDto(
                new[]
                    {
                        // CreateCmdLineToolDto("20", IntegrationType.CommandArg),
                        // CreateCmdLineToolDto("20", IntegrationType.EnvironmentVar),
                        // CreateCmdLineToolDto("40", IntegrationType.CommandArg),
                        // CreateCmdLineToolDto("40", IntegrationType.EnvironmentVar),
                        // CreateCmdLineToolDto("45", IntegrationType.CommandArg),
                        CreateCmdLineToolDto("45", IntegrationType.EnvironmentVar),
                    });
        }

        private static CmdLineToolDto CreateCmdLineToolDto(string framework, IntegrationType integrationType)
        {
            var args = new List<string>
            {
                string.Format(@"net{0}\NUnit.Integration.Mocks.dll", framework)
            };

            var environmentVariables = new Dictionary<string, string>();
            string integrationTypeDescription = "unknown integration type";
            switch (integrationType)
            {
                case IntegrationType.CommandArg:
                    args.Add("--teamcity");
                    integrationTypeDescription = "--teamcity";
                    break;

                case IntegrationType.EnvironmentVar:
                    environmentVariables.Add("TEAMCITY_PROJECT_NAME", "Test");
                    integrationTypeDescription = "env var";
                    break;
            }

            return new CmdLineToolDto(
                CertType.TestFramework,
                string.Format("NUnit v{0} using {1}", framework, integrationTypeDescription),
                string.Format(@"net{0}\nunit-console.exe", framework),
                args.ToArray(),
                environmentVariables,
                CaseList.Cases.Select(CreateCaseDto));
        }

        private static CaseDto CreateCaseDto(string caseId)
        {
            return new CaseDto(caseId, new[] { string.Format("--include={0}", caseId) });
        }
    }
}
