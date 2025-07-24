// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal.Execution
{
    [TestFixture]
    public sealed class ExplicitExecutionTest
    {
        [Test]
        public void ByDefaultOnlyNonExplicitTestsAreRun()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExplicitTests));
            workItem.Execute();

            TestResult result = workItem.Result;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.PassCount, Is.EqualTo(1));
                Assert.That(result.SkipCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void ExplicitTestsCanBeEnabled()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExplicitTests), TestFilter.Explicit);
            workItem.Execute();

            TestResult result = workItem.Result;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.PassCount, Is.EqualTo(2));
            }
        }
    }
}
