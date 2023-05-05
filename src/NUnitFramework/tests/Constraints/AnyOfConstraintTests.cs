// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AnyOfConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new AnyOfConstraint(new object[] { 1, 2, 3 });

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "any of < 1, 2, 3 >";
            StringRepresentation = "<anyof 1 2 3>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 1, 2, 3 };
        private static readonly object[] FailureData = new object[] { new object[] { 4, "4" }, new object[] { "A", "\"A\"" } };
#pragma warning restore IDE0052 // Remove unread private members

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

        [Test]
        public void ValidMemberInParams()
        {
            Assert.That(42, Is.AnyOf(0, -1, 42, 100));
        }

        [Test]
        public void ValidMemberInArray()
        {
            var array = new[] { 0, -1, 42, 100 };
            Assert.That(42, Is.AnyOf(array));
        }

        [Test]
        public void ValidMemberInList()
        {
            var list = new List<int>() { 0, -1, 42, 100 };
            Assert.That(42, Is.AnyOf(list));
        }

        [Test]
        public void MissingMember()
        {
            Assert.That(42, Is.Not.AnyOf(0, -1, 100));
        }
    }
}
