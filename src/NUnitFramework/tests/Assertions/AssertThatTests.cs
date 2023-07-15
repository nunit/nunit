// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertThatTests
    {
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
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

        [Test]
        public void AssertionPasses_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assert.That(2 + 2 == 4, getExceptionMessage);
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

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

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
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
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assert.That(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
        }

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

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assert.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }

        private int ReturnsFour()
        {
            return 4;
        }
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

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

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("Not Equal to 4"));
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

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

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("error"));
        }

        [Test]
        public void AssertionsAreCountedCorrectly()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(AssertCountFixture));

            int totalCount = 0;
            foreach (var childResult in result.Children)
            {
                int expectedCount = childResult.Name == "ThreeAsserts" ? 3 : 1;
                Assert.That(childResult.AssertCount, Is.EqualTo(expectedCount), "Bad count for {0}", childResult.Name);
                totalCount += expectedCount;
            }

            Assert.That(result.AssertCount, Is.EqualTo(totalCount), "Fixture count is not correct");
        }

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

        private int ReturnsFive()
        {
            return 5;
        }

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

        [Test, Platform(Exclude = "Linux", Reason = "Intermittent failures on Linux")]
        public void AssertThatErrorTask()
        {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionTask(), Is.EqualTo(1)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint

            Assert.That(exception.StackTrace, Does.Contain("ThrowInvalidOperationExceptionTask"));
        }

        [Test]
        public void AssertThatErrorGenericTask()
        {
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
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

        private static async Task ThrowInvalidOperationExceptionTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

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
    }
}
