// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.ExecutionHooks;

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
            await Task.Yield();
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
            await Task.Yield();
            await ThrowSpecifiedExceptionAsync<InvalidOperationException>();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task ThrowSpecifiedExceptionAsync<T>()
            where T : Exception
        {
            await Task.Yield();
            throw (T)Activator.CreateInstance(typeof(T), "Message")!;
        }

        [Test]
        public static async Task ThrowExceptionInNestedClassAsyncMethod()
        {
            await Task.Yield();
            await NestedClass.ThrowExceptionInAsyncMethod();
        }

        // The async lambda is a method of the compiler-generated class "<>c",
        // its async state machine a nested type of that class.
        [Test]
        public static async Task ThrowExceptionInAsyncLambda()
        {
            Func<Task> throwAsync = async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("Not allowed at this time.");
            };

            await Task.Yield();
            await throwAsync();
        }

        // The async state machine of a local function is a nested type of the declaring class.
        [Test]
        public static async Task ThrowExceptionInAsyncLocalFunction()
        {
            await Task.Yield();
            await ThrowAsync();

            static async Task ThrowAsync()
            {
                await Task.Yield();
                throw new InvalidOperationException("Not allowed at this time.");
            }
        }

        // User extensions run inside the NUnit command pipeline (NUnit.Framework.Internal.Commands),
        // their frames must survive the cut-off at that pipeline.
        [Test]
        [FailingCommandWrapper]
        public static void FailureInUserCommandWrapper()
        {
        }

        [Test]
        [PassingCommandWrapper]
        public static void FailureInTestWrappedByUserCommand()
        {
            AssertThatFailure();
        }

        [Test]
        [FailingExecutionHook]
        public static void FailureInExecutionHook()
        {
        }

        [Test]
        [FailingTestAction]
        public static void FailureInTestAction()
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class FailingCommandWrapperAttribute : Attribute, IWrapTestMethod
        {
            public TestCommand Wrap(TestCommand command) => new FailingCommand(command);

            private sealed class FailingCommand : DelegatingTestCommand
            {
                public FailingCommand(TestCommand innerCommand)
                    : base(innerCommand)
                {
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                public override TestResult Execute(TestExecutionContext context)
                {
                    Assert.Fail("Failure in user command wrapper.");
                    return innerCommand.Execute(context);
                }
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class PassingCommandWrapperAttribute : Attribute, IWrapTestMethod
        {
            public TestCommand Wrap(TestCommand command) => new PassingCommand(command);

            private sealed class PassingCommand : DelegatingTestCommand
            {
                public PassingCommand(TestCommand innerCommand)
                    : base(innerCommand)
                {
                }

                public override TestResult Execute(TestExecutionContext context) => innerCommand.Execute(context);
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class FailingExecutionHookAttribute : ExecutionHookAttribute
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public override void BeforeTestHook(HookData hookData)
            {
                Assert.Fail("Failure in user execution hook.");
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class FailingTestActionAttribute : Attribute, ITestAction
        {
            public ActionTargets Targets => ActionTargets.Test;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void BeforeTest(ITest test)
            {
                Assert.Fail("Failure in user test action.");
            }

            public void AfterTest(ITest test)
            {
            }
        }

        // The async state machines of these methods are nested types of a nested type.
        // .NET Framework writes them as "StackFilterFixture.NestedClass.<Method>d__N.MoveNext()",
        // Mono as "StackFilterFixture+NestedClass+<Method>d__N.MoveNext ()", including generic parameters.
        public static class NestedClass
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static async Task ThrowExceptionInAsyncMethod()
            {
                await Task.Yield();
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
}
