namespace nunit.integration.tests.Dsl
{
    using System.IO;
    using System.Linq;

    internal class ArgumentsCommandLineSetupFactory : ICommandLineSetupFactory
    {
        private const string ConfigFileResourceName = "nunit.integration.tests.Templates.App.config";
        private readonly static ResourceManager ResourceManager = new ResourceManager();

        public CommandLineSetup Create(TestContext ctx)
        {
            var configuration = ctx.GetOrCreateNUnitConfiguration();
            var configFiles =
                from configFile in configuration.ConfigFiles
                select new CommandLineArtifact(configFile.ConfigFileName, ResourceManager.GetContentFromResource(ConfigFileResourceName));
            var artifacts = configFiles.ToArray();

            return new CommandLineSetup(
                Path.Combine(configuration.NUnitConsolePath, Const.NUnitConsoleFileName),
                ctx.CurrentDirectory,                
                string.Join(" ", configuration.AssemblyFileNames)
                + " "
                + string.Join(" ", configuration.Args.Select(arg => arg.ConvertToString(configuration.NUnitVersion))),
                configuration.EnvVariables.ToDictionary(envVariable => envVariable.GetName(configuration.NUnitVersion), envVariable => envVariable.GetValue(configuration.NUnitVersion)),
                artifacts);
        }
    }
}
