namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    internal class CommandLineSetup
    {
        public CommandLineSetup(string toolName, string workingDirectory, string arguments, IDictionary<string, string> envVariables, params CommandLineArtifact[] artifacts)
        {
            ToolName = toolName;
            WorkingDirectory = workingDirectory;
            Arguments = arguments;
            EnvVariables = envVariables;
            Artifacts = artifacts.ToImmutableList();
        }

        public string ToolName { get; private set; }

        public string WorkingDirectory { get; private set; }

        public string Arguments { get; private set; }

        public IDictionary<string, string> EnvVariables { get; private set; }

        public IEnumerable<CommandLineArtifact> Artifacts { get; private set; }
    }
}
