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

#if TASK_PARALLEL_LIBRARY_API
using System;
using NUnit.Framework.Internal;

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
                        ex = Assert.ThrowsAsync<Exception>(asyncUserCode).GetAwaiter().GetResult();
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
                        ex = Assert.CatchAsync(asyncUserCode).GetAwaiter().GetResult();
                    }
                    catch { }
                }

                if (ex != null) ExceptionHelper.Rethrow(ex);
            }

            public override string ToString() => "Assert.CatchAsync(…)";
        }
    }
}
#endif
