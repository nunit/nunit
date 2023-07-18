// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

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
        public void AssertionPasses_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(2 + 2 == 4, GetExceptionMessage);
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
        public void AssertionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(2 + 2, Is.EqualTo(4), GetExceptionMessage);
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
        public void AssertionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(() => 2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraint()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessage()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(ReturnsFour, Is.EqualTo(4), GetExceptionMessage);
        }

        private int ReturnsFour() => 4;

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
            Assert.That(ex?.Message, Does.Contain("message"));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Not Equal to 4"));
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
            Assert.That(ex?.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
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
            Assert.That(ex?.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4)));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
        }

        [Test]
        public void AssertionsAreCountedCorrectly()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(AssertCountFixture));

            int totalCount = 0;
            foreach (var childResult in result.Children)
            {
                int expectedCount = childResult.Name == "ThreeAsserts" ? 3 : 1;
                Assert.That(childResult.AssertCount, Is.EqualTo(expectedCount), $"Bad count for {childResult.Name}");
                totalCount += expectedCount;
            }

            Assert.That(result.AssertCount, Is.EqualTo(totalCount), "Fixture count is not correct");
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
#pragma warning disable NUnit2045 // Use Assert.Multiple
            Assert.That(0 + 1 == 1, GetExceptionMessage);
#pragma warning restore NUnit2045 // Use Assert.Multiple

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssertion_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
            var ex = Assert.Throws<AssertionException>(() => Assert.That(1 + 1 == 1, GetExceptionMessage));

            // Assert
            Assert.That(ex?.Message, Does.Contain("Func was called"));
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

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionTask"));
        }

        [Test]
        public void AssertThatErrorGenericTask()
        {
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
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
            Assert.That(ex?.Message, Does.Contain("Error"));
        }
    }
}
