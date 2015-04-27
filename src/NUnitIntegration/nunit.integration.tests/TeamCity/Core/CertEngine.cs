using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

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

            return RunInternal(cert).ToList();
        }

        [NotNull] 
        private IEnumerable<TestResultDto> RunInternal([NotNull] CertDto cert)
        {
            Contract.Requires<ArgumentNullException>(cert != null);
            Contract.Ensures(Contract.Result<IEnumerable<TestResultDto>>() != null);

            var testsRepository = ServiceLocator.Root.GetService<ICaseRepository>();
            foreach (var cmdLineTool in cert.CmdLineTools)
            {
                var caseDict = cmdLineTool.Cases.ToDictionary(i => i.CaseId, i => i, StringComparer.CurrentCultureIgnoreCase);

                var testList = testsRepository.GetCases(cmdLineTool.CertType);
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
                        var processManager = ServiceLocator.Root.GetService<IProcessManager>();
                        var output = processManager.StartProcess(cmdLineTool.CmdLineFileName, cmdLineTool.Args.Concat(curCase.Args));
                        if (output.ExitCode != 0)
                        {
                            testResult = new TestResultDto(cmdLineTool.ToolId, curCase.CaseId, TestState.Failed)
                            {
                                Details = string.Join(Environment.NewLine, output)
                            };
                        }
                        else
                        {
                            var validationResult = test.Validate(output.OutputLines);
                            TestState testState;
                            switch (validationResult.State)
                            {
                                case ValidationState.Valid:
                                case ValidationState.HasWarning:
                                    testState = TestState.Passed;
                                    break;

                                case ValidationState.NotValid:
                                case ValidationState.Unknow:
                                    testState = TestState.Passed;
                                    break;

                                default:
                                    throw new NotImplementedException(string.Format("Unknown validation state \"{0}\"", validationResult.State));
                            }

                            testResult = new TestResultDto(cmdLineTool.ToolId, curCase.CaseId, testState)
                            {
                                Details = string.Join(Environment.NewLine, validationResult.Details)
                            };
                        }
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
