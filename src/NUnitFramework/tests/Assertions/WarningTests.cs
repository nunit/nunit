// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class WarningTests
    {
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_Boolean))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_Boolean))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_BooleanWithMessage))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_BooleanWithMessage))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_BooleanWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_BooleanWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_BooleanLambda))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_BooleanLambda))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_BooleanLambdaWithMessage))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_BooleanLambdaWithMessage))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_BooleanLambdaWithWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_BooleanLambdaWithWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualLambdaAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualLambdaAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualLambdaAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualLambdaAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_ActualLambdaAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_ActualLambdaAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_DelegateAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_DelegateAndConstraint))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_DelegateAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_DelegateAndConstraintWithMessage))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_DelegateAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_DelegateAndConstraintWithMessageStringFunc))]
        [TestCase(nameof(WarningFixture.WarnUnless_Passes_Async))]
        [TestCase(nameof(WarningFixture.WarnIf_Passes_Async))]
        public void WarningPasses(string methodName)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
                Assert.That(result.AssertionResults, Is.Empty, "There should be no AssertionResults");
            });
        }

        [TestCase(nameof(WarningFixture.WarnUnless_Fails_Boolean), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_Boolean), null)]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_BooleanWithMessage), "message")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_BooleanWithMessage), "message")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_BooleanWithMessageStringFunc), "got 5")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_BooleanWithMessageStringFunc), "got 5")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_BooleanLambda), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_BooleanLambda), null)]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_BooleanLambdaWithMessage), "message")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_BooleanLambdaWithMessage), "message")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_BooleanLambdaWithMessageStringFunc), "got 5")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_BooleanLambdaWithMessageStringFunc), "got 5")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualAndConstraintWithMessageStringFunc), "Should be 5")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualAndConstraintWithMessageStringFunc), "Should be 5")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualLambdaAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualLambdaAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualLambdaAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualLambdaAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_ActualLambdaAndConstraintWithMessageStringFunc), "Should be 5")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_ActualLambdaAndConstraintWithMessageStringFunc), "Should be 5")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_DelegateAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_DelegateAndConstraint), null)]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_DelegateAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_DelegateAndConstraintWithMessage), "Error")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_DelegateAndConstraintWithMessageStringFunc), "Should be 4")]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_DelegateAndConstraintWithMessageStringFunc), "Should be 4")]
        [TestCase(nameof(WarningFixture.WarnUnless_Fails_Async), null)]
        [TestCase(nameof(WarningFixture.WarnIf_Fails_Async), null)]
        public void WarningFails(string methodName, string? expectedMessage)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);
            string expectedMethodName = methodName.Contains("WarnUnless") ? "Warn.Unless" : "Warn.If";

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
                Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
            });

            Assert.That(result.AssertionResults, Has.Count.EqualTo(1), "Incorrect number of AssertionResults");
            Assert.That(result.AssertionResults[0].Status, Is.EqualTo(AssertionStatus.Warning));
            string? message = result.AssertionResults[0].Message;
            Assert.That(message, Is.Not.Null, "Assertion Message should not be null");
            string? stackTrace = result.AssertionResults[0].StackTrace;
            Assert.That(stackTrace, Is.Not.Null, "StackTrace should not be null");
            Assert.That(stackTrace, Does.Contain("WarningFixture"));
            Assert.That(stackTrace.Split(new[] { '\n' }), Has.Length.LessThan(3));
            Assert.That(result.Message, Is.Not.Null, "Result Message should not be null");
            Assert.That(result.Message, Contains.Substring(message), "Result message should contain assertion message");
            Assert.That(result.Message, Contains.Substring(expectedMethodName), "Result message should contain assert method name");

            if (expectedMessage is not null)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result.Message, Does.Contain(expectedMessage));
                    Assert.That(result.AssertionResults[0].Message, Does.Contain(expectedMessage));
                });
            }
        }

        [TestCase(typeof(WarningInSetUpPasses), nameof(WarningInSetUpPasses.WarningPassesInTest), 2, 0)]
        [TestCase(typeof(WarningInSetUpPasses), nameof(WarningInSetUpPasses.WarningFailsInTest), 2, 1)]
        [TestCase(typeof(WarningInSetUpPasses), nameof(WarningInSetUpPasses.ThreeWarningsFailInTest), 4, 3)]
        [TestCase(typeof(WarningInSetUpFails), nameof(WarningInSetUpFails.WarningPassesInTest), 2, 1)]
        [TestCase(typeof(WarningInSetUpFails), nameof(WarningInSetUpFails.WarningFailsInTest), 2, 2)]
        [TestCase(typeof(WarningInSetUpFails), nameof(WarningInSetUpFails.ThreeWarningsFailInTest), 4, 4)]
        [TestCase(typeof(WarningInTearDownPasses), nameof(WarningInTearDownPasses.WarningPassesInTest), 2, 0)]
        [TestCase(typeof(WarningInTearDownPasses), nameof(WarningInTearDownPasses.WarningFailsInTest), 2, 1)]
        [TestCase(typeof(WarningInTearDownPasses), nameof(WarningInTearDownPasses.ThreeWarningsFailInTest), 4, 3)]
        [TestCase(typeof(WarningInTearDownFails), nameof(WarningInTearDownFails.WarningPassesInTest), 2, 1)]
        [TestCase(typeof(WarningInTearDownFails), nameof(WarningInTearDownFails.WarningFailsInTest), 2, 2)]
        [TestCase(typeof(WarningInTearDownFails), nameof(WarningInTearDownFails.ThreeWarningsFailInTest), 4, 4)]
        public void WarningUsedInSetUpOrTearDown(Type fixtureType, string methodName, int expectedAsserts, int expectedWarnings)
        {
            var result = TestBuilder.RunTestCase(fixtureType, methodName);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(expectedWarnings == 0 ? ResultState.Success : ResultState.Warning));
                Assert.That(result.AssertCount, Is.EqualTo(expectedAsserts), "Incorrect AssertCount");
                Assert.That(result.AssertionResults, Has.Count.EqualTo(expectedWarnings), $"There should be {expectedWarnings} AssertionResults");
            });
        }

        [Test]
        public void PassingAssertion_DoesNotCallExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
            using (new TestExecutionContext.IsolatedContext())
                Warn.Unless(0 + 1 == 1, GetExceptionMessage);

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingWarning_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
            using (new TestExecutionContext.IsolatedContext())
                Warn.Unless(1 + 1 == 1, (Func<string>)GetExceptionMessage);

            // Assert
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }

        [Test]
        public void WarnUnless_Async_Error()
        {
            var exception =
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.Unless(async () => await ThrowExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception!.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
        }

        [Test]
        public void WarnIf_Async_Error()
        {
            var exception =
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.If(async () => await ThrowExceptionGenericTask(), Is.Not.EqualTo(1)));

            Assert.That(exception!.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
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

        // We decided to trim ExecutionContext and below because ten lines per warning adds up
        // and makes it hard to read build logs.
        // See https://github.com/nunit/nunit/pull/2431#issuecomment-328404432.
        [TestCase(nameof(WarningFixture.WarningSynchronous), 1)]
        [TestCase(nameof(WarningFixture.WarningInThreadStart), 2)]
        [TestCase(nameof(WarningFixture.WarningInBeginInvoke), 5, ExcludePlatform = "mono", Reason = "Warning has no effect inside BeginInvoke on Mono")]
#if NET7_0_OR_GREATER
        [TestCase(nameof(WarningFixture.WarningInThreadPoolQueueUserWorkItem), 4, ExcludePlatform = "MacOSX")]
        [TestCase(nameof(WarningFixture.WarningInThreadPoolQueueUserWorkItem), 5, IncludePlatform = "MacOSX")]
#else
        [TestCase(nameof(WarningFixture.WarningInThreadPoolQueueUserWorkItem), 2)]
#endif
        [TestCase(nameof(WarningFixture.WarningInTaskRun), 4)]
        [TestCase(nameof(WarningFixture.WarningAfterAwaitTaskDelay), 5)]
        public static void StackTracesAreFiltered(string methodName, int maxLineCount)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);
            if (result.FailCount != 0 &&
                result.Message.StartsWith(typeof(PlatformNotSupportedException).FullName()))
            {
                return; // BeginInvoke causes PlatformNotSupportedException on .NET Core
            }

            if (result.AssertionResults.Count != 1 || result.AssertionResults[0].Status != AssertionStatus.Warning)
            {
                Assert.Fail("Expected a single warning assertion. Message: " + result.Message);
            }

            var warningStackTrace = result.AssertionResults[0].StackTrace;
            Assert.That(warningStackTrace, Is.Not.Null);
            var lines = warningStackTrace!.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (maxLineCount < lines.Length)
            {
                Assert.Fail(
                    $"Expected the number of lines to be no more than {maxLineCount}, but it was {lines.Length}:" + Environment.NewLine
                    + Environment.NewLine
                    + string.Concat(lines.Select((line, i) => $" {i + 1}. {line.Trim()}" + Environment.NewLine).ToArray())
                    + "(end)");

                // ^ Most of that is to differentiate it from the current method's stack trace
                // reported directly underneath at the same level of indentation.
            }
        }
    }
}
