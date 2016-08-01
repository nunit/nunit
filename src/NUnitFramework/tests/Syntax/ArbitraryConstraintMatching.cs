// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class ArbitraryConstraintMatching
    {
        Constraint custom = new CustomConstraint();
        Constraint another = new AnotherConstraint();

        [Test]
        public void CanMatchCustomConstraint()
        {
            IResolveConstraint constraint = new ConstraintExpression().Matches(custom);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<custom>"));
        }

        [Test]
        public void CanMatchCustomConstraintAfterPrefix()
        {
            IResolveConstraint constraint = Is.All.Matches(custom);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<all <custom>>"));
        }

        [Test]
        public void CanMatchCustomConstraintsUnderAndOperator()
        {
            IResolveConstraint constraint = Is.All.Matches(custom).And.Matches(another);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<all <and <custom> <another>>>")); 
        }

        [Test]
        public void CanMatchPredicate()
        {
            IResolveConstraint constraint = new ConstraintExpression().Matches(new Predicate<int>(IsEven));
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<predicate>"));
            Assert.That(42, constraint);
        }

        bool IsEven(int num)
        {
            return (num & 1) == 0;
        }

        [Test]
        public void CanMatchLambda()
        {
            IResolveConstraint constraint = new ConstraintExpression().Matches<int>( (x) => (x & 1) == 0);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<predicate>"));
            Assert.That(42, constraint);
        }

        class CustomConstraint : Constraint
        {
            public override ConstraintResult ApplyTo<TActual>(TActual actual)
            {
                throw new NotImplementedException();
            }
        }

        class AnotherConstraint : CustomConstraint
        {
        }

        [Test]
        public void ApplyMatchesToProperty()
        {
            var unit = new Unit();

            // All forms should pass
            Assert.That(unit, Has.Property("Items").With.Property("Count").EqualTo(5));
            Assert.That(unit, Has.Property("Items").With.Count.EqualTo(5));
            Assert.That(unit, Has.Property("Items").Property("Count").EqualTo(5));
            Assert.That(unit, Has.Property("Items").Count.EqualTo(5));

            // This is the one the bug refers to
            Assert.That(unit, Has.Property("Items").Matches(Has.Count.EqualTo(5)));
        }

        private class Unit
        {
            public List<int> Items { get; private set; }
            public Unit()
            {
                Items = new List<int>(new int[] { 1, 2, 3, 4, 5 });
            }
        }
    }
}
