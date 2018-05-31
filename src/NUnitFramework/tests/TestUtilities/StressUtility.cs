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
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    public static class StressUtility
    {
        public static void RunParallel(Action action, int times, int maxParallelism, bool useThreadPool = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (times < 0) throw new ArgumentOutOfRangeException(nameof(times), times, "Number of times to run must be greater than or equal to 0.");
            if (maxParallelism < 1) throw new ArgumentOutOfRangeException(nameof(maxParallelism), maxParallelism, "Max parallelism must be greater than or equal to 1.");

            if (times == 0) return;

            maxParallelism = Math.Min(times, maxParallelism);
            var exception = (Exception)null;

            var threadsToWaitFor = maxParallelism;
            using (var noMoreThreadsEvent = new ManualResetEventSlim())
            {
                for (var i = 0; i < maxParallelism; i++)
                {
                    var work = new Action(() =>
                    {
                        try
                        {
                            while (Interlocked.Decrement(ref times) >= 0)
                            {
                                action.Invoke();
                            }
                        }
                        catch (Exception ex)
                        {
#if NET20 || NET35 || NET40
                            exception = ex;
                            times = 0;
                            Thread.MemoryBarrier();
#else
                            Volatile.Write(ref exception, ex);
                            Volatile.Write(ref times, 0);
#endif
                        }
                        finally
                        {
                            if (Interlocked.Decrement(ref threadsToWaitFor) == 0)
                                noMoreThreadsEvent.Set();
                        }
                    });

                    if (useThreadPool)
                    {
                        ThreadPool.QueueUserWorkItem(_ => work.Invoke());
                    }
                    else
                    {
                        var actionMethod = action.GetMethodInfo();

                        new Thread(work.Invoke)
                        {
                            Name = $"{nameof(StressUtility)}.{nameof(RunParallel)} ({actionMethod.Name}) dedicated thread {maxParallelism + 1}"
                        }.Start();
                    }
                }

                noMoreThreadsEvent.Wait();

#if NET20 || NET35 || NET40
                Thread.MemoryBarrier();
                if (exception != null)
#else
                if (Volatile.Read(ref exception) != null)
#endif
                {
                    ExceptionHelper.Rethrow(exception);
                }
            }
        }
    }
}
