// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    partial class AsyncExecutionApiAdapter
    {
        private abstract class ConstraintApiAdapter : AsyncExecutionApiAdapter
        {
            private readonly Constraint _constraint;

            protected ConstraintApiAdapter(Constraint constraint)
            {
                _constraint = constraint;
            }

            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                _constraint.ApplyTo(asyncUserCode);
                _constraint.ApplyTo(asyncUserCode.Invoke); // ActualValueDelegate<> overload
            }

            public sealed override string ToString() => _constraint.GetType().Name + ".ApplyTo(…)";
        }

        private sealed class ThrowsNothingConstraintAdapter : ConstraintApiAdapter
        {
            public ThrowsNothingConstraintAdapter() : base(new ThrowsNothingConstraint())
            {
            }
        }

        private sealed class ThrowsConstraintAdapter : ConstraintApiAdapter
        {
            public ThrowsConstraintAdapter() : base(new ThrowsConstraint(DummyConstraint.Instance))
            {
            }
        }

        private sealed class ThrowsExceptionConstraintAdapter : ConstraintApiAdapter
        {
            public ThrowsExceptionConstraintAdapter() : base(new ThrowsExceptionConstraint())
            {
            }
        }

        private sealed class NormalConstraintAdapter : ConstraintApiAdapter
        {
            public NormalConstraintAdapter() : base(DummyConstraint.Instance)
            {
            }
        }
    }
}
