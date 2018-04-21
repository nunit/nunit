using NUnit.Framework.Constraints;

namespace NUnit.TestUtilities
{
    internal sealed class DummyConstraint : Constraint
    {
        public static DummyConstraint Instance { get; } = new DummyConstraint();
        private DummyConstraint() { }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return null;
        }
    }
}
