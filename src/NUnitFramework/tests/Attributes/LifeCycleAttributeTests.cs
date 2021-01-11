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

using System.Linq;
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
        public void SetupTearDownIsCalledOnce()
        {
            var fixture = TestBuilder.MakeFixture(typeof(SetupAndTearDownFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), result.Message);
        }

        [Test]
        public void OneTimeSetupTearDownIsCalledOnce()
        {
            StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeSetupCount = 0;
            StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeTearDownCount = 0;

            var fixture = TestBuilder.MakeFixture(typeof(StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(1, StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeSetupCount);
            Assert.AreEqual(1, StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase.TotalOneTimeTearDownCount);
        }

        [Test]
        public void InstanceOneTimeSetupTearDownThrows()
        {
            var fixture = TestBuilder.MakeFixture(typeof(InstanceOneTimeSetupAndTearDownFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Label, Is.EqualTo("Error"));
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

            Assert.AreEqual(3, DisposableLifeCycleFixtureInstancePerTestCase.DisposeCalls);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestFixtureSourceTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithTestFixtureSourceFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

#if NETFRAMEWORK
        [Test]
        public void AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var code = @"
                using NUnit.Framework;

                [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

                [TestFixture]
                public class AssemblyLevelFixtureLifeCycleTest
                {
                    private int _value;

                    [Test]
                    public void Test1()
                    {
                        Assert.AreEqual(0, _value++);
                    }

                    [Test]
                    public void Test2()
                    {
                        Assert.AreEqual(0, _value++);
                    }
                }
                ";

            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("AssemblyLevelFixtureLifeCycleTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }
#endif

        [Test]
        public void InstancePerTestCaseWithRepeatShouldWorkAsExpected()
        {
            RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter = 0;
            var fixture = TestBuilder.MakeFixture(typeof(RepeatingLifeCycleFixtureInstancePerTestCase));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(3, RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter);
        }

        [Test]
        public void ConstructorIsCalledOnceForEachTestInParallelTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(ParallelLifeCycleFixtureInstancePerTestCase)); 
            
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }
    }
}
