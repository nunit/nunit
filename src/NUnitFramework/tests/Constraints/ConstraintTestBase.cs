// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public abstract class ConstraintTestBaseNoData
    {
        protected Constraint TheConstraint;
        protected string ExpectedDescription = "<NOT SET>";
        protected string StringRepresentation = "<NOT SET>";

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

        static object[] SuccessData => throw new NotImplementedException(nameof(SuccessData) + Message);

        static object[] FailureData => throw new NotImplementedException(nameof(FailureData) + Message);

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
            string NL = Environment.NewLine;

            var constraintResult = TheConstraint.ApplyTo(badValue);
            Assert.That(constraintResult.IsSuccess, Is.False);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That( writer.ToString(), Is.EqualTo(
                TextMessageWriter.Pfx_Expected + ExpectedDescription + NL +
                TextMessageWriter.Pfx_Actual + message + NL ));
        }
    }
}
