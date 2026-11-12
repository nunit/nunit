// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public static class ExceptionHelperTests
    {
        [Test]
        public static void BuildMessageThrowsForNullException()
        {
            Assert.That(() => ExceptionHelper.BuildMessage(null!), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("exception"));
        }

        [Test]
        public static void RecordExceptionThrowsForNullDelegate()
        {
            Assert.That(
                () => ExceptionHelper.RecordException(null!, "someParamName"),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("someParamName"));
        }

        [Test]
        public static void RecordExceptionThrowsForDelegateThatRequiresParameters()
        {
            Assert.That(
                () => ExceptionHelper.RecordException(new Action<object>(_ => { }), "someParamName"),
                Throws.ArgumentException.With.Property("ParamName").EqualTo("someParamName"));
        }

        [Test]
        public static void RecordExceptionHandlesDelegatesThatHaveOneFewerParameterThanTheBoundMethod()
        {
            Assert.That(
                ExceptionHelper.RecordException(new Foo(null).ThrowingExtensionMethod, "someParamName"),
                Is.Null);

            var exceptionToThrow = new Exception();

            Assert.That(
                ExceptionHelper.RecordException(new Foo(exceptionToThrow).ThrowingExtensionMethod, "someParamName"),
                Is.SameAs(exceptionToThrow));
        }

        [Test]
        public static void RecordExceptionThrowsProperExceptionForDelegatesThatHaveOneMoreParameterThanTheBoundMethod()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DummyInstanceMethod))!;
            var delegateThatParameterizesTheInstance = (Action<Foo>)methodInfo.CreateDelegate(typeof(Action<Foo>));

            Assert.That(
                () => ExceptionHelper.RecordException(delegateThatParameterizesTheInstance, "someParamName"),
                Throws.ArgumentException);
        }

        private sealed class Foo
        {
            public Foo(Exception? exceptionToThrow)
            {
                ExceptionToThrow = exceptionToThrow;
            }

            public Exception? ExceptionToThrow { get; }

            public void DummyInstanceMethod()
            {
            }
        }

        private static void ThrowingExtensionMethod(this Foo foo)
        {
            if (foo.ExceptionToThrow is not null)
                throw foo.ExceptionToThrow;
        }

        [Test]
        public static void RecordExceptionReturnsExceptionThrownBeforeReturningAwaitableObject()
        {
            var exceptionToThrow = new Exception();

            Assert.That(
                ExceptionHelper.RecordException(new Func<Task>(() => throw exceptionToThrow), "someParamName"),
                Is.SameAs(exceptionToThrow));
        }

        [Test]
        public static void RecordExceptionReturnsExceptionFromAwaitableObjectResult()
        {
            var exceptionToThrow = new Exception();

            Assert.That(
                ExceptionHelper.RecordException(new Func<Task>(() => TaskFromException(exceptionToThrow)), "someParamName"),
                Is.SameAs(exceptionToThrow));
        }

        private const string OuterTrace = "   at NUnit.Framework.Internal.Outer()\r\n   at User.Outer()";
        private const string InnerTrace = "   at NUnit.Framework.Internal.Inner()\r\n   at User.Inner()";

        private static readonly StackFilter NUnitOnlyFilter = StackFilter.Create(
            nunitStackPattern: @"^ +at NUnit\.", systemStackPattern: null, runnerStackPattern: null);

        [Test]
        public static void BuildStackTraceWithoutFilterKeepsStackTracesUnchanged()
        {
            var exception = new FixedStackTraceException(OuterTrace, new FixedStackTraceException(InnerTrace, null));

            Assert.That(
                ExceptionHelper.BuildStackTrace(exception),
                Is.EqualTo(OuterTrace + Environment.NewLine + "--" + nameof(FixedStackTraceException) + Environment.NewLine + InnerTrace));
        }

        [Test]
        public static void BuildStackTraceFiltersEachExceptionSeparately()
        {
            var exception = new FixedStackTraceException(OuterTrace, new FixedStackTraceException(InnerTrace, null));

            Assert.That(
                ExceptionHelper.BuildStackTrace(exception, NUnitOnlyFilter),
                Is.EqualTo("   at User.Outer()" + Environment.NewLine +
                           "--" + nameof(FixedStackTraceException) + Environment.NewLine +
                           "   at User.Inner()" + Environment.NewLine));
        }

        private sealed class FixedStackTraceException : Exception
        {
            private readonly string _stackTrace;

            public FixedStackTraceException(string stackTrace, Exception? innerException)
                : base("message", innerException)
            {
                _stackTrace = stackTrace;
            }

            public override string StackTrace => _stackTrace;
        }

        // Task.FromException was added in .NET Framework 4.6
        private static Task TaskFromException(Exception exception)
        {
            var source = new TaskCompletionSource<object>();
            source.SetException(exception);
            return source.Task;
        }
    }
}
