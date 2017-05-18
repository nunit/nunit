// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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
            _testFixture = new TestFixture(new TypeWrapper(typeof(MyFixture)));
        }

        [TestCase(ParallelScope.Default, ParallelScope.Default, "Direct")]
        [TestCase(ParallelScope.Self, ParallelScope.Default, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Default, "NonParallel")]
        [TestCase(ParallelScope.Default, ParallelScope.Children, "Parallel")]
        [TestCase(ParallelScope.Self, ParallelScope.Children, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Children, "NonParallel")]
        [TestCase(ParallelScope.Default, ParallelScope.Fixtures, "Direct")]
        [TestCase(ParallelScope.Self, ParallelScope.Fixtures, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Fixtures, "NonParallel")]
        public void ParallelExecutionStrategy_TestCase(ParallelScope testScope, ParallelScope contextScope, string expectedStrategy)
        {
            _testMethod.Properties.Set(PropertyNames.ParallelScope, testScope);
            _context.ParallelScope = contextScope;

            WorkItem work = WorkItemBuilder.CreateWorkItem(_testMethod, TestFilter.Empty);
            work.InitializeContext(_context);

            // We use a string for expected because the ExecutionStrategy enum is internal and can't be an arg to a public method
            Assert.That(ParallelWorkItemDispatcher.GetExecutionStrategy(work).ToString(), Is.EqualTo(expectedStrategy));
            
            // Make context single threaded - should always be direct
            _context.IsSingleThreaded = true;
            Assert.That(ParallelWorkItemDispatcher.GetExecutionStrategy(work).ToString(), Is.EqualTo("Direct"));
        }

        [TestCase(ParallelScope.Default, ParallelScope.Default, "Direct")]
        [TestCase(ParallelScope.Self, ParallelScope.Default, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Default, "NonParallel")]
        [TestCase(ParallelScope.Default, ParallelScope.Children, "Parallel")]
        [TestCase(ParallelScope.Self, ParallelScope.Children, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Children, "NonParallel")]
        [TestCase(ParallelScope.Default, ParallelScope.Fixtures, "Parallel")]
        [TestCase(ParallelScope.Self, ParallelScope.Fixtures, "Parallel")]
        [TestCase(ParallelScope.None, ParallelScope.Fixtures, "NonParallel")]
        public void ParallelExeutionStrategy_TestFixture(ParallelScope testScope, ParallelScope contextScope, string expectedStrategy)
        {
            _testFixture.Properties.Set(PropertyNames.ParallelScope, testScope);
            _context.ParallelScope = contextScope;

            WorkItem work = WorkItemBuilder.CreateWorkItem(_testFixture, TestFilter.Empty);
            work.InitializeContext(_context);

            // We use a string for expected because the ExecutionStrategy enum is internal and can't be an arg to a public method
            Assert.That(ParallelWorkItemDispatcher.GetExecutionStrategy(work).ToString(), Is.EqualTo(expectedStrategy));
        }

        private void TestMethod() { }

        private class MyFixture { }
    }
}
#endif