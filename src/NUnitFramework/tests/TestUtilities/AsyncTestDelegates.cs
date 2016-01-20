#if NET_4_0 || NET_4_5 || PORTABLE

using System;
using System.Threading.Tasks;

namespace NUnit.TestUtilities
{
    public class AsyncTestDelegates
    {
        public static Task ThrowsArgumentException()
        {
            throw new ArgumentException("myMessage", "myParam");
        }

        public static Task ThrowsArgumentExceptionAsync()
        {
            return Delay(5).ContinueWith(t => { throw new ArgumentException("myMessage", "myParam"); }, TaskScheduler.Default);
        }

        public static Task ThrowsNothing()
        {
#if NET_4_0
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
#else
            return Task.FromResult(0);
#endif
        }

        public static Task ThrowsNullReferenceException()
        {
            throw new NullReferenceException("my message");
        }

        public static Task ThrowsNullReferenceExceptionAsync()
        {
            return Delay(5).ContinueWith(t => { throw new NullReferenceException("my message"); }, TaskScheduler.Default);
        }

        public static Task ThrowsSystemException()
        {
            throw new Exception("my message");
        }

        public static Task ThrowsSystemExceptionAsync()
        {
            return Delay(5).ContinueWith(t => { throw new Exception("my message"); }, TaskScheduler.Default);
        }

        public static Task Delay(int milliseconds)
        {
#if NET_4_0
            var tcs = new TaskCompletionSource<int>();
            var timer = new System.Threading.Timer(_ => tcs.SetResult(0), null, milliseconds, System.Threading.Timeout.Infinite);
            return tcs.Task.ContinueWith(t => timer.Dispose(), TaskScheduler.Default);
#else
            return Task.Delay(milliseconds);
#endif
        }
    }
}

#endif