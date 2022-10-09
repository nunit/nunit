// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ThrowsExceptionConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsExceptionConstraint();
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

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
        };

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
            Assert.That<Task<int>>(() =>
            {
                throw new Exception();
            }, Throws.Exception);
        }
    }
}
