// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

#if NET_4_0 || NET_4_5 || PORTABLE
using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

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
#endif