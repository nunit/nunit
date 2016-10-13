namespace nunit.integration.tests.Dsl
{
    using TechTalk.SpecFlow;

    internal static class ScenarioContextExtensions
    {
        private const string ContextKey = "TestContext";

        public static TestContext GetTestContext(this ScenarioContext scenarioContext)
        {
            object testContextObj;
            TestContext testContext;
            if (!scenarioContext.TryGetValue(ContextKey, out testContextObj))
            {
                testContext = new TestContext();
                scenarioContext[ContextKey] = testContext;
            }
            else
            {
                testContext = (TestContext)testContextObj;
            }
            
            return testContext;
        }
    }
}
