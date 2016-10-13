namespace nunit.integration.tests.Dsl
{
    internal class ConfigFile
    {        
        public ConfigFile(string configFileName)
        {
            ConfigFileName = configFileName;
        }

        public string ConfigFileName { get; private set; }
    }
}
