// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.RepeatingTests
{
    [TestFixture]
    public class RepeatingTestsFixtureBase
    {
        private int fixtureSetupCount;
        private int fixtureTeardownCount;
        private int setupCount;
        private int teardownCount;
        private readonly List<string> tearDownResults = new List<string>();

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            fixtureSetupCount++;
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            fixtureTeardownCount++;
        }

        [SetUp]
        public void SetUp()
        {
            setupCount++;
        }

        [TearDown]
        public void TearDown()
        {
            tearDownResults.Add(TestContext.CurrentContext.Result.Outcome.ToString());
            teardownCount++;
        }

        public int FixtureSetupCount
        {
            get { return fixtureSetupCount; }
        }
        public int FixtureTeardownCount
        {
            get { return fixtureTeardownCount; }
        }
        public int SetupCount
        {
            get { return setupCount; }
        }
        public int TeardownCount
        {
            get { return teardownCount; }
        }

        public List<string> TearDownResults
        {
            get { return tearDownResults; }
        }
        public int Count { get; protected set; }
    }
}
