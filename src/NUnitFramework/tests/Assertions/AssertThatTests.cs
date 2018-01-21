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

using NUnit.Framework.Interfaces;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

#if ASYNC
using System;
using System.Threading.Tasks;
#endif

#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.Framework.Assertions
{
    using System;

    [TestFixture]
    public class AssertThatTests
    {
        [Test]
        public void AssertionPasses_Boolean()
        {
            Assert.That(2 + 2 == 4);
        }

        [Test]
        public void AssertionPasses_BooleanWithMessage()
        {
            Assert.That(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssertionPasses_BooleanWithMessageAndArgs()
        {
            Assert.That(2 + 2 == 4, "Not Equal to {0}", 4);
        }

#if !NET20
        [Test]
        public void AssertionPasses_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assert.That(2 + 2 == 4, getExceptionMessage);
        }
#endif

        [Test]
        public void AssertionPasses_ActualAndConstraint()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessage()
        {
            Assert.That(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessageAndArgs()
        {
            Assert.That(2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

#if !NET20
        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assert.That(2 + 2, Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraint()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessage()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assert.That(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
        }
#endif

        [Test]
        public void AssertionPasses_DelegateAndConstraint()
        {
            Assert.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessage()
        {
            Assert.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageAndArgs()
        {
            Assert.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Should be {0}", 4);
        }

#if !NET20
        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assert.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }
#endif

        private int ReturnsFour()
        {
            return 4;
        }

        [Test]
        public void FailureThrowsAssertionException_Boolean()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, "message"));
            Assert.That(ex.Message, Does.Contain("message"));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageAndArgs()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, "got {0}", 5));
            Assert.That(ex.Message, Does.Contain("got 5"));
        }

#if !NET20
        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("Not Equal to 4"));
        }
#endif

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), "Should be {0}", 5));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
        }

#if !NET20
        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("error"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), "Should be {0}", 5));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("error"));
        }
#endif

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4)));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Error"));
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Should be {0}", 4));
            Assert.That(ex.Message, Does.Contain("Should be 4"));
        }

#if !NET20
        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("error"));
        }
#endif

        [Test]
        public void AssertionsAreCountedCorrectly()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(AssertCountFixture));

            int totalCount = 0;
            foreach (TestResult childResult in result.Children)
            {
                int expectedCount = childResult.Name == "ThreeAsserts" ? 3 : 1;
                Assert.That(childResult.AssertCount, Is.EqualTo(expectedCount), "Bad count for {0}", childResult.Name);
                totalCount += expectedCount;
            }

            Assert.That(result.AssertCount, Is.EqualTo(totalCount), "Fixture count is not correct");
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
            Assert.That(0 + 1 == 1, getExceptionMessage);

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssertion_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;
            Func<string> getExceptionMessage = () =>
                {
                    funcWasCalled = true;
                    return "Func was called";
                };

            // Act
            var ex = Assert.Throws<AssertionException>(() => Assert.That(1 + 1 == 1, getExceptionMessage));
            
            // Assert
            Assert.That(ex.Message, Does.Contain("Func was called"));
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }
#endif

        private int ReturnsFive()
        {
            return 5;
        }

#if ASYNC
        [Test]
        public void AssertThatSuccess()
        {
            Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(1));
        }

        [Test]
        public void AssertThatFailure()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(2)));
        }

#if PLATFORM_DETECTION
        [Test, Platform(Exclude="Linux", Reason="Intermittent failures on Linux")]
        public void AssertThatErrorTask()
        {
#if NET45
            var exception = 
#endif
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionTask(), Is.EqualTo(1)));

#if NET45
            Assert.That(exception.StackTrace, Does.Contain("ThrowInvalidOperationExceptionTask"));
#endif
        }
#endif

        [Test]
        public void AssertThatErrorGenericTask()
        {
#if NET45
            var exception = 
#endif
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionGenericTask(), Is.EqualTo(1)));

#if NET45
        Assert.That(exception.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
#endif
        }

        [Test]
        public void AssertThatErrorVoid()
        {
#if NET45
            var exception = 
#endif
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => { await ThrowInvalidOperationExceptionGenericTask(); }, Is.EqualTo(1)));

#if NET45
        Assert.That(exception.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
#endif
        }

        private static Task<int> AsyncReturnOne()
        {
            return Task.Run(() => 1);
        }

        private static async Task<int> ThrowInvalidOperationExceptionGenericTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

        private static async System.Threading.Tasks.Task ThrowInvalidOperationExceptionTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }
#endif

#if !NET20
        [Test]
        public void AssertThatWithLambda()
        {
            Assert.That(() => true);
        }

        [Test]
        public void AssertThatWithFalseLambda()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => false, "Error"));
            Assert.That(ex.Message, Does.Contain("Error"));
        }
#endif
    }
}
