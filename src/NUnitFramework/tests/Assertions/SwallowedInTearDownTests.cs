// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.SwallowedInTearDownData;

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// A TearDown that swallows Assert.Pass, Ignore or Inconclusive keeps the outcome of the test.
    /// </summary>
    [TestFixture]
    public class SwallowedInTearDownTests
    {
        private static readonly TestCaseData[] Cases =
        [
            new TestCaseData(typeof(FailedTestSwallowsPassInTearDown), ResultState.Failure, "Test failed"),
            new TestCaseData(typeof(PassedTestSwallowsInconclusiveInTearDown), ResultState.Success, string.Empty),
            new TestCaseData(typeof(IgnoredTestSwallowsPassInTearDown), ResultState.Ignored, "Test ignored"),
            new TestCaseData(typeof(InconclusiveTestSwallowsPassInTearDown), ResultState.Inconclusive, "Test inconclusive"),
        ];

        [TestCaseSource(nameof(Cases))]
        public void TearDownKeepsOutcome(Type fixture, ResultState expectedState, string expectedMessage)
        {
            ITestResult result = TestBuilder.RunTestCase(fixture, "Test");

            Assert.That(result.ResultState, Is.EqualTo(expectedState));
            Assert.That(result.Message, Is.EqualTo(expectedMessage));
        }
    }
}
