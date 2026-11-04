// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    [NonParallelizable] // Disabling parallelization because the tests modify the static StackFilter.DefaultFilter property.
    public class WarningTests
    {
        // Not sure why we are testing the filter here
        // Because of that we need to be sure it is enabled to its default state.
        // The tests assembly has a `SetUpFixture` that disables the filter for debugging.
        private StackFilter _savedFilter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _savedFilter = StackFilter.DefaultFilter;
            StackFilter.DefaultFilter = new();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            StackFilter.DefaultFilter = _savedFilter;
        }

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
            Assert.That(stackTrace, Does.Contain(methodName));
            Assert.That(stackTrace.Count(x => x == '\n'), Is.EqualTo(1), "Expected only one method");
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
        public void WarnSetsCurrentResultImmediately()
        {
            TestResult result;
            using (new TestExecutionContext.IsolatedContext())
            {
                Assert.Warn("MESSAGE");
                result = TestExecutionContext.CurrentContext.CurrentResult;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
                Assert.That(result.Message, Is.EqualTo("MESSAGE"));
            }
        }

        [Test]
        public void WarningInOneTimeSetUpSetsFixtureResult()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(WarningInOneTimeSetUpWithPassingChild));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
                Assert.That(result.Message, Is.EqualTo("Warning in OneTimeSetUp"));
            }
        }

        [Test]
        public void WarningInOneTimeTearDownKeepsChildFailure()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(WarningInOneTimeTearDownAfterChildFailure));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(result.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE + Environment.NewLine + "Warning in OneTimeTearDown"));
            }
        }

        [Test]
        public void FailureInOneTimeTearDownAfterChildFailureIsReportedOnce()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(FailureInOneTimeTearDownAfterChildFailure));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure.WithSite(FailureSite.TearDown)));
                Assert.That(result.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE + Environment.NewLine + "Failure in OneTimeTearDown"));
            }
        }

        [Test]
        public void WarningsInBaseAndDerivedOneTimeTearDownAreReportedOnce()
        {
            const string derivedWarning = "Warning in derived OneTimeTearDown";
            ITestResult result = TestBuilder.RunTestFixture(typeof(WarningsInBaseAndDerivedOneTimeTearDownAfterChildFailure));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(result.Message, Does.StartWith(TestResult.CHILD_ERRORS_MESSAGE + Environment.NewLine + "Multiple failures or warnings in test:"));
                Assert.That(result.Message, Does.Contain("Warning in base OneTimeTearDown"));
                Assert.That(result.Message.IndexOf(derivedWarning, StringComparison.Ordinal),
                    Is.EqualTo(result.Message.LastIndexOf(derivedWarning, StringComparison.Ordinal)));
            }
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

            Assert.That(exception.StackTrace, Does.Contain(nameof(ThrowExceptionGenericTask)));
        }

        [Test]
        public void WarnIf_Async_Error()
        {
            var exception =
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.If(async () => await ThrowExceptionGenericTask(), Is.Not.EqualTo(1)));

            Assert.That(exception.StackTrace, Does.Contain(nameof(ThrowExceptionGenericTask)));
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
        // Remove all NUnit.Framework and System.Environment lines from the top of the stack trace,
        // and all System.Reflection, System.Runtime.Remoting, System.RuntimeMethodHandle, and System.Threading lines from the bottom of the stack trace.
        // See https://github.com/nunit/nunit/issues/5043
        // The first would stop the stack trace at Async state machine methods, hiding further methods on the stack.
        // We now skip those methods and show the rest of the stack trace.
        [TestCase(nameof(WarningFixture.WarningSynchronous))]
        [TestCase(nameof(WarningFixture.WarningInThreadStart))]
        [TestCase(nameof(WarningFixture.WarningInBeginInvoke), IncludePlatform = PlatformNames.NETFramework)]
        [TestCase(nameof(WarningFixture.WarningInThreadPoolQueueUserWorkItem))]
        [TestCase(nameof(WarningFixture.WarningInTaskRun))]
        [TestCase(nameof(WarningFixture.WarningAfterAwaitTaskYield))]
        [TestCase(nameof(WarningFixture.WarningInUserMethodAfterAwaitTaskYield), 2)]
        public static void StackTracesAreFiltered(string methodName, int expectedStackTraceLines = 1)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);
            if (result.AssertionResults.Count != 1 || result.AssertionResults[0].Status != AssertionStatus.Warning)
            {
                Assert.Fail("Expected a single warning assertion. Message: " + result.Message);
            }

            var warningStackTrace = result.AssertionResults[0].StackTrace;
            Assert.That(warningStackTrace, Is.Not.Null);
            Assert.That(warningStackTrace, Does.Contain(methodName));
            int numberOfStackTraceLines = warningStackTrace.Count(x => x == '\n');

            // On .NET Framework async method are called from the state machine, so the actual test method is not on the stack.
            // On .NET Core this is corrected by the framework, so the actual test method is on the stack.
            // The actual test method only shows up on synchronous methods.
            // Similarly on all framework using a separate Thread or Task to run the warning, the actual test method is not on the stack.
            if (warningStackTrace.Contains(nameof(StackTracesAreFiltered)))
                expectedStackTraceLines++;

            if (numberOfStackTraceLines != expectedStackTraceLines)
            {
                StringBuilder sb = new();
                sb.AppendLine($"Expected {expectedStackTraceLines} method(s) on stack, but found {numberOfStackTraceLines}.");
                sb.AppendLine("Stacktrace:");
                sb.AppendLine(warningStackTrace);
                Assert.Fail(sb.ToString());
            }

            Assert.That(numberOfStackTraceLines, Is.EqualTo(expectedStackTraceLines),
                "Mismatch in number of method(s) in stacktrace");
        }
    }
}
