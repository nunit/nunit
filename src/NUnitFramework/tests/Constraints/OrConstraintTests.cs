// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class OrConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new OrConstraint(new EqualConstraint(42), new EqualConstraint(99));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "42 or 99";
            StringRepresentation = "<or <equal 42> <equal 99>>";
        }

        private static readonly object[] SuccessData = new object[] { 99, 42 };
        private static readonly object[] FailureData = new object[] { new object[] { 37, "37" } };

        [Test]
        public void CanCombineTestsWithOrOperator()
        {
            Assert.That(99, new EqualConstraint(42) | new EqualConstraint(99));
        }
    }
}
