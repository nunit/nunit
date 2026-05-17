// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests
{
    public partial class AsyncExecutionApiAdapter
    {
        private abstract class ConstraintApiAdapter : AsyncExecutionApiAdapter
        {
            private readonly Constraint _constraint;

            protected ConstraintApiAdapter(Constraint constraint)
            {
                _constraint = constraint;
            }

            public override void Execute(Func<Task> asyncUserCode)
            {
                _constraint.ApplyTo(asyncUserCode);
#pragma warning disable CS0618 // Type or member is obsolete
                _constraint.ApplyTo((Func<Task>)asyncUserCode.Invoke); // ActualValueDelegate<> overload
#pragma warning restore CS0618 // Type or member is obsolete
            }

            public sealed override string ToString() => _constraint.GetType().Name + ".ApplyTo(…)";
        }

        private sealed class ThrowsNothingConstraintAdapter()
            : ConstraintApiAdapter(new ThrowsNothingConstraint());

        private sealed class ThrowsConstraintAdapter()
            : ConstraintApiAdapter(new ThrowsConstraint(DummyConstraint.Instance));

        private sealed class ThrowsExceptionConstraintAdapter()
            : ConstraintApiAdapter(new ThrowsExceptionConstraint());

        private sealed class NormalConstraintAdapter()
            : ConstraintApiAdapter(DummyConstraint.Instance);
    }
}
