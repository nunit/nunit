// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Attributes
{
    public class CommandWrapperTests
    {
        [Test]
        public void CorrectExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, "ThrowsCorrectException");
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsCorrectException()
        {
            throw new NullReferenceException();
        }

        [Test]
        public void NoExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, "ThrowsNoException");
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Expected NullReferenceException but no exception was thrown"));
        }

        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNoException()
        {
        }

        [Test]
        public void WrongExceptionThrown()
        {
            var result = TestBuilder.RunTestCase(this, "ThrowsWrongException");
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Expected NullReferenceException but got Exception"));
        }

        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsWrongException()
        {
            throw new Exception();
        }

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
                    Type caughtType = null;

                    try
                    {
                        innerCommand.Execute(context);
                    }
                    catch(Exception ex)
                    {
                        if (ex is NUnitException)
                            ex = ex.InnerException;
                        caughtType = ex.GetType();
                    }

                    if (caughtType == _expectedType)
                        context.CurrentResult.SetResult(ResultState.Success);
                    else if (caughtType != null)
                        context.CurrentResult.SetResult(ResultState.Failure,
                            $"Expected {_expectedType.Name} but got {caughtType.Name}");
                    else
                        context.CurrentResult.SetResult(ResultState.Failure,
                            $"Expected {_expectedType.Name} but no exception was thrown");
                    
                    return context.CurrentResult;
}
            }
        }
    }
}
