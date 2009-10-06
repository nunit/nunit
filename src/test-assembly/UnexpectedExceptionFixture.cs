// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using NUnit.Framework;

namespace NUnit.TestData
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
        public void ThrowsWithBadStackTrace()
        {
            throw new ExceptionWithBadStackTrace("thrown by me");
        }

        [Test]
        public void ThrowsCustomException()
        {
            throw new CustomException("message", new CustomType());
        }
    }

    class CustomException : Exception
    {
        private CustomType custom;

        public CustomException(string msg, CustomType custom)
            : base(msg)
        {
            this.custom = custom;
        }
    }

    class CustomType
    {
    }

    public class ExceptionWithBadStackTrace : Exception
    {
        public ExceptionWithBadStackTrace(string message)
            : base(message) { }

        public override string StackTrace
        {
            get
            {
                throw new InvalidOperationException("Simulated failure getting stack trace");
            }
        }
    }
}
