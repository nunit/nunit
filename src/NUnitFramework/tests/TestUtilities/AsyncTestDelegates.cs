// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            return Task.FromResult(0);
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
            return Task.Delay(milliseconds);
        }
    }
}
