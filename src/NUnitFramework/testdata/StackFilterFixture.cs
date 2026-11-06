// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public static class StackFilterFixture
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AssertThatFailure()
        {
            Assert.That(1, Is.EqualTo(2));
        }

        [Test]
        public static void AssertThatFailureInUserMethod()
        {
            AssertThatFailure();
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task AssertThatFailureAsync()
        {
            await Task.Yield();
            Assert.That(1, Is.EqualTo(2));
        }

        [Test]
        public static async Task AssertThatFailureInAsyncUserMethod()
        {
            await Task.Yield();
            await AssertThatFailureAsync();
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowException()
        {
            throw new InvalidOperationException("Not allowed at this time.");
        }

        [Test]
        public static async Task ThrowExceptionInUserMethod()
        {
            await Task.Delay(1);
            ThrowException();
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task ThrowExceptionAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException("Not allowed at this time.");
        }

        [Test]
        public static async Task ThrowExceptionInAsyncUserMethod()
        {
            await Task.Delay(1);
            await ThrowSpecifiedExceptionAsync<InvalidOperationException>();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task ThrowSpecifiedExceptionAsync<T>()
            where T : Exception
        {
            await Task.Yield();
            throw (T)Activator.CreateInstance(typeof(T), "Message")!;
        }
    }
}
