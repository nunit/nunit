// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

        public int FixtureSetupCount => fixtureSetupCount;

        public int FixtureTeardownCount => fixtureTeardownCount;

        public int SetupCount => setupCount;

        public int TeardownCount => teardownCount;

        public List<string> TearDownResults => tearDownResults;
        public int Count { get; protected set; }
    }
}
