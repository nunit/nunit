// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AnyOfConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new AnyOfConstraint(new object[] { 1, 2, 3 });
            ExpectedDescription = "any of < 1, 2, 3 >";
            StringRepresentation = "<anyof 1 2 3>";
        }

        private static object[] SuccessData = new object[] { 1, 2, 3 };
        private static object[] FailureData = new object[] { new object[] { 4, "4" }, new object[] { "A", "\"A\"" } };

        [Test]
        public void ItemIsPresent_IgnoreCase()
        {
            var anyOf = new AnyOfConstraint(new[] { "a", "B", "ab" }).IgnoreCase;
            Assert.That(anyOf.ApplyTo("AB").Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [Test]
        public void ItemIsPresent_WithEqualityComparer()
        {
            Func<string, string, bool> comparer = (expected, actual) => actual.Contains(expected);
            var anyOf = new AnyOfConstraint(new[] { "A", "B", "C" }).Using(comparer);
            Assert.That(anyOf.ApplyTo("1. A").Status, Is.EqualTo(ConstraintStatus.Success));
        }
    }
}
