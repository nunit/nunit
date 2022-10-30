// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.UnexpectedExceptionFixture
{
    /// <summary>
    /// This models a .NET Framework race condition that resulted in stack traces throwing AccessViolationException
    /// which would produce another AccessViolationException while attempting to print its stack trace in turn.
    /// </summary>
    public sealed class RecursivelyThrowingException : Exception
    {
        public override string Message => throw this;

        public override IDictionary Data => throw this;

        public override string StackTrace => throw this;

        public override string HelpLink
        {
            get => throw this;
            set => throw this;
        }

        public override string Source
        {
            get => throw this;
            set => throw this;
        }

        public override bool Equals(object obj) => throw this;

        public override Exception GetBaseException() => throw this;

        public override int GetHashCode() => throw this;

        public override string ToString() => throw this;
    }
}
