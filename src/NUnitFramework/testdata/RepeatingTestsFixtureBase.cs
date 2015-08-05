using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.TestData.RepeatingTests
{
    [TestFixture]
    public class RepeatingTestsFixtureBase
    {
        private int fixtureSetupCount;
        private int fixtureTeardownCount;
        private int setupCount;
        private int teardownCount;
        protected int count;

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
        public int Count
        {
            get { return count; }
        }
    }
}
