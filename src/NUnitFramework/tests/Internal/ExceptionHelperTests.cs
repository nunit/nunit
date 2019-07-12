// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Compatibility;
#if TASK_PARALLEL_LIBRARY_API
using System.Threading.Tasks;
#endif

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

#if TASK_PARALLEL_LIBRARY_API
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
#endif
    }
}
