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
using NUnit.Framework.Constraints;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif

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

#if !NET20
        [Test]
        public void AssumptionPasses_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
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
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assume.That(() => 2 + 2 == 4, getExceptionMessage);
        }
#endif

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

#if !NET20
        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
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
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assume.That(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
        }
#endif

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

#if !NET20
        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Assume.That(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }
#endif

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

#if !NET20
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
#endif

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

#if !NET20
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
#endif

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

#if !NET20
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
#endif

        private int ReturnsFive()
        {
            return 5;
        }

#if ASYNC
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
#if NET45
            var exception = 
#endif
            Assert.Throws<InvalidOperationException>(() =>
                Assume.That(async () => await ThrowExceptionGenericTask(), Is.EqualTo(1)));

#if NET45
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
