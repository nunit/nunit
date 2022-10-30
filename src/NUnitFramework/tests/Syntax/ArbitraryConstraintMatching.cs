// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class ArbitraryConstraintMatching
    {
        readonly Constraint custom = new CustomConstraint();
        readonly Constraint another = new AnotherConstraint();

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
            public List<int> Items { get; }
            public Unit()
            {
                Items = new List<int>(new[] { 1, 2, 3, 4, 5 });
            }
        }
    }
}
