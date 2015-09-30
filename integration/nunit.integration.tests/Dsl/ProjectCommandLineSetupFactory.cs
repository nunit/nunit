namespace nunit.integration.tests.Dsl
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    internal class ProjectCommandLineSetupFactory : ICommandLineSetupFactory
    {
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

            var projectContent = new XDocument(
                new XElement(
                    "NUnitProject",
                    new XElement(
                        "Settings",
                        new XAttribute("activeconfig", "active"),
                        new XAttribute("appbase", appBase)),
                    new XElement(
                        "Config",
                        new XAttribute("binpath", string.Join(",", assemblies.Select(Path.GetDirectoryName).Distinct())),
                        new XAttribute("name", "active"),
                        assemblies.Select(path => new XElement("assembly", new XAttribute("path", path)))))).ToString();


            var artifact = new CommandLineArtifact(Path.GetFullPath(Path.Combine(ctx.SandboxPath, "project.nunit")), projectContent);

            return new CommandLineSetup(
                Path.Combine(configuration.NUnitConsolePath, "nunit-console.exe"),
                ctx.CurrentDirectory,
                artifact.FileName
                + " "
                + string.Join(" ", args.Select(arg => arg.ConvertToString(configuration.NUnitVersion))),
                configuration.EnvVariables.ToDictionary(envVariable => envVariable.GetName(configuration.NUnitVersion), envVariable => envVariable.GetValue(configuration.NUnitVersion)),
                artifact);
        }
    }
}
