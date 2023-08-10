// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;

namespace NUnit.TestData.RepeatingTests
{
    [TestFixture]
    public class RepeatingTestsFixtureBase
    {
        private int _fixtureSetupCount;
        private int _fixtureTeardownCount;
        private int _setupCount;
        private int _teardownCount;
        private readonly List<string> _tearDownResults = new List<string>();

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _fixtureSetupCount++;
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _fixtureTeardownCount++;
        }

        [SetUp]
        public void SetUp()
        {
            _setupCount++;
        }

        [TearDown]
        public void TearDown()
        {
            _tearDownResults.Add(TestContext.CurrentContext.Result.Outcome.ToString());
            _teardownCount++;
        }

        public int FixtureSetupCount => _fixtureSetupCount;

        public int FixtureTeardownCount => _fixtureTeardownCount;

        public int SetupCount => _setupCount;

        public int TeardownCount => _teardownCount;

        public List<string> TearDownResults => _tearDownResults;
        public int Count { get; protected set; }
    }
}
