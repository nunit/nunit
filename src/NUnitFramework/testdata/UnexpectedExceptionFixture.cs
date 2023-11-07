// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.UnexpectedExceptionFixture
{
    [TestFixture]
    public class UnexpectedExceptionFixture
    {
        [Test]
        public void ThrowsWithInnerException()
        {
            throw new Exception("Outer Exception", new Exception("Inner Exception"));
        }

        [Test]
        public void ThrowsWithNestedInnerException()
        {
            throw new Exception("Outer Exception",
                new Exception("Inner Exception",
                    new Exception("Inner Inner Exception")));
        }

        [Test]
        public void ThrowsWithAggregateException()
        {
            throw new AggregateException("Outer Aggregate Exception",
                new Exception("Inner Exception 1 of 2"),
                new Exception("Inner Exception 2 of 2"));
        }

        [Test]
        public void ThrowsWithAggregateExceptionContainingNestedInnerException()
        {
            throw new AggregateException("Outer Aggregate Exception",
                new Exception("Inner Exception",
                    new Exception("Inner Inner Exception")));
        }

        [Test]
        public void ThrowsWithBadStackTrace()
        {
            throw new ExceptionWithBadStackTrace("thrown by me");
        }

        [Test]
        public void ThrowsCustomException()
        {
            throw new CustomException("message", new CustomType());
        }

        [Test]
        public void AssertThatWithRecursivelyThrowingExceptionAsActual()
        {
            Assert.That(new RecursivelyThrowingException(), Is.Null);
        }

        [Test]
        public void AssertThatWithRecursivelyThrowingExceptionAsExpected()
        {
            var actual = default(RecursivelyThrowingException);
            Assert.That(actual, Is.EqualTo(new RecursivelyThrowingException()));
        }
    }

    internal class CustomException : Exception
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly CustomType _custom;
#pragma warning restore IDE0052 // Remove unread private members

        public CustomException(string msg, CustomType custom)
            : base(msg)
        {
            _custom = custom;
        }
    }

    internal class CustomType
    {
    }

    public class ExceptionWithBadStackTrace : Exception
    {
        public ExceptionWithBadStackTrace(string message)
            : base(message)
        {
        }

        public override string StackTrace => throw new InvalidOperationException("Simulated failure getting stack trace");
    }
}
