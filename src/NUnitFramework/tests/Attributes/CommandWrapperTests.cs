// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes.Tests
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
            private Type _expectedExceptionType;

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
                private Type _expectedType;

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
                            string.Format("Expected {0} but got {1}", _expectedType.Name, caughtType.Name));
                    else
                        context.CurrentResult.SetResult(ResultState.Failure,
                            string.Format("Expected {0} but no exception was thrown", _expectedType.Name));
                    
                    return context.CurrentResult;
}
            }
        }
    }
}
