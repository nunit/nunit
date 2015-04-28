using System;
using System.Collections.Generic;
using System.IO;
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

            var caseRepository = ServiceLocator.Root.GetService<ICaseRepository>();
            foreach (var cmdLineTool in cert.CmdLineTools)
            {
                var caseDict = cmdLineTool.Cases.ToDictionary(i => i.CaseId, i => i, StringComparer.CurrentCultureIgnoreCase);

                var cases = caseRepository.GetCases(cmdLineTool.CertType);
                foreach (var @case in cases)
                {
                    CaseDto curCase;
                    if (!caseDict.TryGetValue(@case.CaseId, out curCase))
                    {
                        yield return new TestResultDto(cmdLineTool.ToolId, @case.CaseId, TestState.NotImplemented);
                        continue;
                    }

                    caseDict.Remove(@case.CaseId);
                    TestResultDto testResult;
                    try
                    {
                        var processManager = ServiceLocator.Root.GetService<IProcessManager>();
                        var output = processManager.StartProcess(cmdLineTool.CmdLineFileName, cmdLineTool.Args.Concat(curCase.Args), cmdLineTool.EnvironmentVariables);
                        if (output.ExitCode != 0)
                        {
                            testResult = new TestResultDto(cmdLineTool.ToolId, curCase.CaseId, TestState.Failed)
                            {
                                Details = string.Join(Environment.NewLine, output)
                            };
                        }
                        else
                        {
                            var rawMessages = output.OutputLines.Select(line => ServiceLocator.Root.GetService<IServiceMessageParser>().ParseServiceMessages(new StringReader(line))).SelectMany(i => i).ToList();
                            if (rawMessages.Count == 0)
                            {
                                testResult = new TestResultDto(cmdLineTool.ToolId, curCase.CaseId, TestState.UnknownCase) { Details = string.Join(Environment.NewLine, output.OutputLines) };
                            }
                            else
                            {                                
                                var validationResult = @case.Validate(rawMessages);
                                TestState testState;
                                switch (validationResult.State)
                                {
                                    case ValidationState.Valid:
                                    case ValidationState.HasWarning:
                                        testState = TestState.Passed;
                                        break;

                                    case ValidationState.NotValid:
                                        testState = TestState.Failed;
                                        break;

                                    case ValidationState.Unknown:
                                        testState = TestState.Ignored;
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
                    }
                    catch (Exception ex)
                    {
                        testResult = new TestResultDto(cmdLineTool.ToolId, @case.CaseId, TestState.Exception)
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
