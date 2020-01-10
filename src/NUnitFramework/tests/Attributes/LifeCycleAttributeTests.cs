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
        public void OneTimeSetupTearDownIsCalledOnFirstInstance()
        {
            OneTimeSetupAndTearDownFixtureInstancePerTestCase.OneTimeSetupGuid = Guid.Empty;
            OneTimeSetupAndTearDownFixtureInstancePerTestCase.OneTimeTearDownGuid = Guid.Empty;

            var fixture = new OneTimeSetupAndTearDownFixtureInstancePerTestCase();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreNotEqual(fixture.Guid, Guid.Empty);
            Assert.AreEqual(fixture.Guid, OneTimeSetupAndTearDownFixtureInstancePerTestCase.OneTimeSetupGuid);
            Assert.AreEqual(fixture.Guid, OneTimeSetupAndTearDownFixtureInstancePerTestCase.OneTimeTearDownGuid);
        }

        [Test]
        public void SetupTearDownIsCalledOnce()
        {
            var fixture = new SetupAndTearDownFixtureInstancePerTestCase();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.TotalSetupCount);
            Assert.AreEqual(1, fixture.TotalTearDownCount);
        }

        [Test]
        public void OneTimeSetupTearDownIsCalledOnce()
        {
            OneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeSetupCount = 0;
            OneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeTearDownCount = 0;

            var fixture = TestBuilder.MakeFixture(typeof(OneTimeSetupAndTearDownFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(1, OneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeSetupCount);
            Assert.AreEqual(1, OneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeTearDownCount);
        }

        [Test]
        public void InstancePerTestCaseCreatesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(CountingLifeCycleTestFixture));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.InstancePerTestCase);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void SingleInstanceSharesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(CountingLifeCycleTestFixture));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.SingleInstance);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(
                result.Children.Select(t => t.ResultState),
                Is.EquivalentTo(new[] { ResultState.Success, ResultState.Failure }));
        }

        [Test]
        public void InstancePerTestCaseShouldDisposeForEachTestCase()
        {
            DisposableLifeCycleFixtureInstancePerTestCase.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(DisposableLifeCycleFixtureInstancePerTestCase));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(2, DisposableLifeCycleFixtureInstancePerTestCase.DisposeCalls);
        }

        [Test]
        public void InstancePerTestCaseWithRepeatShouldWorkAsExpected()
        {
            RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter = 0;
            var fixture = TestBuilder.MakeFixture(typeof(RepeatingLifeCycleFixtureInstancePerTestCase));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(3, RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter);
        }
    }
}
