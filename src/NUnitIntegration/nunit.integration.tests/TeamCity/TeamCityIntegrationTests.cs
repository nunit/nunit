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
                    return ServiceLocator.Root.GetService<ICertEngine>().Run(CertDtoFactory.CreateCert()).Select(i => (object)i).ToArray();
                }
            }
        }

        [Test, TestCaseSource("CaseSource"), Category("Integration")]
        public void Case(ITestResultEvaluator resultEvaluator)
        {
            var result = resultEvaluator.Evaluate();
            var details = CreateDetails(result);
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

        private static string CreateDetails(params TestResultDto[] results)
        {
            var sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendFormat("Case {0} - {1}", result, result.State);
                sb.AppendLine();
                sb.Append(result.Details);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
