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
using System.Threading.Tasks;

namespace NUnit.Framework
{
    public static partial class AsyncAssert
    {
        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">A TestDelegate to be executed in Multiple Assertion mode.</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task Multiple(AsyncTestDelegate testDelegate)
        {
            Guard.ArgumentNotNull(testDelegate, nameof(TestDelegate));

            var helper = AssertMultipleHelper.Start();

            return testDelegate.Invoke().ContinueWith(task =>
            {
                helper.Finally();
                task.ThrowAwaitExceptionOnFailure();
                helper.OnSuccess();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task Multiple(AsyncTestDelegate testDelegate)
        {
            Guard.ArgumentNotNull(testDelegate, nameof(TestDelegate));

            var helper = AssertMultipleHelper.Start();
            try
            {
                await testDelegate.Invoke().ConfigureAwait(true);
            }
            finally
            {
                helper.Finally();
            }

            helper.OnSuccess();
        }
#endif
    }
}
#endif
