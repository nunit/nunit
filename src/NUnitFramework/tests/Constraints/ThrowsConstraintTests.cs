// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ThrowsConstraintTest_ExactType : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new ExceptionTypeConstraint(typeof(ArgumentException)));
            ExpectedDescription = "<System.ArgumentException>";
            StringRepresentation = "<throws <typeof System.ArgumentException>>";
        }

        static readonly object[] SuccessData =
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static readonly object[] FailureData =
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Environment.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Environment.NewLine )
        };
    }

    [TestFixture]
    public class ThrowsConstraintTest_InstanceOfType : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new InstanceOfTypeConstraint(typeof(TestDelegates.BaseException)));
            ExpectedDescription = "instance of <NUnit.TestUtilities.TestDelegates+BaseException>";
            StringRepresentation = "<throws <instanceof NUnit.TestUtilities.TestDelegates+BaseException>>";
        }

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsBaseException ),
            new TestDelegate( TestDelegates.ThrowsDerivedException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsArgumentException ), "<System.ArgumentException: myMessage" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" )
        };
    }

    public class ThrowsConstraintTest_WithConstraint : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new AndConstraint(
                    new ExceptionTypeConstraint(typeof(ArgumentException)),
                    new PropertyConstraint("ParamName", new EqualConstraint("myParam"))));
            ExpectedDescription = @"<System.ArgumentException> and property ParamName equal to ""myParam""";
            StringRepresentation = @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>";
        }

        static readonly object[] SuccessData =
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static readonly object[] FailureData =
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Environment.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Environment.NewLine )
        };
    }

    public abstract class ThrowsConstraintTestBase : ConstraintTestBaseNoData
    {
        [Test, TestCaseSource("SuccessData")]
        public void SucceedsWithGoodValues(object value)
        {
            var constraintResult = TheConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [Test, TestCaseSource("FailureData"), SetUICulture("en-US")]
        public void FailsWithBadValues(object badValue, string message)
        {
            string NL = Environment.NewLine;

            var constraintResult = TheConstraint.ApplyTo(badValue);
            Assert.IsFalse(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.StartWith(
                TextMessageWriter.Pfx_Expected + ExpectedDescription + NL +
                TextMessageWriter.Pfx_Actual + message));
        }
    }
}
