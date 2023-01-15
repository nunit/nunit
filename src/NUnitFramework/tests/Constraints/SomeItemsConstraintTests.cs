// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    class SomeItemsConstraintTests
    {
        [Test]
        public void EqualConstraintUsingDoesNotThrow()
        {
            var constraint = new SomeItemsConstraint(new EqualConstraint("42"));
            Assert.DoesNotThrow(() => constraint.Using((IComparer<string>)Comparer<string>.Default));
        }

        [Test]
        public void AnotherConstraintUsingThrows()
        {
            var constraint = new SomeItemsConstraint(new EmptyCollectionConstraint());
            Assert.Throws<ArgumentException>(() => constraint.Using((IComparer<string>)Comparer<string>.Default));
        }

        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new NoItemConstraint(Is.Null));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }
    }
}
