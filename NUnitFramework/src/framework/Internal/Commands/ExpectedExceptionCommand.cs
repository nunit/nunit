// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Threading;
using NUnit.Framework.Api;
using System.Text.RegularExpressions;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class ExpectedExceptionCommand : DelegatingTestCommand
    {
        //private readonly ExpectedExceptionProcessor exceptionProcessor;
        private TestMethod testMethod;
        private MethodInfo exceptionHandler;
        private Type expectedExceptionType;
        private string expectedExceptionName;
        private string expectedMessage;
        private MessageMatch matchType;
        private string userMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedExceptionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public ExpectedExceptionCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            this.testMethod = (TestMethod)Test;
            //ExpectedExceptionData exceptionData = testMethod.ExpectedExceptionData;

            this.expectedExceptionType = testMethod.ExpectedExceptionType;
            this.expectedExceptionName = testMethod.ExpectedExceptionName;
            this.expectedMessage = testMethod.ExpectedExceptionMessage;
            this.matchType = testMethod.MessageMatchType;
            this.userMessage = testMethod.ExpectedExceptionUserMessage;
            this.exceptionHandler = testMethod.AlternateExceptionHandler;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(object testObject, ITestListener listener)
        {
            try
            {
                CurrentResult = innerCommand.Execute(testObject, listener);

                if (CurrentResult.ResultState == ResultState.Success)
                    ProcessNoException(CurrentResult);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                ProcessException(ex, testObject);
            }

            return CurrentResult;
        }

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
        public void ProcessException(Exception exception, object testObject)
        {
            if (exception is NUnitException)
                exception = exception.InnerException;

            if (IsExpectedExceptionType(exception))
            {
                if (IsExpectedMessageMatch(exception))
                {
                    if (exceptionHandler != null)
                        Reflect.InvokeMethod(exceptionHandler, testObject, exception);
                    else
                    {
                        IExpectException handler = testObject as IExpectException;
                        if (handler != null)
                            handler.HandleException(exception);
                    }

                    CurrentResult.SetResult(ResultState.Success);
                }
                else
                {
#if NETCF_1_0
                    CurrentResult.SetResult(ResultState.Failure, WrongTextMessage(exception));
#else
                    CurrentResult.SetResult(ResultState.Failure, WrongTextMessage(exception), GetStackTrace(exception));
#endif
                }
            }
            else
            {
                CurrentResult.RecordException(exception);

                // If it shows as an error, change it to a failure due to the wrong type
                if (CurrentResult.ResultState == ResultState.Error)
#if NETCF_1_0
                    CurrentResult.SetResult(ResultState.Failure, WrongTypeMessage(exception));
#else
                    CurrentResult.SetResult(ResultState.Failure, WrongTypeMessage(exception), GetStackTrace(exception));
#endif
            }
        }

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

        #endregion
    }
}