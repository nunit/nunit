// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    internal class FailingActionAttributeTests
    {
        [Test]
        public void FailureInAfterFixtureIsRecorded()
        {
            var suite = TestBuilder.MakeFixture(typeof(FailingActionAttributeOnFixtureFixture));

            var fixtureResult = TestBuilder.RunTest(suite);

            Assert.That(fixtureResult.ResultState, Is.EqualTo(ResultState.Error));
        }

        [Test]
        public void FailureInAfterTestIsRecorded()
        {
            var suite = TestBuilder.MakeFixture(typeof(FailingActionAttributeOnTestFixture));

            var fixtureResult = TestBuilder.RunTest(suite);

            Assert.That(fixtureResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
        }
    }
}
