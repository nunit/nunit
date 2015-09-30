namespace nunit.integration.tests.Dsl
{
    using System;
    using System.IO;
    using System.Linq;

    internal class ArgumentsCommandLineSetupFactory : ICommandLineSetupFactory
    {
        public CommandLineSetup Create(TestContext ctx)
        {
            var configuration = ctx.GetOrCreateNUnitConfiguration();
            return new CommandLineSetup(
                Path.Combine(configuration.NUnitConsolePath, "nunit-console.exe"),
                ctx.CurrentDirectory,                
                string.Join(" ", configuration.AssemblyFileNames)
                + " "
                + string.Join(" ", configuration.Args.Select(arg => arg.ConvertToString(configuration.NUnitVersion))),
                configuration.EnvVariables.ToDictionary(envVariable => envVariable.GetName(configuration.NUnitVersion), envVariable => envVariable.GetValue(configuration.NUnitVersion)));
        }
    }
}
