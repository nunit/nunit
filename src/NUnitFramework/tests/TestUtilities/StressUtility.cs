using System;
using System.Threading;
using NUnit.Framework.Internal;

#if NETSTANDARD1_3 || NETSTANDARD1_6
using System.Threading.Tasks;
#endif

#if NETSTANDARD1_6
using System.Reflection;
#endif

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
            using (var noMoreThreadsEvent = new ManualResetEvent(false))
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
#if NET_2_0 || NET_3_5 || NET_4_0
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
#if NETSTANDARD1_3 || NETSTANDARD1_6
                        Task.Run(work);
#else
                        ThreadPool.QueueUserWorkItem(_ => work.Invoke());
#endif
                    }
                    else
                    {
#if NETSTANDARD1_3
                        throw new PlatformNotSupportedException(".NET Standard 1.3 does not have access to System.Threading.Thread, so useThreadPool must be true.");
#else
#if NETSTANDARD1_6
                        var actionMethod = action.GetMethodInfo();
#else
                        var actionMethod = action.Method;
#endif

                        new Thread(work.Invoke)
                        {
                            Name = $"{nameof(StressUtility)}.{nameof(RunParallel)} ({actionMethod.Name}) dedicated thread {maxParallelism + 1}"
                        }.Start();
#endif
                    }
                }

                noMoreThreadsEvent.WaitOne();

#if NET_2_0 || NET_3_5 || NET_4_0
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
