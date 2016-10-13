namespace nunit.integration.tests.Dsl
{
    internal class CommandLineArtifact
    {
        public CommandLineArtifact(string fileName, string content)
        {
            FileName = fileName;
            Content = content;
        }

        public string FileName { get; private set; }

        public string Content { get; private set; }
    }
}
