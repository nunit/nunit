// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    internal sealed class Issue4659
    {
        [TestCaseSource(nameof(GetSomeExceptionCases))]
        public void NoMethod_GivenException_ShouldPassTest(Exception testCase) => Assert.That(testCase, Is.Not.InstanceOf<string>());

        private static TestCaseData[] GetSomeExceptionCases() => new[]
        {
            new TestCaseData(new ArgumentNullException()),
            new TestCaseData(new ArgumentNullException("message", new Exception("Some Exception Message"))),
        };
    }
}
