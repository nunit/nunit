// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    partial class AsyncExecutionApiAdapter
    {
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
    }
}
