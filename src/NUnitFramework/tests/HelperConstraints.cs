// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    public static class HelperConstraints
    {
        public static Constraint HasMaxTime(int milliseconds)
        {
            return new HasMaxTimeConstraint(milliseconds);
        }

        private sealed class HasMaxTimeConstraint : Constraint
        {
            private readonly int _milliseconds;

            public HasMaxTimeConstraint(int milliseconds)
            {
                _milliseconds = milliseconds;
            }

            public override string Description => "Has Maximum Time";

            protected override object? GetTestObject<TActual>(ActualValueDelegate<TActual> del)
            {
                return del;
            }

            public override ConstraintResult ApplyTo<TActual>(TActual actual)
            {
                var @delegate = ConstraintUtils.RequireActual<Delegate>(actual, nameof(actual));

                var invokeMethod = @delegate.GetType().GetMethod("Invoke")!;
                if (invokeMethod.GetParameters().Length != 0)
                    throw new ArgumentException("Delegate must be parameterless.", nameof(actual));

                var stopwatch = new System.Diagnostics.Stopwatch();

                if (AsyncToSyncAdapter.IsAsyncOperation(@delegate))
                {
                    stopwatch.Start();
                    AsyncToSyncAdapter.Await(TestExecutionContext.CurrentContext, () => @delegate.DynamicInvoke());
                    stopwatch.Stop();
                }
                else
                {
                    stopwatch.Start();
                    @delegate.DynamicInvoke();
                    stopwatch.Stop();
                }

                return new Result(this, stopwatch.ElapsedMilliseconds);
            }

            private sealed class Result : ConstraintResult
            {
                private readonly HasMaxTimeConstraint _constraint;

                public Result(HasMaxTimeConstraint constraint, long actualMilliseconds)
                    : base(constraint, actualMilliseconds, actualMilliseconds <= constraint._milliseconds)
                {
                    _constraint = constraint;
                }

                public override void WriteMessageTo(MessageWriter writer)
                {
                    writer.Write($"Elapsed time of {ActualValue}ms exceeds maximum of {_constraint._milliseconds}ms");
                }
            }
        }
    }
}
