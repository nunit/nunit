namespace nunit.integration.tests.Dsl
{
    internal interface ICommandLineSetupFactory
    {
        CommandLineSetup Create(TestContext ctx);
    }
}