// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if ASYNC
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework
{
    /// <summary>
    /// Adapts each NUnit API that executes async user code synchronously to a common interface
    /// to be tested for behavior.
    /// </summary>
    public abstract class AsyncExecutionApiAdapter
    {
        public static IEnumerable<AsyncExecutionApiAdapter> All { get; } = new AsyncExecutionApiAdapter[]
        {
            new TaskReturningTestMethodAdapter(),
            new AssertThrowsAsyncAdapter(),
            new AssertDoesNotThrowAsyncAdapter(),
            new AssertCatchAsyncAdapter(),
            new ThrowsNothingConstraintAdapter(),
            new ThrowsConstraintAdapter(),
            new ThrowsExceptionConstraintAdapter(),
            new NormalConstraintAdapter()
        };

        public abstract void Execute(AsyncTestDelegate asyncUserCode);
        public abstract override string ToString();

        private sealed class TaskReturningTestMethodAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                TestBuilder
                    .RunTestCase(new Fixture(asyncUserCode), nameof(Fixture.TestMethod))
                    .AssertPassed();
            }

            private sealed class Fixture
            {
                private readonly AsyncTestDelegate _asyncUserCode;

                public Fixture(AsyncTestDelegate asyncUserCode)
                {
                    _asyncUserCode = asyncUserCode;
                }

                public Task TestMethod() => _asyncUserCode.Invoke();
            }

            public override string ToString() => "[Test] Task TestMethod() { … }";
        }

        private sealed class AssertThrowsAsyncAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                var ex = (Exception)null;
                using (new TestExecutionContext.IsolatedContext())
                {
                    try
                    {
                        ex = Assert.ThrowsAsync<Exception>(asyncUserCode);
                    }
                    catch { }
                }

                if (ex != null) ExceptionHelper.Rethrow(ex);
            }

            public override string ToString() => "Assert.ThrowsAsync(…)";
        }

        private sealed class AssertDoesNotThrowAsyncAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                Assert.DoesNotThrowAsync(asyncUserCode);
            }

            public override string ToString() => "Assert.DoesNotThrowAsync(…)";
        }

        private sealed class AssertCatchAsyncAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                var ex = (Exception)null;
                using (new TestExecutionContext.IsolatedContext())
                {
                    try
                    {
                        ex = Assert.CatchAsync(asyncUserCode);
                    }
                    catch { }
                }

                if (ex != null) ExceptionHelper.Rethrow(ex);
            }

            public override string ToString() => "Assert.CatchAsync(…)";
        }

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
#endif
