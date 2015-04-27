using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class CertEngine : ICertEngine
    {
        public IEnumerable<TestResultDto> Run(CertDto cert)
        {
            Contract.Requires<ArgumentNullException>(cert != null);
            Contract.Ensures(Contract.Result<IEnumerable<TestResultDto>>() != null);

            var testsRepository = ServiceLocator.Root.GetService<ITestsRepository>();
            foreach (var cmdLineTool in cert.CmdLineTools)
            {
                var caseDict = cmdLineTool.Cases.ToDictionary(i => i.CaseId, i => i, StringComparer.CurrentCultureIgnoreCase);

                var testList = testsRepository.GetCmdLineToolTests(cmdLineTool.CertType);
                foreach (var test in testList)
                {
                    CaseDto curCase;
                    if (!caseDict.TryGetValue(test.CaseId, out curCase))
                    {
                        yield return new TestResultDto(cmdLineTool.ToolId, test.CaseId, TestState.NotImplemented);
                        continue;
                    }

                    caseDict.Remove(test.CaseId);
                    TestResultDto testResult;
                    try
                    {
                        testResult = test.Run(cmdLineTool, curCase);
                    }
                    catch (Exception ex)
                    {
                        testResult = new TestResultDto(cmdLineTool.ToolId, test.CaseId, TestState.Exception)
                        {
                            Details = ex.Message                                             
                        };
                    }

                    yield return testResult;
                }

                foreach (var caseDto in caseDict)
                {
                    yield return new TestResultDto(cmdLineTool.ToolId, caseDto.Key, TestState.UnknownCase);
                }
            }
        }
    }
}
