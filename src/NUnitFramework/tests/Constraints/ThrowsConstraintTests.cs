// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ThrowsConstraintTest_ExactType : ThrowsConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new ThrowsConstraint(
                new ExceptionTypeConstraint(typeof(ArgumentException)));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "<System.ArgumentException>";
            StringRepresentation = "<throws <typeof System.ArgumentException>>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData =
        {
            new TestDelegate(TestDelegates.ThrowsArgumentException)
        };
        private static readonly object[] FailureData =
        {
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNullReferenceException), "<System.NullReferenceException: my message" + Environment.NewLine),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNothing), "no exception thrown"),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsSystemException), "<System.Exception: my message" + Environment.NewLine)
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    [TestFixture]
    public class ThrowsConstraintTest_InstanceOfType : ThrowsConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new ThrowsConstraint(
                new InstanceOfTypeConstraint(typeof(TestDelegates.BaseException)));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "instance of <NUnit.Framework.Tests.TestUtilities.TestDelegates+BaseException>";
            StringRepresentation = "<throws <instanceof NUnit.Framework.Tests.TestUtilities.TestDelegates+BaseException>>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[]
        {
            new TestDelegate(TestDelegates.ThrowsBaseException),
            new TestDelegate(TestDelegates.ThrowsDerivedException)
        };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsArgumentException), "<System.ArgumentException: myMessage"),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNothing), "no exception thrown"),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNullReferenceException), "<System.NullReferenceException: my message")
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    public class ThrowsConstraintTest_WithConstraint : ThrowsConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new ThrowsConstraint(
                new AndConstraint(
                    new ExceptionTypeConstraint(typeof(ArgumentException)),
                    new PropertyConstraint("ParamName", new EqualConstraint("myParam"))));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = @"<System.ArgumentException> and property ParamName equal to ""myParam""";
            StringRepresentation = @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData =
        {
            new TestDelegate(TestDelegates.ThrowsArgumentException)
        };
        private static readonly object[] FailureData =
        {
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNullReferenceException), "<System.NullReferenceException: my message" + Environment.NewLine),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsNothing), "no exception thrown"),
            new TestCaseData(new TestDelegate(TestDelegates.ThrowsSystemException), "<System.Exception: my message" + Environment.NewLine)
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    public abstract class ThrowsConstraintTestBase : ConstraintTestBaseNoData
    {
        private const string Message = ": Must be implemented in derived class";

        private static object[] SuccessData => throw new NotImplementedException(nameof(SuccessData) + Message);

        private static object[] FailureData => throw new NotImplementedException(nameof(FailureData) + Message);

        [Test, TestCaseSource(nameof(SuccessData))]
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

        [Test, TestCaseSource(nameof(FailureData)), SetUICulture("en-US")]
        public void FailsWithBadValues(object badValue, string message)
        {
            string nl = Environment.NewLine;

            var constraintResult = TheConstraint.ApplyTo(badValue);
            Assert.That(constraintResult.IsSuccess, Is.False);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.StartWith(
                TextMessageWriter.Pfx_Expected + ExpectedDescription + nl +
                TextMessageWriter.Pfx_Actual + message));
        }
    }
}
