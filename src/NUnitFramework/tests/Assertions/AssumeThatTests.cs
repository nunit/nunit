// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssumeThatTests
    {
        [Test]
        public void AssumptionPasses_Boolean()
        {
            Assume.That(2 + 2 == 4);
        }

        [Test]
        public void AssumptionPasses_BooleanWithMessage()
        {
            Assume.That(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssumptionPasses_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to 4";
            Assume.That(2 + 2 == 4, (Func<string>)GetExceptionMessage);
        }

        [Test]
        public void AssumptionPasses_BooleanLambda()
        {
            Assume.That(() => 2 + 2 == 4);
        }

        [Test]
        public void AssumptionPasses_BooleanLambdaWithMessage()
        {
            Assume.That(() => 2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssumptionPasses_BooleanLambdaWithWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to 4";
            Assume.That(() => 2 + 2 == 4, (Func<string>)GetExceptionMessage);
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraint()
        {
            Assume.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessage()
        {
            Assume.That(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to 4";
            Assume.That(2 + 2, Is.EqualTo(4), (Func<string>)GetExceptionMessage);
        }

        [Test]
        public void AssumptionPasses_ActualLambdaAndConstraint()
        {
            Assume.That(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssumptionPasses_ActualLambdaAndConstraintWithMessage()
        {
            Assume.That(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssumptionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assume.That(() => 2 + 2, Is.EqualTo(4), (Func<string>)GetExceptionMessage);
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraint()
        {
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4));
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessage()
        {
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), (Func<string>)GetExceptionMessage);
        }

        private int ReturnsFour() => 4;

        [Test]
        public void FailureThrowsInconclusiveException_Boolean()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5, "message"));
            Assert.That(ex?.Message, Does.Contain("message"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5, GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("got 5"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanLambda()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanLambdaWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2 == 5, "message"));
            Assert.That(ex?.Message, Does.Contain("message"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(() => 2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanLambdaWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2 == 5, GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("got 5"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(() => 2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualAndConstraint()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Should be 5"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualLambdaAndConstraint()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualLambdaAndConstraintWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Should be 5"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_DelegateAndConstraint()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4)));
        }

        [Test]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4))"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 4";
            var ex = Assert.Throws<InconclusiveException>(
                () => Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Should be 4"));
            Assert.That(ex?.Message, Does.Contain("Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4))"));
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
            Assume.That(0 + 1 == 1, GetExceptionMessage);

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssumption_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(1 + 1 == 1, GetExceptionMessage));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(ex?.Message, Does.Contain("Func was called"));
                Assert.That(ex?.Message, Does.Contain("Assume.That(1 + 1 == 1, Is.True)"));
                Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
            });
        }

        private int ReturnsFive() => 5;

        [Test]
        public void AssumeThatSuccess()
        {
            Assume.That(async () => await One(), Is.EqualTo(1));
        }

        [Test]
        public void AssumeThatFailure()
        {
            Assert.Throws<InconclusiveException>(() =>
                Assume.That(async () => await One(), Is.EqualTo(2)));
        }

        [Test]
        public void AssumeThatError()
        {
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assume.That(async () => await ThrowExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception?.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
        }

        private static Task<int> One() => Task.Run(() => 1);

        private static async Task<int> ThrowExceptionGenericTask()
        {
            await One();
            throw new InvalidOperationException();
        }
    }
}
