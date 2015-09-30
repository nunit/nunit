namespace nunit.integration.tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class TestContext
    {
        private readonly Dictionary<string, TestAssembly> _assemblies = new Dictionary<string, TestAssembly>(StringComparer.InvariantCultureIgnoreCase);
        private readonly NUnitConfiguration _nUnitConfiguration = new NUnitConfiguration();

        public TestContext()
        {
            SandboxPath = Path.GetFullPath(GetSandboxPath());
            CurrentDirectory = SandboxPath;
        }

        public string SandboxPath { get; private set; }

        public string CurrentDirectory { get; set; }

        public TestSession TestSession { get; set; }

        public TestAssembly GetOrCreateAssembly(string assemblyName)
        {            
            TestAssembly testAssembly;
            if (!_assemblies.TryGetValue(assemblyName, out testAssembly))
            {
                _assemblies[assemblyName] = testAssembly = new TestAssembly(assemblyName);
            }

            return testAssembly;
        }

        public NUnitConfiguration GetOrCreateNUnitConfiguration()
        {
            return _nUnitConfiguration;
        }

        public override string ToString() => TestSession?.ToString() ?? string.Empty;

        private static string GetSandboxPath()
        {
            return NUnit.Framework.TestContext.CurrentContext.Test.Name?
                .Replace("(", "_")
                .Replace("\"", string.Empty)
                .Replace(",null", string.Empty)
                .Replace(",", "_")
                .Replace(")", string.Empty) ?? string.Empty;
        }
    }
}
