using System;
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
        [Test]
        public void Tests()
        {
            var certData = CreateCertData();
            using (ServiceLocator.Root.RegisterExtension(new ServiceLocatorConfigurationExtension()))
            {
                var results = ServiceLocator.Root.GetService<ICertEngine>().Run(certData);

                var fails = results.Where(i => i.State == TestState.Failed || i.State == TestState.Exception).ToList();
                if (fails.Any())
                {
                    Assert.Fail(CreateDetails(fails));                    
                }

                var inconclusive = results.Where(i => i.State == TestState.UnknownCase).ToList();
                if (inconclusive.Any())
                {
                    Assert.Inconclusive(CreateDetails(inconclusive));
                }

                var passed = results.Where(i => i.State == TestState.Passed || i.State == TestState.NotImplemented).ToList();
                if (passed.Any())
                {
                    Assert.Pass(CreateDetails(passed));
                }
            }
        }

        private string CreateDetails(IEnumerable<TestResultDto> results)
        {            
            var sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendFormat("Case {0} {1} - {2}", result.ToolId, result.CaseId, result.State);
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
                        new[] { @"v2.0\NUnit.Tests.dll", "--teamcity" },
                        new[]
                        {
                                new CaseDto("TwoSuccesfullTests", Enumerable.Empty<string>()), 
                        })
                });
        }
    }
}
