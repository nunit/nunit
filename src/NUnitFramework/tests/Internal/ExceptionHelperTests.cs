// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    public static class ExceptionHelperTests
    {
        [Test]
        public static void BuildMessageThrowsForNullException()
        {
            Assert.That(() => ExceptionHelper.BuildMessage(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("exception"));
        }

        [Test]
        public static void RecordExceptionThrowsForNullDelegate()
        {
            Assert.That(
                () => ExceptionHelper.RecordException(null, "someParamName"),
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
                ExceptionHelper.RecordException(new TestDelegate(new Foo(null).ThrowingExtensionMethod), "someParamName"),
                Is.Null);

            var exceptionToThrow = new Exception();

            Assert.That(
                ExceptionHelper.RecordException(new TestDelegate(new Foo(exceptionToThrow).ThrowingExtensionMethod), "someParamName"),
                Is.SameAs(exceptionToThrow));
        }

        [Test]
        public static void RecordExceptionThrowsProperExceptionForDelegatesThatHaveOneMoreParameterThanTheBoundMethod()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DummyInstanceMethod));
            var delegateThatParameterizesTheInstance = (Action<Foo>)methodInfo.CreateDelegate(typeof(Action<Foo>));

            Assert.That(
                () => ExceptionHelper.RecordException(delegateThatParameterizesTheInstance, "someParamName"),
                Throws.ArgumentException);
        }

        private sealed class Foo
        {
            public Foo(Exception exceptionToThrow)
            {
                ExceptionToThrow = exceptionToThrow;
            }

            public Exception ExceptionToThrow { get; }

            public void DummyInstanceMethod()
            {
            }
        }

        private static void ThrowingExtensionMethod(this Foo foo)
        {
            if (foo.ExceptionToThrow != null)
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

        // Task.FromException was added in .NET Framework 4.6
        private static Task TaskFromException(Exception exception)
        {
            var source = new TaskCompletionSource<object>();
            source.SetException(exception);
            return source.Task;
        }
    }
}
