namespace nunit.integration.tests.Dsl
{
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Xml.Linq;

    internal class ProjectCommandLineSetupFactory : ICommandLineSetupFactory
    {
        private const string ConfigFileResourceName = "nunit.integration.tests.Templates.App.config";
        private readonly static ResourceManager ResourceManager = new ResourceManager();

        public CommandLineSetup Create(TestContext ctx)
        {            
            var configuration = ctx.GetOrCreateNUnitConfiguration();
            var appBase = ctx.GetOrCreateNUnitConfiguration().AppBase ?? ctx.CurrentDirectory;

            var args = configuration.Args.ToList();
            var assemblies = (
                from assemblyName in configuration.AssemblyFileNames
                let assembly = Path.GetFileName(assemblyName)
                let fullPath = Path.GetFullPath(Path.GetDirectoryName(assemblyName) ?? string.Empty)
                let relativePath = fullPath.Replace(appBase, string.Empty)
                let normRelativePath = relativePath.Length > 0 && relativePath[0] == Path.DirectorySeparatorChar ? relativePath.Substring(1) : relativePath
                select Path.Combine(normRelativePath, assembly)).ToList();

            var singleConfigFile = configuration.ConfigFiles.FirstOrDefault();
            var configElement = new XElement(
                "Config",
                new XAttribute("name", "active"),                
                assemblies.Select(path => new XElement("assembly", new XAttribute("path", path))));

            if (singleConfigFile != null)
            {
                configElement.Add(new XAttribute("configfile", singleConfigFile.ConfigFileName));
            }

            var projectContent = new XDocument(
                new XElement(
                    "NUnitProject",
                    new XElement(
                        "Settings",
                        new XAttribute("activeconfig", "active"),
                        new XAttribute("appbase", appBase)),
                    configElement
                    )).ToString();


            var projectFile = new CommandLineArtifact(Path.GetFullPath(Path.Combine(ctx.SandboxPath, "project.nunit")), projectContent);
            var configFiles = 
                from configFile in configuration.ConfigFiles
                select new CommandLineArtifact(configFile.ConfigFileName, ResourceManager.GetContentFromResource(ConfigFileResourceName));
            var artifacts = configFiles.Union(Enumerable.Repeat(projectFile, 1)).ToArray();

            return new CommandLineSetup(
                Path.Combine(configuration.NUnitConsolePath, Const.NUnitConsoleFileName),
                ctx.CurrentDirectory,
                projectFile.FileName
                + " "
                + string.Join(" ", args.Select(arg => arg.ConvertToString(configuration.NUnitVersion))),
                configuration.EnvVariables.ToDictionary(envVariable => envVariable.GetName(configuration.NUnitVersion), envVariable => envVariable.GetValue(configuration.NUnitVersion)),
                artifacts);
        }
    }
}
