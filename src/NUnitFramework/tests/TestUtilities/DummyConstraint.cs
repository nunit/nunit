// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.TestUtilities
{
    internal sealed class DummyConstraint : Constraint
    {
        public static DummyConstraint Instance { get; } = new DummyConstraint();

        private DummyConstraint() { }

        public override string Description => throw new NotImplementedException();


        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return null;
        }
    }
}
