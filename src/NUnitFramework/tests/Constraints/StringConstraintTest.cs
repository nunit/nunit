// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Constraints
{

    public abstract class StringConstraintTests : ConstraintTestBase
    {
        [Test]
        public void NonStringDataThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(123));
        }
    }
}
