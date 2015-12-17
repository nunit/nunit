#if NET_4_5 || PORTABLE

using System;
using System.Threading.Tasks;

namespace NUnit.TestUtilities
{
    public class AsyncTestDelegates
    {
        public static Task ThrowsArgumentExceptionSync()
        {
            throw new ArgumentException("myMessage", "myParam");
        }

        public static async Task ThrowsArgumentExceptionAsync()
        {
            await Task.Yield();
            throw new ArgumentException("myMessage", "myParam");
        }

        public static Task ThrowsNothing()
        {
            return Task.FromResult(0);
        }

        public static Task ThrowsNullReferenceExceptionSync()
        {
            throw new NullReferenceException("my message");
        }

        public static async Task ThrowsNullReferenceExceptionAsync()
        {
            await Task.Yield();
            throw new NullReferenceException("my message");
        }

        public static Task ThrowsSystemExceptionSync()
        {
            throw new Exception("my message");
        }

        public static async Task ThrowsSystemExceptionAsync()
        {
            await Task.Yield();
            throw new Exception("my message");
        }
    }
}

#endif