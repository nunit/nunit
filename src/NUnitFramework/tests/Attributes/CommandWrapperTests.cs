// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    public class CommandWrapperTests
    {
        [Test]
        public void CorrectExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, nameof(ThrowsCorrectException));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

#pragma warning disable NUnit1028 // The non-test method is public
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsCorrectException()
        {
            throw new NullReferenceException();
        }
#pragma warning restore NUnit1028 // The non-test method is public

        [Test]
        public void NoExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, nameof(ThrowsNoException));
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.Message, Is.EqualTo("Expected NullReferenceException but no exception was thrown"));
            });
        }

#pragma warning disable NUnit1028 // The non-test method is public
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNoException()
        {
        }
#pragma warning restore NUnit1028 // The non-test method is public

        [Test]
        public void WrongExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, nameof(ThrowsWrongException));
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.Message, Is.EqualTo("Expected NullReferenceException but got Exception"));
            });
        }

#pragma warning disable NUnit1028 // The non-test method is public
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsWrongException()
        {
            throw new Exception();
        }
#pragma warning restore NUnit1028 // The non-test method is public

        /// <summary>
        /// Extremely simple ExpectedException implementation for use in the test
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        private class ExpectedExceptionAttribute : NUnitAttribute, IWrapTestMethod
        {
            private readonly Type _expectedExceptionType;

            public ExpectedExceptionAttribute(Type type)
            {
                _expectedExceptionType = type;
            }

            public TestCommand Wrap(TestCommand command)
            {
                return new ExpectedExceptionCommand(command, _expectedExceptionType);
            }

            private class ExpectedExceptionCommand : DelegatingTestCommand
            {
                private readonly Type _expectedType;

                public ExpectedExceptionCommand(TestCommand innerCommand, Type expectedType)
                    : base(innerCommand)
                {
                    _expectedType = expectedType;
                }

                public override TestResult Execute(TestExecutionContext context)
                {
                    Type? caughtType = null;

                    try
                    {
                        innerCommand.Execute(context);
                    }
                    catch(Exception ex)
                    {
                        if (ex is NUnitException)
                            ex = ex.InnerException!;
                        caughtType = ex.GetType();
                    }

                    if (caughtType == _expectedType)
                    {
                        context.CurrentResult.SetResult(ResultState.Success);
                    }
                    else if (caughtType is not null)
                    {
                        context.CurrentResult.SetResult(ResultState.Failure,
                            $"Expected {_expectedType.Name} but got {caughtType.Name}");
                    }
                    else
                    {
                        context.CurrentResult.SetResult(ResultState.Failure,
                            $"Expected {_expectedType.Name} but no exception was thrown");
                    }

                    return context.CurrentResult;
}
            }
        }
    }
}
