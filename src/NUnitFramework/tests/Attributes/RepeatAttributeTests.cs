// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

// TODO: Rework this
// RepeatAttribute should either
//  1) Apply at load time to create the exact number of tests, or
//  2) Apply at run time, generating tests or results dynamically
//
// #1 is feasible but doesn't provide much benefit
// #2 requires infrastructure for dynamic test cases first
using System;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.RepeatingTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture] 
    public class RepeatAttributeTests
    {
        [TestCase(typeof(RepeatFailOnFirstTryFixture), "Failed(Child)", 1)]
        [TestCase(typeof(RepeatFailOnSecondTryFixture), "Failed(Child)", 2)]
        [TestCase(typeof(RepeatFailOnThirdTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatSuccessFixture), "Passed", 3)]
        [TestCase(typeof(RepeatedTestWithIgnoreAttribute), "Skipped:Ignored", 0)]
        [TestCase(typeof(RepeatIgnoredOnFirstTryFixture), "Skipped:Ignored", 1)]
        [TestCase(typeof(RepeatIgnoredOnSecondTryFixture), "Skipped:Ignored", 2)]
        [TestCase(typeof(RepeatIgnoredOnThirdTryFixture), "Skipped:Ignored", 3)]
        [TestCase(typeof(RepeatErrorOnFirstTryFixture), "Failed(Child)", 1)]
        [TestCase(typeof(RepeatErrorOnSecondTryFixture), "Failed(Child)", 2)]
        [TestCase(typeof(RepeatErrorOnThirdTryFixture), "Failed(Child)", 3)]
        public void RepeatWorksAsExpected(Type fixtureType, string outcome, int nTries)
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(fixtureType);
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.That(result.ResultState.ToString(), Is.EqualTo(outcome));
            Assert.AreEqual(1, fixture.FixtureSetupCount);
            Assert.AreEqual(1, fixture.FixtureTeardownCount);
            Assert.AreEqual(nTries, fixture.SetupCount);
            Assert.AreEqual(nTries, fixture.TeardownCount);
            Assert.AreEqual(nTries, fixture.Count);
        }

        [Test]
        public void CategoryWorksWithRepeatedTest()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(RepeatedTestWithCategory));
            Test test = suite.Tests[0] as Test;
            System.Collections.IList categories = test.Properties["Category"];
            Assert.IsNotNull(categories);
            Assert.AreEqual(1, categories.Count);
            Assert.AreEqual("SAMPLE", categories[0]);
        }
    }
}
