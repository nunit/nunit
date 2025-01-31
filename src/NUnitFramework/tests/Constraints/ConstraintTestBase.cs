// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    public abstract class ConstraintTestBaseNoData
    {
        protected string ExpectedDescription = "<NOT SET>";
        protected string StringRepresentation = "<NOT SET>";

        protected abstract Constraint TheConstraint { get; }

        [Test]
        public void ProvidesProperDescription()
        {
            Assert.That(TheConstraint.Description, Is.EqualTo(ExpectedDescription));
        }

        [Test]
        public void ProvidesProperStringRepresentation()
        {
            Assert.That(TheConstraint.ToString(), Is.EqualTo(StringRepresentation));
        }
    }

    public abstract class ConstraintTestBase : ConstraintTestBaseNoData
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

        [Test, TestCaseSource(nameof(FailureData))]
        public void FailsWithBadValues(object badValue, string message)
        {
            var constraintResult = TheConstraint.ApplyTo(badValue);
            Assert.That(constraintResult.IsSuccess, Is.False);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.Contain(ExpectedDescription).And.Contains(message));
        }
    }
}
