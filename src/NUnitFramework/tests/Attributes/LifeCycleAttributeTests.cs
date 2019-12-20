// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.LifeCycleTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class LifeCycleAttributeTests
    {
        [Test]
        public void InstancePerTestCaseCreatesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleFixtureTestCountIsAlwaysOne));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.InstancePerTestCase);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.ToString(), Is.EqualTo("Passed"));
        }

        [Test]
        public void SingleInstanceSharesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleFixtureTestCountIsAlwaysOne));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.SingleInstance);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.AreEqual(result.Children.Count(), 2);
            var childTests = result.Children.ToArray();

            Assert.That(childTests.Any(x => x.ResultState == ResultState.Success));
            Assert.That(childTests.Any(x => x.ResultState == ResultState.Failure));
        }

        [Test]
        public void InstancePerTestCaseShouldDisposeForEachTestCase()
        {
            LifeCycleFixtureInstancePerTestCaseDispose.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleFixtureInstancePerTestCaseDispose));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.ToString(), Is.EqualTo("Passed"));

            Assert.AreEqual(2, LifeCycleFixtureInstancePerTestCaseDispose.DisposeCalls);
        }

        [Test]
        public void InstancePerTestCaseWithRepeatShouldWorkAsExpected()
        {
            LifeCycleFixtureInstancePerTestCaseRepeat.RepeatCounter = 0;
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleFixtureInstancePerTestCaseRepeat));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.ToString(), Is.EqualTo("Passed"));

            Assert.AreEqual(3, LifeCycleFixtureInstancePerTestCaseRepeat.RepeatCounter);
        }
    }
}
