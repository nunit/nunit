namespace nunit.integration.tests.Dsl
{
    internal class TestSession
    {
        private readonly TestContext _context;
        
        public TestSession(TestContext context, int exitCode, string output)
        {
            _context = context;
            ExitCode = exitCode;
            Output = output;
        }

        public int ExitCode { get; }

        public string Output { get; }

        public override string ToString()
        {
            return $"cd {_context.SandboxPath}";
        }
    }
}
