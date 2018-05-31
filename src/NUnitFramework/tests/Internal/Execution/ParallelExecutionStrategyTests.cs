// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

#if PARALLEL
using NUnit.Framework.Interfaces;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Execution
{
    public class ParallelExecutionStrategyTests
    {
        private TestExecutionContext _context;
        private TestMethod _testMethod;
        private TestFixture _testFixture;

        [SetUp]
        public void SetUp()
        {
            _context = new TestExecutionContext();
            _testMethod = Fakes.GetTestMethod(GetType(), "TestMethod");
            _testFixture = new TestFixture(typeof(MyFixture));
        }

        [TestCase(ParallelScope.Default, ParallelScope.Default, ParallelExecutionStrategy.Direct)]
        [TestCase(ParallelScope.Self, ParallelScope.Default, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Default, ParallelExecutionStrategy.NonParallel)]
        [TestCase(ParallelScope.Default, ParallelScope.Children, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.Self, ParallelScope.Children, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Children, ParallelExecutionStrategy.NonParallel)]
        [TestCase(ParallelScope.Default, ParallelScope.Fixtures, ParallelExecutionStrategy.Direct)]
        [TestCase(ParallelScope.Self, ParallelScope.Fixtures, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Fixtures, ParallelExecutionStrategy.NonParallel)]
        public void TestCaseStrategy(ParallelScope testScope, ParallelScope contextScope, ParallelExecutionStrategy expectedStrategy)
        {
            var strategy = MakeWorkItem(_testMethod, testScope, contextScope).ExecutionStrategy;
            Assert.That(strategy, Is.EqualTo(expectedStrategy));
        }

        [TestCase(ParallelScope.Default, ParallelScope.Default)]
        [TestCase(ParallelScope.Self, ParallelScope.Default)]
        [TestCase(ParallelScope.None, ParallelScope.Default)]
        [TestCase(ParallelScope.Default, ParallelScope.Children)]
        [TestCase(ParallelScope.Self, ParallelScope.Children)]
        [TestCase(ParallelScope.None, ParallelScope.Children)]
        [TestCase(ParallelScope.Default, ParallelScope.Fixtures)]
        [TestCase(ParallelScope.Self, ParallelScope.Fixtures)]
        [TestCase(ParallelScope.None, ParallelScope.Fixtures)]
        public void SingleThreadedTestCase(ParallelScope testScope, ParallelScope contextScope)
        {
            _context.IsSingleThreaded = true;
            var strategy = MakeWorkItem(_testMethod, testScope, contextScope).ExecutionStrategy;
            Assert.That(strategy, Is.EqualTo(ParallelExecutionStrategy.Direct));
        }

        [TestCase(ParallelScope.Default, ParallelScope.Default, ParallelExecutionStrategy.NonParallel)]
        [TestCase(ParallelScope.Self, ParallelScope.Default, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Default, ParallelExecutionStrategy.NonParallel)]
        [TestCase(ParallelScope.Default, ParallelScope.Children, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.Self, ParallelScope.Children, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Children, ParallelExecutionStrategy.NonParallel)]
        [TestCase(ParallelScope.Default, ParallelScope.Fixtures, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.Self, ParallelScope.Fixtures, ParallelExecutionStrategy.Parallel)]
        [TestCase(ParallelScope.None, ParallelScope.Fixtures, ParallelExecutionStrategy.NonParallel)]
        public void TestFixtureStrategy(ParallelScope testScope, ParallelScope contextScope, ParallelExecutionStrategy expectedStrategy)
        {
            var strategy = MakeWorkItem(_testFixture, testScope, contextScope).ExecutionStrategy;
            Assert.That(strategy, Is.EqualTo(expectedStrategy));
        }

        private WorkItem MakeWorkItem(ITest test, ParallelScope testScope, ParallelScope contextScope)
        {
            test.Properties.Set(PropertyNames.ParallelScope, testScope);
            _context.ParallelScope = contextScope;

            var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty);
            work.InitializeContext(_context);

            return work;
        }

        private void TestMethod() { }

        private class MyFixture { }
    }
}
#endif
