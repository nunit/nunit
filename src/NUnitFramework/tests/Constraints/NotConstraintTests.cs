// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class NotConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new NotConstraint( new EqualConstraint(null) );
            ExpectedDescription = "not equal to null";
            StringRepresentation = "<not <equal null>>";
        }

        static object[] SuccessData = new object[] { 42, "Hello" };

        static object[] FailureData = new object [] { new object[] { null, "null" } };

        [Test]
        public void NotHonorsIgnoreCaseUsingConstructors()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That("abc", new NotConstraint(new EqualConstraint("ABC").IgnoreCase)));
            Assert.That(ex.Message, Does.Contain("ignoring case"));
        }

        [Test]
        public void NotHonorsIgnoreCaseUsingPrefixNotation()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That( "abc", Is.Not.EqualTo( "ABC" ).IgnoreCase ));
            Assert.That(ex.Message, Does.Contain("ignoring case"));
        }

        [Test]
        public void NotHonorsTolerance()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That( 4.99d, Is.Not.EqualTo( 5.0d ).Within( .05d ) ));
            Assert.That(ex.Message, Does.Contain("+/-"));
        }

        [Test]
        public void CanUseNotOperator()
        {
            Assert.That(42, !new EqualConstraint(99));
        }
    }
}
