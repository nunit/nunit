// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    public abstract class ConstraintTestBaseNoData<T>
    {
        protected string ExpectedDescription = "<NOT SET>";
        protected string StringRepresentation = "<NOT SET>";

        protected abstract Constraint<T> TheConstraint { get; }

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

    public abstract class ConstraintTestBase<T> : ConstraintTestBaseNoData<T>
    {
        private const string Message = ": Must be implemented in derived class";

        private static T[] SuccessData => throw new NotImplementedException(nameof(SuccessData) + Message);

        private static (T, string)[] FailureData => throw new NotImplementedException(nameof(FailureData) + Message);

        [TestCaseSource(nameof(SuccessData))]
        public void SucceedsWithGoodValues(T value)
        {
            var constraintResult = TheConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [TestCaseSource(nameof(FailureData))]
        public void FailsWithBadValues((T BadValue, string Message) data)
        {
            var constraintResult = TheConstraint.ApplyTo(data.BadValue);
            Assert.That(constraintResult.IsSuccess, Is.False);

            MessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.Contain(ExpectedDescription).And.Contains(data.Message));
        }
    }
}
