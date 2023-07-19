// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NotConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new NotConstraint(new EqualConstraint(null));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "not equal to null";
            StringRepresentation = "<not <equal null>>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 42, "Hello" };
        private static readonly object?[] FailureData = new object?[] { new object?[] { null, "null" } };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void NotHonorsIgnoreCaseUsingConstructors()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That("abc", new NotConstraint(new EqualConstraint("ABC").IgnoreCase)));
            Assert.That(ex?.Message, Does.Contain("ignoring case"));
        }

        [Test]
        public void NotHonorsIgnoreCaseUsingPrefixNotation()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That("abc", Is.Not.EqualTo("ABC").IgnoreCase));
            Assert.That(ex?.Message, Does.Contain("ignoring case"));
        }

        [Test]
        public void NotHonorsTolerance()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(4.99d, Is.Not.EqualTo(5.0d).Within(.05d)));
            Assert.That(ex?.Message, Does.Contain("+/-"));
        }

        [Test]
        public void CanUseNotOperator()
        {
            Assert.That(42, !new EqualConstraint(99));
        }
    }
}
