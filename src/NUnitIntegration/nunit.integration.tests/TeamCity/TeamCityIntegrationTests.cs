using System.Linq;
using System.Text;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity
{
    public sealed class TeamCityIntegrationTests
    {        
        public static object[] CaseSource
        {
            get
            {
                using (ServiceLocator.Root.RegisterExtension(new ServiceLocatorConfigurationExtension()))
                {
                    return ServiceLocator.Root.GetService<ICertEngine>().Run(new CertDtoFactory().CreateCert()).Select(i => (object)i).ToArray();
                }
            }        
        }

        [Test, TestCaseSource("CaseSource"), Category("Integration")]
        public void Case(ITestResultEvaluator resultEvaluator)
        {
            var result = resultEvaluator.Evaluate();
            var details = CreateDetails(result);
            System.Diagnostics.Debug.WriteLine(details);
            if (result.State == TestState.Failed)
            {
                Assert.Fail(details);
            }

            if (result.State == TestState.Ignored)
            {
                Assert.Ignore(details);
            }

            if (result.State == TestState.UnknownCase || result.State == TestState.Exception || result.State == TestState.NotImplemented)
            {
                Assert.Inconclusive(details);
            }

            if (result.State == TestState.Passed)
            {
                Assert.Pass(details);
            }
        }

        private string CreateDetails(params TestResultDto[] results)
        {            
            var sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendFormat("Case {0} - {1}", result, result.State);
                sb.AppendLine();
                sb.AppendLine(result.Details);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
