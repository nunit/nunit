// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
#pragma warning disable 1998
    public class RealAsyncSetupTeardownTests
    {
        private object _initializedOnce;
        private object _initializedEveryTime;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _initializedOnce = new object();
        }

        [SetUp]
        public async Task Setup()
        {
            Assume.That(_initializedOnce, Is.Not.Null);
            _initializedEveryTime = new object();
        }


        [Test]
        public void TestCurrentFixtureInitialization()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_initializedOnce, Is.Not.Null);
                Assert.That(_initializedEveryTime, Is.Not.Null);
            });

            _initializedEveryTime = null;
        }

        [TearDown]
        public async Task TearDown()
        {
            Assume.That(_initializedEveryTime, Is.Null);
            _initializedOnce = null;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            Assume.That(_initializedOnce, Is.Null);
        }

    }
#pragma warning restore 1998
}
