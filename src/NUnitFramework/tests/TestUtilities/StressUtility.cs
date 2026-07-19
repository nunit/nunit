// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities
{
    public static class StressUtility
    {
        public static void RunParallel(Action action, int times, int maxParallelism, bool useThreadPool = true)
        {
            ArgumentNullException.ThrowIfNull(action);
            ArgumentOutOfRangeException.ThrowIfNegative(times);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxParallelism);

            if (times == 0)
                return;

            maxParallelism = Math.Min(times, maxParallelism);
            var exception = default(Exception);

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
                            Volatile.Write(ref exception, ex);
                            Volatile.Write(ref times, 0);
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
                        MethodInfo? actionMethod = action.GetMethodInfo();

                        new Thread(work.Invoke)
                        {
                            Name = $"{nameof(StressUtility)}.{nameof(RunParallel)} ({actionMethod?.Name ?? "anonymous"}) dedicated thread {maxParallelism + 1}"
                        }.Start();
                    }
                }

                noMoreThreadsEvent.Wait();

                Exception? readException = Volatile.Read(ref exception);
                if (readException is not null)
                {
                    ExceptionHelper.Rethrow(readException);
                }
            }
        }
    }
}
