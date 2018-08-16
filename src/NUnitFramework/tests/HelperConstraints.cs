// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    public sealed class HelperConstraints
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

            protected override object GetTestObject<TActual>(ActualValueDelegate<TActual> del)
            {
                return del;
            }

            public override ConstraintResult ApplyTo<TActual>(TActual actual)
            {
                var @delegate = actual as Delegate;
                if (@delegate == null)
                    throw new ArgumentException("Actual value must be a delegate.", nameof(actual));

                var invokeMethod = @delegate.GetType().GetTypeInfo().GetMethod("Invoke");
                if (invokeMethod.GetParameters().Length != 0)
                    throw new ArgumentException("Delegate must be parameterless.", nameof(actual));

                var stopwatch = new System.Diagnostics.Stopwatch();

#if ASYNC
                if (AsyncToSyncAdapter.IsAsyncOperation(@delegate))
                {
                    stopwatch.Start();
                    AsyncToSyncAdapter.Await(() => @delegate.DynamicInvoke());
                    stopwatch.Stop();
                }
                else
#endif
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
