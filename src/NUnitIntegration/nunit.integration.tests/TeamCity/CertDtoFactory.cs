// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
        private const string NoResultArg = "--noresult";
        private const string TeamCityEnvVar = "TEAMCITY_PROJECT_NAME";
        private const string MockTestsAssemblyName = "nunit.integration.mocks.dll";
        private const string NUnitConsoleName = "nunit-console.exe";

        public static CertDto CreateCert()
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
                string.Format(@"net{0}\{1}", framework, MockTestsAssemblyName),
                NoResultArg
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
