// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    public partial class AsyncExecutionApiAdapter
    {
        private sealed class AssertThrowsAsyncAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                var ex = default(Exception);
                using (new TestExecutionContext.IsolatedContext())
                {
                    try
                    {
                        ex = Assert.ThrowsAsync<Exception>(asyncUserCode);
                    }
                    catch { }
                }

                if (ex is not null) ExceptionHelper.Rethrow(ex);
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
                var ex = default(Exception);
                using (new TestExecutionContext.IsolatedContext())
                {
                    try
                    {
                        ex = Assert.CatchAsync(asyncUserCode);
                    }
                    catch { }
                }

                if (ex is not null) ExceptionHelper.Rethrow(ex);
            }

            public override string ToString() => "Assert.CatchAsync(…)";
        }
    }
}
