namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;

    using Microsoft.Build.Utilities;

    internal class NUnitConfiguration
    {
        private readonly List<string> _assemblyFileNames = new List<string>();
        private readonly List<CmdArg> _args = new List<CmdArg>();
        private readonly List<EnvVariable> _envVariables = new List<EnvVariable>();
        private readonly List<ConfigFile> _configFiles = new List<ConfigFile>();

        public NUnitConfiguration()
        {
            NUnitVersion = NUnitVersion.NUnit3;
            FrameworkVersion = TargetDotNetFrameworkVersion.Version40;
            ConfigurationType = ConfigurationType.CmdArguments;
        }

        public string OriginNUnitPath { get; set; }

        public string NUnitConsolePath { get; set; }

        public string AppBase { get; set; }

        public NUnitVersion NUnitVersion { get; set; }

        public TargetDotNetFrameworkVersion FrameworkVersion { get; set; }

        public IEnumerable<string> AssemblyFileNames => _assemblyFileNames;

        public IEnumerable<CmdArg> Args => _args;

        public IEnumerable<EnvVariable> EnvVariables => _envVariables;

        public IEnumerable<ConfigFile> ConfigFiles => _configFiles;

        public ConfigurationType ConfigurationType { get; set; }

        public void AddAssemblyFile(string assemblyFileName)
        {
            _assemblyFileNames.Add(assemblyFileName);
        }

        public void AddArg(CmdArg arg)
        {
            _args.Add(arg);
        }

        public void AddEnvVariable(EnvVariable envVariable)
        {
            _envVariables.Add(envVariable);
        }

        public void AddConfigFile(ConfigFile configFile)
        {
            _configFiles.Add(configFile);            
        }
    }
}
