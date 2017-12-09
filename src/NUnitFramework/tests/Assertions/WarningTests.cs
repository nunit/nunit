// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class WarningTests
    {
        [TestCase("WarnUnless_Passes_Boolean")]
        [TestCase("WarnIf_Passes_Boolean")]
        [TestCase("WarnUnless_Passes_BooleanWithMessage")]
        [TestCase("WarnIf_Passes_BooleanWithMessage")]
        [TestCase("WarnUnless_Passes_BooleanWithMessageAndArgs")]
        [TestCase("WarnIf_Passes_BooleanWithMessageAndArgs")]
#if !NET20
        [TestCase("WarnUnless_Passes_BooleanWithMessageStringFunc")]
        [TestCase("WarnIf_Passes_BooleanWithMessageStringFunc")]
        [TestCase("WarnUnless_Passes_BooleanLambda")]
        [TestCase("WarnIf_Passes_BooleanLambda")]
        [TestCase("WarnUnless_Passes_BooleanLambdaWithMessage")]
        [TestCase("WarnIf_Passes_BooleanLambdaWithMessage")]
        [TestCase("WarnUnless_Passes_BooleanLambdaWithMessageAndArgs")]
        [TestCase("WarnIf_Passes_BooleanLambdaWithMessageAndArgs")]
        [TestCase("WarnUnless_Passes_BooleanLambdaWithWithMessageStringFunc")]
        [TestCase("WarnIf_Passes_BooleanLambdaWithWithMessageStringFunc")]
#endif
        [TestCase("WarnUnless_Passes_ActualAndConstraint")]
        [TestCase("WarnIf_Passes_ActualAndConstraint")]
        [TestCase("WarnUnless_Passes_ActualAndConstraintWithMessage")]
        [TestCase("WarnIf_Passes_ActualAndConstraintWithMessage")]
        [TestCase("WarnUnless_Passes_ActualAndConstraintWithMessageAndArgs")]
        [TestCase("WarnIf_Passes_ActualAndConstraintWithMessageAndArgs")]
#if !NET20
        [TestCase("WarnUnless_Passes_ActualAndConstraintWithMessageStringFunc")]
        [TestCase("WarnIf_Passes_ActualAndConstraintWithMessageStringFunc")]
        [TestCase("WarnUnless_Passes_ActualLambdaAndConstraint")]
        [TestCase("WarnIf_Passes_ActualLambdaAndConstraint")]
        [TestCase("WarnUnless_Passes_ActualLambdaAndConstraintWithMessage")]
        [TestCase("WarnIf_Passes_ActualLambdaAndConstraintWithMessage")]
        [TestCase("WarnUnless_Passes_ActualLambdaAndConstraintWithMessageAndArgs")]
        [TestCase("WarnIf_Passes_ActualLambdaAndConstraintWithMessageAndArgs")]
        [TestCase("WarnUnless_Passes_ActualLambdaAndConstraintWithMessageStringFunc")]
        [TestCase("WarnIf_Passes_ActualLambdaAndConstraintWithMessageStringFunc")]
#endif
        [TestCase("WarnUnless_Passes_DelegateAndConstraint")]
        [TestCase("WarnIf_Passes_DelegateAndConstraint")]
        [TestCase("WarnUnless_Passes_DelegateAndConstraintWithMessage")]
        [TestCase("WarnIf_Passes_DelegateAndConstraintWithMessage")]
        [TestCase("WarnUnless_Passes_DelegateAndConstraintWithMessageAndArgs")]
        [TestCase("WarnIf_Passes_DelegateAndConstraintWithMessageAndArgs")]
#if !NET20
        [TestCase("WarnUnless_Passes_DelegateAndConstraintWithMessageStringFunc")]
        [TestCase("WarnIf_Passes_DelegateAndConstraintWithMessageStringFunc")]
#endif
#if ASYNC
        [TestCase("WarnUnless_Passes_Async")]
        [TestCase("WarnIf_Passes_Async")]
#endif
        public void WarningPasses(string methodName)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(0), "There should be no AssertionResults");
        }

        [TestCase("WarnUnless_Fails_Boolean", null)]
        [TestCase("WarnIf_Fails_Boolean", null)]
        [TestCase("WarnUnless_Fails_BooleanWithMessage", "message")]
        [TestCase("WarnIf_Fails_BooleanWithMessage", "message")]
        [TestCase("WarnUnless_Fails_BooleanWithMessageAndArgs", "got 5")]
        [TestCase("WarnIf_Fails_BooleanWithMessageAndArgs", "got 5")]
#if !NET20
        [TestCase("WarnUnless_Fails_BooleanWithMessageStringFunc", "got 5")]
        [TestCase("WarnIf_Fails_BooleanWithMessageStringFunc", "got 5")]
        [TestCase("WarnUnless_Fails_BooleanLambda", null)]
        [TestCase("WarnIf_Fails_BooleanLambda", null)]
        [TestCase("WarnUnless_Fails_BooleanLambdaWithMessage", "message")]
        [TestCase("WarnIf_Fails_BooleanLambdaWithMessage", "message")]
        [TestCase("WarnUnless_Fails_BooleanLambdaWithMessageAndArgs", "got 5")]
        [TestCase("WarnIf_Fails_BooleanLambdaWithMessageAndArgs", "got 5")]
        [TestCase("WarnUnless_Fails_BooleanLambdaWithMessageStringFunc", "got 5")]
        [TestCase("WarnIf_Fails_BooleanLambdaWithMessageStringFunc", "got 5")]
#endif
        [TestCase("WarnUnless_Fails_ActualAndConstraint", null)]
        [TestCase("WarnIf_Fails_ActualAndConstraint", null)]
        [TestCase("WarnUnless_Fails_ActualAndConstraintWithMessage", "Error")]
        [TestCase("WarnIf_Fails_ActualAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_Fails_ActualAndConstraintWithMessageAndArgs", "Should be 5")]
        [TestCase("WarnIf_Fails_ActualAndConstraintWithMessageAndArgs", "Should be 5")]
#if !NET20
        [TestCase("WarnUnless_Fails_ActualAndConstraintWithMessageStringFunc", "Should be 5")]
        [TestCase("WarnIf_Fails_ActualAndConstraintWithMessageStringFunc", "Should be 5")]
        [TestCase("WarnUnless_Fails_ActualLambdaAndConstraint", null)]
        [TestCase("WarnIf_Fails_ActualLambdaAndConstraint", null)]
        [TestCase("WarnUnless_Fails_ActualLambdaAndConstraintWithMessage", "Error")]
        [TestCase("WarnIf_Fails_ActualLambdaAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_Fails_ActualLambdaAndConstraintWithMessageAndArgs", "Should be 5")]
        [TestCase("WarnIf_Fails_ActualLambdaAndConstraintWithMessageAndArgs", "Should be 5")]
        [TestCase("WarnUnless_Fails_ActualLambdaAndConstraintWithMessageStringFunc", "Should be 5")]
        [TestCase("WarnIf_Fails_ActualLambdaAndConstraintWithMessageStringFunc", "Should be 5")]
#endif
        [TestCase("WarnUnless_Fails_DelegateAndConstraint", null)]
        [TestCase("WarnIf_Fails_DelegateAndConstraint", null)]
        [TestCase("WarnUnless_Fails_DelegateAndConstraintWithMessage", "Error")]
        [TestCase("WarnIf_Fails_DelegateAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_Fails_DelegateAndConstraintWithMessageAndArgs", "Should be 4")]
        [TestCase("WarnIf_Fails_DelegateAndConstraintWithMessageAndArgs", "Should be 4")]
#if !NET20
        [TestCase("WarnUnless_Fails_DelegateAndConstraintWithMessageStringFunc", "Should be 4")]
        [TestCase("WarnIf_Fails_DelegateAndConstraintWithMessageStringFunc", "Should be 4")]
#endif
#if ASYNC
        [TestCase("WarnUnless_Fails_Async", null)]
        [TestCase("WarnIf_Fails_Async", null)]
#endif
        public void WarningFails(string methodName, string expectedMessage)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
            Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(1), "Incorrect number of AssertionResults");
            Assert.That(result.AssertionResults[0].Status, Is.EqualTo(AssertionStatus.Warning));
            Assert.That(result.AssertionResults[0].Message, Is.Not.Null, "Assertion Message should not be null");
            Assert.That(result.Message, Is.Not.Null, "Result Message should not be null");
            Assert.That(result.Message, Contains.Substring(result.AssertionResults[0].Message), "Result message should contain assertion message");
            Assert.That(result.AssertionResults[0].StackTrace, Does.Contain("WarningFixture"));
            Assert.That(result.AssertionResults[0].StackTrace.Split(new char[] { '\n' }).Length, Is.LessThan(3));

            if (expectedMessage != null)
            {
                Assert.That(result.Message, Does.Contain(expectedMessage));
                Assert.That(result.AssertionResults[0].Message, Does.Contain(expectedMessage));
            }
        }

#if !NET20
        [Test]
        public void PassingAssertion_DoesNotCallExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;
            Func<string> getExceptionMessage = () =>
            {
                funcWasCalled = true;
                return "Func was called";
            };

            // Act
            using (new TestExecutionContext.IsolatedContext())
                Warn.Unless(0 + 1 == 1, getExceptionMessage);

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingWarning_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;
            Func<string> getExceptionMessage = () =>
            {
                funcWasCalled = true;
                return "Func was called";
            };

            // Act
            using (new TestExecutionContext.IsolatedContext())
                Warn.Unless(1 + 1 == 1, getExceptionMessage);

            // Assert
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }
#endif

#if ASYNC
        [Test]
        public void WarnUnless_Async_Error()
        {
#if !NET40
            var exception =
#endif
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.Unless(async () => await ThrowExceptionGenericTask(), Is.EqualTo(1)));

#if !NET40
            Assert.That(exception.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
#endif
        }

        [Test]
        public void WarnIf_Async_Error()
        {
#if !NET40
            var exception =
#endif
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.If(async () => await ThrowExceptionGenericTask(), Is.Not.EqualTo(1)));

#if !NET40
            Assert.That(exception.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
#endif
        }

        private static Task<int> One()
        {
            return Task.Run(() => 1);
        }

        private static async Task<int> ThrowExceptionGenericTask()
        {
            await One();
            throw new InvalidOperationException();
        }
#endif

        // We decided to trim ExecutionContext and below because ten lines per warning adds up
        // and makes it hard to read build logs.
        // See https://github.com/nunit/nunit/pull/2431#issuecomment-328404432.
        [TestCase(nameof(WarningFixture.WarningSynchronous), 1)]
        [TestCase(nameof(WarningFixture.WarningInThreadStart), 2)]
#if !PLATFORM_DETECTION
        [TestCase(nameof(WarningFixture.WarningInBeginInvoke), 4)]
#else
        [TestCase(nameof(WarningFixture.WarningInBeginInvoke), 4, ExcludePlatform = "mono", Reason = "Warning has no effect inside BeginInvoke on Mono")]
#endif
        [TestCase(nameof(WarningFixture.WarningInThreadPoolQueueUserWorkItem), 2)]
#if ASYNC
        [TestCase(nameof(WarningFixture.WarningInTaskRun), 4)]
        [TestCase(nameof(WarningFixture.WarningAfterAwaitTaskDelay), 5)]
#endif
        public static void StackTracesAreFiltered(string methodName, int maxLineCount)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);
            if (result.FailCount != 0 && result.Message.StartsWith(typeof(PlatformNotSupportedException).FullName))
            {
                return; // BeginInvoke causes PlatformNotSupportedException on .NET Core
            }

            if (result.AssertionResults.Count != 1 || result.AssertionResults[0].Status != AssertionStatus.Warning)
            {
                Assert.Fail("Expected a single warning assertion. Message: " + result.Message);
            }

            var warningStackTrace = result.AssertionResults[0].StackTrace;
            var lines = warningStackTrace.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (maxLineCount < lines.Length)
            {
                Assert.Fail(
                    $"Expected the number of lines to be no more than {maxLineCount}, but it was {lines.Length}:" + Environment.NewLine
                    + Environment.NewLine
                    + string.Concat(lines.Select((line, i) => $" {i + 1}. {line.Trim()}" + Environment.NewLine))
                    + "(end)");

                 // ^ Most of that is to differentiate it from the current method's stack trace
                 // reported directly underneath at the same level of indentation.
            }
        }
    }
}
