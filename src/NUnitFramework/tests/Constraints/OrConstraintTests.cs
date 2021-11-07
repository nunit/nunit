// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class OrConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new OrConstraint(new EqualConstraint(42), new EqualConstraint(99));
            ExpectedDescription = "42 or 99";
            StringRepresentation = "<or <equal 42> <equal 99>>";
        }

        static object[] SuccessData = new object[] { 99, 42 };

        static object[] FailureData = new object[] { new object[] { 37, "37" } };

        [Test]
        public void CanCombineTestsWithOrOperator()
        {
            Assert.That(99, new EqualConstraint(42) | new EqualConstraint(99) );
        }
    }
}
