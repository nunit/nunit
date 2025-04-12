// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    /// <summary>
    /// Summary description for ConstraintExpressionTests.
    /// </summary>
    [TestFixture]
    public class ConstraintExpressionTests
    {
        private static readonly Func<int, int, bool> MyIntComparer = (x, y) => x == y;

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
                .Using((IComparer<string>)Comparer<string>.Default);
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
            var constraint = constraintExpression.Contains(4).Using(MyIntComparer);
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
            var constraint = constraintExpression.Contain(8).Using(MyIntComparer);
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

        [Test]
        public void ConstraintExpressionAnyOfTypeIgnoreWhiteSpace()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.AnyOf(new string[] { "RED", "GREEN" }).IgnoreWhiteSpace;
            Assert.That(" R E D ", constraint);
        }

        [Test]
        public void ConstraintExpressionAnyOfTypeNormalizeLineEndings()
        {
            var constraintExpression = new ConstraintExpression();
            var constraint = constraintExpression.AnyOf(new string[] { "R\rE\nD\r\n", "GREEN" }).NormalizeLineEndings;
            Assert.That("R\nE\r\nD\r", constraint);
        }

        [Test]
        public void ConstraintExpressionAnyOfList()
        {
            var constraintExpression = new ConstraintExpression();
            var expected = new List<string>() { "RED", "GREEN" };
            var constraint = constraintExpression.AnyOf(expected);
            Assert.That("RED", constraint);
        }

        [Test]
        public void ConstraintExpressionAnyOfListMissingItem()
        {
            var constraintExpression = new ConstraintExpression();
            var expected = new List<string>() { "RED", "GREEN" };
            var constraint = constraintExpression.Not.AnyOf(expected);
            Assert.That("BLUE", constraint);
        }
    }
}
