// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AsyncThrowsTests
    {
        private readonly TestDelegate _asyncVoid = new TestDelegate(async () => await Task.Delay(1));
        private readonly ActualValueDelegate<System.Threading.Tasks.Task> _noThrowsAsyncTask = async () => await Task.Delay(1);
        private readonly ActualValueDelegate<Task<int>> _noThrowsAsyncGenericTask = async () => await ReturnOne();
        private readonly ActualValueDelegate<System.Threading.Tasks.Task> _throwsAsyncTask = async () => await ThrowAsyncTask();
        private readonly ActualValueDelegate<Task<int>> _throwsAsyncGenericTask = async () => await ThrowAsyncGenericTask();

        private static ThrowsConstraint ThrowsInvalidOperationExceptionConstraint
        {
            get { return new ThrowsConstraint(new ExactTypeConstraint(typeof(InvalidOperationException))); }
        }

        [Test]
        public void ThrowsConstraintAsyncTask()
        {
            Assert.That(ThrowsInvalidOperationExceptionConstraint.ApplyTo(_throwsAsyncTask).IsSuccess);
        }

        [Test]
        public void ThrowsConstraintAsyncGenericTask()
        {
            Assert.That(ThrowsInvalidOperationExceptionConstraint.ApplyTo(_throwsAsyncGenericTask).IsSuccess);
        }

        [Test]
        public void ThrowsConstraintAsyncVoid()
        {
            Assert.That(() => ThrowsInvalidOperationExceptionConstraint.ApplyTo(_asyncVoid), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsVoidIsAnError()
        {
            Assert.That(() => Assert.That(_asyncVoid, Throws.Nothing), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsNothingConstraintTaskVoidSuccess()
        {
            Assert.That(new ThrowsNothingConstraint().ApplyTo(_noThrowsAsyncTask).IsSuccess);
        }

        [Test]
        public void ThrowsNothingConstraintTaskFailure()
        {
            Assert.That(new ThrowsNothingConstraint().ApplyTo(_throwsAsyncTask).Status, Is.EqualTo(ConstraintStatus.Failure));
        }
        
        [Test]
        public void AssertThatThrowsTask()
        {
            Assert.That(_throwsAsyncTask, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void AssertThatThrowsGenericTask()
        {
            Assert.That(_throwsAsyncGenericTask, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void AssertThatThrowsNothingTaskSuccess()
        {
            Assert.That(_noThrowsAsyncTask, Throws.Nothing);
        }

        [Test]
        public void AssertThatThrowsNothingGenericTaskSuccess()
        {
            Assert.That(_noThrowsAsyncGenericTask, Throws.Nothing);
        }

        [Test]
        public void AssertThatThrowsNothingTaskFailure()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_throwsAsyncTask, Throws.Nothing));
        }

        [Test]
        public void AssertThatThrowsNothingGenericTaskFailure()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_throwsAsyncGenericTask, Throws.Nothing));
        }

        private static async System.Threading.Tasks.Task ThrowAsyncTask()
        {
            await ReturnOne();
            throw new InvalidOperationException();
        }

        private static async System.Threading.Tasks.Task<int> ThrowAsyncGenericTask()
        {
            await ThrowAsyncTask();
            return await ReturnOne();
        }

        private static System.Threading.Tasks.Task<int> ReturnOne()
        {
            return Task.Run(() => 1);
        }
    }
}
