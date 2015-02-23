// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

#if NET_4_0 || NET_4_5 || SILVERLIGHT || PORTABLE
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
#endif

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
        #pragma warning disable 414
        private CustomType custom;
        #pragma warning restore 414

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
