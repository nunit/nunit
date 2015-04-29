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
        public IEnumerable<ITestResultEvaluator> Run(CertDto cert)
        {
            Contract.Requires<ArgumentNullException>(cert != null);
            Contract.Ensures(Contract.Result<IEnumerable<TestResultDto>>() != null);

            return RunInternal(cert).ToList();
        }

        [NotNull]
        private IEnumerable<ITestResultEvaluator> RunInternal([NotNull] CertDto cert)
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
                    var description = string.Format("{0} {1}", cmdLineTool.ToolId, @case.CaseId);
                    if (!caseDict.TryGetValue(@case.CaseId, out curCase))
                    {
                        yield return new TestResultEvaluator(new TestResultDto(cmdLineTool.ToolId, @case.CaseId, TestState.NotImplemented, new Details()), description);
                        continue;
                    }

                    caseDict.Remove(@case.CaseId);
                    yield return new TestResultEvaluator(() => CheckCmdLineToolCase(cmdLineTool, curCase, @case), description);
                }

                foreach (var caseDto in caseDict)
                {
                    var description = string.Format("{0} {1}", cmdLineTool.ToolId, caseDto.Key);
                    yield return new TestResultEvaluator(new TestResultDto(cmdLineTool.ToolId, caseDto.Key, TestState.UnknownCase, new Details()), description);
                }
            }
        }

        private static TestResultDto CheckCmdLineToolCase(CmdLineToolDto cmdLineTool, CaseDto caseInfo, ICase @case)
        {
            TestResultDto testResult;
            try
            {
                var processManager = ServiceLocator.Root.GetService<IProcessManager>();
                var output = processManager.StartProcess(cmdLineTool.CmdLineFileName, cmdLineTool.Args.Concat(caseInfo.Args), cmdLineTool.EnvironmentVariables);
                var rawMessages = output.OutputLines.Select(line => ServiceLocator.Root.GetService<IServiceMessageParser>().ParseServiceMessages(new StringReader(line))).SelectMany(i => i).ToList();                
                if (rawMessages.Count == 0)
                {
                    testResult = new TestResultDto(cmdLineTool.ToolId, caseInfo.CaseId, TestState.UnknownCase, new Details(string.Join(Environment.NewLine, output.OutputLines)));
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

                    testResult = new TestResultDto(cmdLineTool.ToolId, caseInfo.CaseId, testState, new Details(string.Join(Environment.NewLine, validationResult.Details)));
                }
            }
            catch (Exception ex)
            {
                testResult = new TestResultDto(cmdLineTool.ToolId, @case.CaseId, TestState.Exception, new Details(ex.Message));
            }

            return testResult;
        }
    }
}
