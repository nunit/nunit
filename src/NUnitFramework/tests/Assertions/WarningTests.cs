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
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class WarningTests
    {
        [TestCase("WarningPasses_Boolean")]
        [TestCase("WarningPasses_BooleanWithMessage")]
        [TestCase("WarningPasses_BooleanWithMessageAndArgs")]
#if !NET_2_0
        [TestCase("WarningPasses_BooleanWithMessageStringFunc")]
        [TestCase("WarningPasses_BooleanLambda")]
        [TestCase("WarningPasses_BooleanLambdaWithMessage")]
        [TestCase("WarningPasses_BooleanLambdaWithMessageAndArgs")]
        [TestCase("WarningPasses_BooleanLambdaWithWithMessageStringFunc")]
#endif
        [TestCase("WarningPasses_ActualAndConstraint")]
        [TestCase("WarningPasses_ActualAndConstraintWithMessage")]
        [TestCase("WarningPasses_ActualAndConstraintWithMessageAndArgs")]
#if !NET_2_0
        [TestCase("WarningPasses_ActualAndConstraintWithMessageStringFunc")]
        [TestCase("WarningPasses_ActualLambdaAndConstraint")]
        [TestCase("WarningPasses_ActualLambdaAndConstraintWithMessage")]
        [TestCase("WarningPasses_ActualLambdaAndConstraintWithMessageAndArgs")]
        [TestCase("WarningPasses_ActualLambdaAndConstraintWithMessageStringFunc")]
#endif
        [TestCase("WarningPasses_DelegateAndConstraint")]
        [TestCase("WarningPasses_DelegateAndConstraintWithMessage")]
        [TestCase("WarningPasses_DelegateAndConstraintWithMessageAndArgs")]
#if !NET_2_0
        [TestCase("WarningPasses_DelegateAndConstraintWithMessageStringFunc")]
#endif
        public void WarningPasses(string methodName)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(0), "There should be no AssertionResults");
        }

        [TestCase("WarnUnless_Boolean", null)]
        [TestCase("WarnUnless_BooleanWithMessage", "message")]
        [TestCase("WarnUnless_BooleanWithMessageAndArgs", "got 5")]
#if !NET_2_0
        [TestCase("WarnUnless_BooleanWithMessageStringFunc", "got 5")]
        [TestCase("WarnUnless_BooleanLambda", null)]
        [TestCase("WarnUnless_BooleanLambdaWithMessage", "message")]
        [TestCase("WarnUnless_BooleanLambdaWithMessageAndArgs", "got 5")]
        [TestCase("WarnUnless_BooleanLambdaWithMessageStringFunc", "got 5")]
#endif
        [TestCase("WarnUnless_ActualAndConstraint", null)]
        [TestCase("WarnUnless_ActualAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_ActualAndConstraintWithMessageAndArgs", "Should be 5")]
#if !NET_2_0
        [TestCase("WarnUnless_ActualAndConstraintWithMessageStringFunc", "Should be 5")]
        [TestCase("WarnUnless_ActualLambdaAndConstraint", null)]
        [TestCase("WarnUnless_ActualLambdaAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_ActualLambdaAndConstraintWithMessageAndArgs", "Should be 5")]
        [TestCase("WarnUnless_ActualLambdaAndConstraintWithMessageStringFunc", "Should be 5")]
#endif
        [TestCase("WarnUnless_DelegateAndConstraint", null)]
        [TestCase("WarnUnless_DelegateAndConstraintWithMessage", "Error")]
        [TestCase("WarnUnless_DelegateAndConstraintWithMessageAndArgs", "Should be 4")]
#if !NET_2_0
        [TestCase("WarnUnless_DelegateAndConstraintWithMessageStringFunc", "Should be 4")]
#endif
#if ASYNC
        [TestCase("WarningFails_Async", null)]
#endif
        public void FailureIssuesWarning(string methodName, string expectedMessage)
        {
            var result = TestBuilder.RunTestCase(typeof(WarningFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
            Assert.That(result.AssertCount, Is.EqualTo(1), "Incorrect AssertCount");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(1), "Incorrect number of AssertionResults");
            Assert.That(result.AssertionResults[0].Status, Is.EqualTo(AssertionStatus.Warning));
            Assert.That(result.AssertionResults[0].Message, Is.Not.Null, "Assertion Message should not be null");
            Assert.That(result.Message, Is.Not.Null, "Result Message should not be null");
            Assert.That(result.Message, Contains.Substring(result.AssertionResults[0].Message), "Result message should contain assertion message");
#if !PORTABLE
            Assert.That(result.AssertionResults[0].StackTrace, Does.Contain("WarningFixture"));
#endif

            if (expectedMessage != null)
            {
                Assert.That(result.Message, Does.Contain(expectedMessage));
                Assert.That(result.AssertionResults[0].Message, Does.Contain(expectedMessage));
            }

        }

#if !NET_2_0
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
        public void WarnPasses_Async()
        {
            Warn.Unless(async () => await One(), Is.EqualTo(1));
        }

        [Test]
        public void WarnUnless_Error()
        {
#if NET_4_5
            var exception =
#endif
                Assert.Throws<InvalidOperationException>(() =>
                    Warn.Unless(async () => await ThrowExceptionGenericTask(), Is.EqualTo(1)));

#if NET_4_5
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
    }
}
