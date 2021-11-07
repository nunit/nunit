// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Summary description for ConstraintExpressionTests.
    /// </summary>
    [TestFixture]
    public class ConstraintExpressionTests
    {
        static readonly Func<int, int, bool> myIntComparer = (x, y) => x == y;

        [Test]
        public void ConstraintExpressionMemberAnd()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.Member(3).And.Contains(7);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ConstraintExpressionMemberUsing()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression
                .Member("3")
                .Using<int, string>((i, s) => i.ToString() == s)
                .Using((IComparer<string>) Comparer<string>.Default);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ConstraintExpressionContainsAnd()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.Contains(4).And.Member(2);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ConstraintExpressionContainsUsing()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.Contains(4).Using(myIntComparer);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ConstraintExpressionContainAnd()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.Contain(8).And.Contain(7);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ConstraintExpressionContainUsing()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.Contain(8).Using(myIntComparer);
            var collection1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ContainsConstraintApplyTo()
        {
            var containsConstraint = new ContainsConstraint(6);
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = containsConstraint.ApplyTo(collection1);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void ConstraintExpressionAnyOfType()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.AnyOf(new string[] { "RED", "GREEN" }).IgnoreCase;
            Assert.That("red", constraint);
        }
    }
}
