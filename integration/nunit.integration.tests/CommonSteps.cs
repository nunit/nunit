namespace nunit.integration.tests
{
    using System;
    using System.IO;

    using nunit.integration.tests.Dsl;

    using TechTalk.SpecFlow;

    [Binding]
    public class CommonSteps
    {
        [Given(@"I have change current directory to (.+)")]
        public void ChangeCurrentDirectory(string newCurrentDirectory)
        {
            var ctx = ScenarioContext.Current.GetTestContext();
            newCurrentDirectory = Path.GetFullPath(Path.Combine(ctx.SandboxPath, newCurrentDirectory));
            if (!Directory.Exists(newCurrentDirectory))
            {
                Directory.CreateDirectory(newCurrentDirectory);
            }

            ctx.CurrentDirectory = newCurrentDirectory;
        }
    }
}
