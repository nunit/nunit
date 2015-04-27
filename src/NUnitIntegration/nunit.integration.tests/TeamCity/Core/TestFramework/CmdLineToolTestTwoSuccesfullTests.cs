namespace JetBrains.TeamCityCert.Tools.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;

    using JetBrains.TeamCityCert.Tools.Common;
    using JetBrains.TeamCityCert.Tools.Contracts;

    internal class CmdLineToolTestTwoSuccesfullTests : ICmdLineToolTest
    {
        public CertType CertType
        {
            get { return CertType.TestFramework; }
        }

        public string CaseId 
        {
            get
            {
                return "TwoSuccesfullTests";
            }
        }

        public string Description 
        {
            get
            {
                return "Two Succesfull Tests";
            }
        }

        public TestResultDto Run(CmdLineToolDto cmdLineToolDto, CaseDto caseDto)
        {
            Contract.Requires<ArgumentNullException>(cmdLineToolDto != null);
            Contract.Requires<ArgumentNullException>(caseDto != null);

            var processManager = ServiceLocator.Root.GetService<IProcessManager>();
            var output = processManager.StartProcess(cmdLineToolDto.CmdLineFileName, cmdLineToolDto.Args.Concat(caseDto.Args));
            var messageParser = ServiceLocator.Root.GetService<IServiceMessageParser>();

            var testState = TestState.Passed;
            var details = new StringBuilder();

            var rawMessages = output.OutputLines.Select(line => messageParser.ParseServiceMessages(new StringReader(line))).SelectMany(i => i).ToList();
            var validatedMessages = new List<IServiceMessage>();
            var messageValidator = ServiceLocator.Root.GetService<IServiceMessageValidator>();
            foreach (var message in rawMessages)
            {
                var messageValidationResult = messageValidator.Validate(message);                
                switch (messageValidationResult.State)
                {
                    case ValidationState.Valid:
                        validatedMessages.Add(message);
                        break;

                    case ValidationState.HasWarning:
                        details.AppendLine(string.Format("Message \"{0}\" has warning(s)", message.Name));
                        validatedMessages.Add(message);
                        break;

                    case ValidationState.Unknow:
                        details.AppendLine(string.Format("Message \"{0}\" is unknown", message.Name));
                        break;

                    case ValidationState.NotValid:
                        details.AppendLine(string.Format("Message \"{0}\" is not valid", message.Name));
                        testState = TestState.Failed;
                        break;
                }

                foreach (var validationDetail in messageValidationResult.Details)
                {
                    details.AppendLine(string.Format("\t{0}", validationDetail));
                }
            }

            if (testState != TestState.Passed)
            {
                return new TestResultDto(cmdLineToolDto.ToolId, CaseId, TestState.Failed) { Details = details.ToString() };
            }

            var structureValidationResult = ServiceLocator.Root.GetService<IServiceMessageStructureValidator>().Validate(validatedMessages);
            switch (structureValidationResult.State)
            {
                case ValidationState.HasWarning:
                    details.AppendLine("Message structure validation has warning(s)");
                    break;

                case ValidationState.Unknow:
                    details.AppendLine("Message structure validation result is unknown");
                    break;

                case ValidationState.NotValid:
                    details.AppendLine("Message structure validation has errors");
                    testState = TestState.Failed;
                    break;
            }

            foreach (var validationDetail in structureValidationResult.Details)
            {
                details.AppendLine(string.Format("\t{0}", validationDetail));
            }

            if (testState != TestState.Passed)
            {
                return new TestResultDto(cmdLineToolDto.ToolId, CaseId, TestState.Failed) { Details = details.ToString() };
            }

            return new TestResultDto(cmdLineToolDto.ToolId, CaseId, TestState.Passed) { Details = details.ToString() };
        }
    }
}
