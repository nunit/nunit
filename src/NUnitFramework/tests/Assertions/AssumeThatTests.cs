// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using System.Threading.Tasks;

namespace NUnit.Framework.Assertions
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
        public void AssumptionPasses_BooleanWithMessageAndArgs()
        {
            Assume.That(2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void AssumptionPasses_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assume.That(2 + 2 == 4, getExceptionMessage);
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
        public void AssumptionPasses_BooleanLambdaWithMessageAndArgs()
        {
            Assume.That(() => 2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void AssumptionPasses_BooleanLambdaWithWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assume.That(() => 2 + 2 == 4, getExceptionMessage);
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
        public void AssumptionPasses_ActualAndConstraintWithMessageAndArgs()
        {
            Assume.That(2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assume.That(2 + 2, Is.EqualTo(4), getExceptionMessage);
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
        public void AssumptionPasses_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Assume.That(() => 2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void AssumptionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assume.That(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
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
        public void AssumptionPasses_DelegateAndConstraintWithMessageAndArgs()
        {
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => $"Not Equal to {4}";
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }

        private int ReturnsFour()
        {
            return 4;
        }

        [Test]
        public void FailureThrowsInconclusiveException_Boolean()
        {
            Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanWithMessage()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5, "message"));
            Assert.That(ex.Message, Does.Contain("message"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanWithMessageAndArgs()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5, "got {0}", 5));
            Assert.That(ex.Message, Does.Contain("got 5"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2 == 5, getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("got 5"));
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
            Assert.That(ex.Message, Does.Contain("message"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanLambdaWithMessageAndArgs()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2 == 5, "got {0}", 5));
            Assert.That(ex.Message, Does.Contain("got 5"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_BooleanLambdaWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2 == 5, getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("got 5"));
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
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2, Is.EqualTo(5), "Should be {0}", 5));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(2 + 2, Is.EqualTo(5), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
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
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2, Is.EqualTo(5), "Should be {0}", 5));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(() => 2 + 2, Is.EqualTo(5), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("Should be 5"));
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
            Assert.That(ex.Message, Does.Contain("Error"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessageAndArgs()
        {
            var ex = Assert.Throws<InconclusiveException>(
                () => Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Should be {0}", 4));
            Assert.That(ex.Message, Does.Contain("Should be 4"));
        }

        [Test]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 4";
            var ex = Assert.Throws<InconclusiveException>(
                () => Assume.That(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), getExceptionMessage));
            Assert.That(ex.Message, Does.Contain("Should be 4"));
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
            Assume.That(0 + 1 == 1, getExceptionMessage);

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssumption_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;
            Func<string> getExceptionMessage = () =>
            {
                funcWasCalled = true;
                return "Func was called";
            };

            // Act
            var ex = Assert.Throws<InconclusiveException>(() => Assume.That(1 + 1 == 1, getExceptionMessage));

            // Assert
            Assert.That(ex.Message, Does.Contain("Func was called"));
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }

        private int ReturnsFive()
        {
            return 5;
        }

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

        Assert.That(exception.StackTrace, Does.Contain("ThrowExceptionGenericTask"));
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
    }
}
