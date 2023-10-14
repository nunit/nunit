// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ThrowsExceptionConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new ThrowsExceptionConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "an exception to be thrown";
            StringRepresentation = "<throwsexception>";
        }

        [Test]
        public void SucceedsWithNonVoidReturningFunction()
        {
            var constraintResult = TheConstraint.ApplyTo(TestDelegates.ThrowsInsteadOfReturns);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[]
        {
            new TestDelegate(TestDelegates.ThrowsArgumentException)
        };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNothing), "no exception thrown"),
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public static void CatchesAsyncException()
        {
            Assert.That(async () => await AsyncTestDelegates.ThrowsArgumentExceptionAsync(), Throws.Exception);
        }

        [Test]
        public static void CatchesAsyncTaskOfTException()
        {
            Assert.That<Task<int>>(async () =>
            {
                await AsyncTestDelegates.Delay(5);
                throw new Exception();
            }, Throws.Exception);
        }

        [Test]
        public static void CatchesSyncException()
        {
            Assert.That(() => AsyncTestDelegates.ThrowsArgumentException(), Throws.Exception);
        }

        [Test]
        public static void CatchesSyncTaskOfTException()
        {
            Assert.That<Task<int>>(() => throw new Exception(), Throws.Exception);
        }
    }
}
