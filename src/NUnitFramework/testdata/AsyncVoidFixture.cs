// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public static class AsyncVoidFixture
    {
        public const string Message = "Exception from  inside async void method.";

        [TearDown]
        public static void TearDown()
        {
            // Emulate other test running activity to give async void time to complete
            Thread.Sleep(1000);
        }

        [Test]
        public static void NonAsyncTestCallingAsyncVoidMethod()
        {
            AsyncVoidMethod();
        }

        private static async void AsyncVoidMethod()
        {
            await Task.Delay(10);
            throw new InvalidOperationException(Message);
        }
    }
}
