namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;

    internal class TestSession
    {
        private static readonly TestOutputSummaryParser TestOutputSummaryParser = new TestOutputSummaryParser();
        private static readonly ServiceMessageParser ServiceMessageParser = new ServiceMessageParser();

        private readonly TestContext _context;

        private readonly string _nunitPath;
        private readonly string _cmdArguments;

        private readonly string _output;

        public TestSession(TestContext context, string nunitPath, string cmdArguments, int exitCode, string output)
        {
            _context = context;
            _nunitPath = nunitPath;
            _cmdArguments = cmdArguments;
            ExitCode = exitCode;
            _output = output;

            if (_output != null)
            {
                TestOutputSummary = TestOutputSummaryParser.Parse(output);
                TeamCityServiceMessages = ServiceMessageParser.ParseServiceMessages(_output).ToList();                
            }
        }

        public int ExitCode { get; }

        public OutputSummary TestOutputSummary { get; private set; }

        public IEnumerable<IServiceMessage> TeamCityServiceMessages { get; }

        public override string ToString()
        {
            return $"cd {_context.SandboxPath}";
        }

        public string GetCommandLine()
        {
            return $"{_nunitPath} {_cmdArguments}";
        }

        public string GetResult()
        {
            return $"Exit code: {ExitCode}\nOutput:\n{_output}";
        }
    }
}
