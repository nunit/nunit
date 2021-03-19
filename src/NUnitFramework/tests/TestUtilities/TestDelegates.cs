// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.TestUtilities
{
    public class TestDelegates
    {
        public static void ThrowsArgumentException()
        {
            throw new ArgumentException("myMessage", "myParam");
        }

        public static void ThrowsArgumentNullException()
        {
            throw new ArgumentNullException("myParam", "myMessage");
        }

        public static void ThrowsNullReferenceException()
        {
            throw new NullReferenceException("my message");
        }

        public static void ThrowsSystemException()
        {
            throw new Exception("my message");
        }

        public static void ThrowsNothing()
        {
        }

        public static void ThrowsBaseException()
        {
            throw new BaseException();
        }

        public static void ThrowsDerivedException()
        {
            throw new DerivedException();
        }

        public static int ThrowsInsteadOfReturns()
        {
            throw new Exception("my message");
        }

        public class BaseException : Exception
        {
        }

        public class DerivedException : BaseException
        {
        }
    }
}
