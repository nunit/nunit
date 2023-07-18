// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal.Execution
{
    public class ParallelExecutionStrategyTests
    {
        private TestExecutionContext? _context;
        private TestMethod? _testMethod;
        private TestFixture? _testFixture;

        [SetUp]
        public void SetUp()
        {
            _context = new TestExecutionContext();
            _testMethod = Fakes.GetTestMethod(GetType(), "TestMethod");
            _testFixture = new TestFixture(new TypeWrapper(typeof(MyFixture)));
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

        private WorkItem MakeWorkItem(Test test, ParallelScope testScope, ParallelScope contextScope)
        {
            test.Properties.Set(PropertyNames.ParallelScope, testScope);
            _context.ParallelScope = contextScope;

            return TestBuilder.CreateWorkItem(test, _context);
        }

        private void TestMethod() { }

        private class MyFixture { }
    }
}
