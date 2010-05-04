// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ExceptedException processor is used internally to handle the
    /// evaluation of exceptions when a method is annotated with the
    /// ExpectedExceptionAttribute or a ParameterSet specifies an
    /// expected exception.
    /// </summary>
    public class ExpectedExceptionProcessor
    {
        #region Fields
        /// <summary>
        /// The TestMethod to which this exception processor applies
        /// </summary>
        internal TestMethod testMethod;

        /// <summary>
        /// The exception handler method
        /// </summary>
        internal MethodInfo exceptionHandler;

        /// <summary>
        /// The type of any expected exception
        /// </summary>
        internal Type expectedExceptionType;

        /// <summary>
        /// The full name of any expected exception type
        /// </summary>
        internal string expectedExceptionName;

        /// <summary>
        /// The value of any message associated with an expected exception
        /// </summary>
        internal string expectedMessage;

        /// <summary>
        /// A string indicating how to match the expected message
        /// </summary>
        internal MessageMatch matchType;

        /// <summary>
        /// A string containing any user message specified for the expected exception
        /// </summary>
        internal string userMessage;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedExceptionProcessor"/> class
        /// based on an ExpectedExceptionAttribute.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        /// <param name="source">The ExpectedExceptionAttribute.</param>
        public ExpectedExceptionProcessor(TestMethod testMethod, ExpectedExceptionAttribute source)
        {
            this.testMethod = testMethod;

            this.expectedExceptionType = source.ExpectedException;
            this.expectedExceptionName = source.ExpectedExceptionName;
            this.expectedMessage = source.ExpectedMessage;
            this.matchType = source.MatchType;
            this.userMessage = source.UserMessage;

            string handlerName = source.Handler;
            if (handlerName != null)
            {
                MethodInfo handler = GetExceptionHandler(testMethod.FixtureType, handlerName);
                if (handler != null)
                    this.exceptionHandler = handler;
                else
                {
                    testMethod.RunState = RunState.NotRunnable;
                    testMethod.IgnoreReason = string.Format(
                        "The specified exception handler {0} was not found", handlerName);
                }
            }
        }

#if !NUNITLITE
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedExceptionProcessor"/> class
        /// based on a ParameterSet.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        /// <param name="source">The ParameterSet.</param>
        public ExpectedExceptionProcessor(TestMethod testMethod, ParameterSet source)
        {
            this.testMethod = testMethod;

            this.expectedExceptionType = source.ExpectedException;
            this.expectedExceptionName = source.ExpectedExceptionName;
            this.expectedMessage = source.ExpectedMessage;
            this.matchType = source.MatchType;
        }
#endif

        #endregion

        #region Public Methods
        /// <summary>
        /// Handles processing when no exception was thrown.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        public void ProcessNoException(TestResult testResult)
        {
            testResult.SetResult(ResultState.Failure, NoExceptionMessage());
        }

        /// <summary>
        /// Handles processing when an exception was thrown.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="testResult">The test result.</param>
        public void ProcessException(Exception exception, TestResult testResult)
        {
            if (exception is NUnitException)
                exception = exception.InnerException;

            if (IsExpectedExceptionType(exception))
            {
                if (IsExpectedMessageMatch(exception))
                {
                    if (exceptionHandler != null)
                        Reflect.InvokeMethod(exceptionHandler, testMethod.Fixture, exception);
                    else
                    {
                        IExpectException handler = testMethod.Fixture as IExpectException;
                        if (handler != null)
                            handler.HandleException(exception);
                    }

                    testResult.SetResult(ResultState.Success);
                }
                else
                {
#if NETCF_1_0
                    testResult.SetResult(ResultState.Failure, WrongTextMessage(exception));
#else
                    testResult.SetResult(ResultState.Failure, WrongTextMessage(exception), GetStackTrace(exception));
#endif
                }
            }
            else
            {
                testResult.RecordException(exception);

                // If it shows as an error, change it to a failure due to the wrong type
                if (testResult.ResultState == ResultState.Error)
#if NETCF_1_0
                    testResult.SetResult(ResultState.Failure, WrongTypeMessage(exception));
#else
                    testResult.SetResult(ResultState.Failure, WrongTypeMessage(exception), GetStackTrace(exception));
#endif
            }
		}
        #endregion

        #region Helper Methods
        private bool IsExpectedExceptionType(Exception exception)
        {
            return expectedExceptionName == null ||
                expectedExceptionName.Equals(exception.GetType().FullName);
        }

        private bool IsExpectedMessageMatch(Exception exception)
        {
            if (expectedMessage == null)
                return true;

            switch (matchType)
            {
                case MessageMatch.Exact:
                default:
                    return expectedMessage.Equals(exception.Message);
                case MessageMatch.Contains:
                    return exception.Message.IndexOf(expectedMessage) >= 0;
                case MessageMatch.Regex:
                    return Regex.IsMatch(exception.Message, expectedMessage);
                case MessageMatch.StartsWith:
                    return exception.Message.StartsWith(expectedMessage);
            }
        }

        private string NoExceptionMessage()
        {
            string expectedType = expectedExceptionName == null ? "An Exception" : expectedExceptionName;
            return CombineWithUserMessage(expectedType + " was expected");
        }

        private string WrongTypeMessage(Exception exception)
        {
            return CombineWithUserMessage(
                "An unexpected exception type was thrown" + Env.NewLine +
                "Expected: " + expectedExceptionName + Env.NewLine +
                " but was: " + exception.GetType().FullName + " : " + exception.Message);
        }

        private string WrongTextMessage(Exception exception)
        {
            string expectedText;
            switch (matchType)
            {
                default:
                case MessageMatch.Exact:
                    expectedText = "Expected: ";
                    break;
                case MessageMatch.Contains:
                    expectedText = "Expected message containing: ";
                    break;
                case MessageMatch.Regex:
                    expectedText = "Expected message matching: ";
                    break;
                case MessageMatch.StartsWith:
                    expectedText = "Expected message starting: ";
                    break;
            }

            return CombineWithUserMessage(
                "The exception message text was incorrect" + Env.NewLine +
                expectedText + expectedMessage + Env.NewLine +
                " but was: " + exception.Message);
        }

        private string CombineWithUserMessage(string message)
        {
            if (userMessage == null)
                return message;
            return userMessage + Env.NewLine + message;
        }

#if !NETCF_1_0
        private string GetStackTrace(Exception exception)
        {
            try
            {
                return exception.StackTrace;
            }
            catch (Exception)
            {
                return "No stack trace available";
            }
        }
#endif

        private static MethodInfo GetExceptionHandler(Type fixtureType, string name)
        {
            return fixtureType.GetMethod(
                name,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(System.Exception) },
                null);
        }
        #endregion
    }
}
