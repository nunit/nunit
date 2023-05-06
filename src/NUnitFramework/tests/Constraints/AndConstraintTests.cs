// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AndConstraintTests : ConstraintTestBase
    {
        private TextMessageWriter _messageWriter;

        protected override Constraint TheConstraint { get; } = new AndConstraint(new GreaterThanConstraint(40), new LessThanConstraint(50));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "greater than 40 and less than 50";
            StringRepresentation = "<and <greaterthan 40> <lessthan 50>>";
            _messageWriter = new TextMessageWriter();
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 42 };
        private static readonly object[] FailureData = new object[] { new object[] { 37, "37" }, new object[] { 53, "53" } };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void CanCombineTestsWithAndOperator()
        {
            Assert.That(42, new GreaterThanConstraint(40) & new LessThanConstraint(50));
        }

        [Test]
        public void HandlesFirstPartFailing()
        {
            const string test = "Couldn't load c:\\myfile.txt";

            IResolveConstraint expression = Does.StartWith("Could not load").And.Contains("c:\\myfile.txt");
            var constraint = expression.Resolve();
            var constraintResult = constraint.ApplyTo(test);

            Assert.That(constraintResult.IsSuccess, Is.False);
            Assert.That(constraintResult.Description, Is.EqualTo("String starting with \"Could not load\" and containing \"c:\\myfile.txt\""));
        }

        [Test]
        public void ShouldIncludeAdditionalInformationFromFailedConstraint_Right()
        {
            var constraint = new AndConstraint(Is.Ordered, Is.EquivalentTo(new[] { 1, 2, 3 }));

            string expectedMsg =
                "  Expected: collection ordered and equivalent to < 1, 2, 3 >" + Environment.NewLine +
                "  But was:  < 1, 2 >" + Environment.NewLine +
                "  Missing (1): < 3 >" + Environment.NewLine;

            var constraintResult = constraint.ApplyTo(new[] { 1, 2 });

            Assert.That(constraintResult.IsSuccess, Is.False);

            constraintResult.WriteMessageTo(_messageWriter);
            Assert.That(_messageWriter.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void ShouldIncludeAdditionalInformationFromFailedConstraint_Left()
        {
            var constraint = new AndConstraint(Is.EquivalentTo(new[] { 1, 2, 3 }), Is.Ordered);

            string expectedMsg =
                "  Expected: equivalent to < 1, 2, 3 > and collection ordered" + Environment.NewLine +
                "  But was:  < 1, 2 >" + Environment.NewLine +
                "  Missing (1): < 3 >" + Environment.NewLine;

            var constraintResult = constraint.ApplyTo(new[] { 1, 2 });

            Assert.That(constraintResult.IsSuccess, Is.False);

            constraintResult.WriteMessageTo(_messageWriter);
            Assert.That(_messageWriter.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void ShouldIncludeAdditionalInformationFromFailedConstraint_Both()
        {
            var constraint = new AndConstraint(Is.EquivalentTo(new[] { 1, 2, 3 }), Is.Ordered);

            string expectedMsg =
                "  Expected: equivalent to < 1, 2, 3 > and collection ordered" + Environment.NewLine +
                "  But was:  < 2, 1 >" + Environment.NewLine +
                "  Missing (1): < 3 >" + Environment.NewLine;

            var constraintResult = constraint.ApplyTo(new[] { 2, 1 });

            Assert.That(constraintResult.IsSuccess, Is.False);

            constraintResult.WriteMessageTo(_messageWriter);
            Assert.That(_messageWriter.ToString(), Is.EqualTo(expectedMsg));
        }
    }
}
