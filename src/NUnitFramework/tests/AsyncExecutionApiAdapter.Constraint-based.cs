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
                _constraint.ApplyTo((ActualValueDelegate<Task>)asyncUserCode.Invoke); // ActualValueDelegate<> overload
#pragma warning restore CS0618 // Type or member is obsolete
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
