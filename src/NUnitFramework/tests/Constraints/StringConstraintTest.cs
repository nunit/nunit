// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    public abstract class StringConstraintTests : ConstraintTestBase
    {
        protected StringConstraint StringConstraint => (StringConstraint)TheConstraint;

        [Test]
        public void NonStringData_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => StringConstraint.ApplyTo(123));
        }

        [Test]
        public void MultipleUsingModifiers_ThrowsException()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => StringConstraint.Using(StringComparison.Ordinal).Using(CultureInfo.InvariantCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => StringConstraint.Using(CultureInfo.InvariantCulture).Using(StringComparison.Ordinal),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => StringConstraint.Using(StringComparison.Ordinal).Using(StringComparison.OrdinalIgnoreCase),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => StringConstraint.Using(CultureInfo.InvariantCulture).Using(CultureInfo.CurrentCulture),
                    Throws.TypeOf<InvalidOperationException>());
            });
        }
    }
}
