// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
    }
}