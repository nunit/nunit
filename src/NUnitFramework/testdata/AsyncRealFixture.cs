// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework;

// Don’t warn when async is used without await
#pragma warning disable CS1998

namespace NUnit.TestData
{
    public class AsyncRealFixture
    {
        [Test]
        public async void AsyncVoid()
        {
            var result = await ReturnOne();

            Assert.That(result, Is.EqualTo(1));
        }

        #region async Task

        [Test]
        public async Task AsyncTaskSuccess()
        {
            var result = await ReturnOne();

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task AsyncTaskFailure()
        {
            var result = await ReturnOne();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task AsyncTaskError()
        {
            await ThrowException();

            Assert.Fail("Should never get here");
        }

        [Test]
        public async Task AsyncTaskPass()
        {
            Assert.Pass();
            throw new Exception("This test expects Assert.Pass() to throw an exception.");
        }

        [Test]
        public async Task AsyncTaskIgnore()
        {
            Assert.Ignore();
            throw new Exception("This test expects Assert.Ignore() to throw an exception.");
        }

        [Test]
        public async Task AsyncTaskInconclusive()
        {
            Assert.Inconclusive();
            throw new Exception("This test expects Assert.Inconclusive() to throw an exception.");
        }

        #endregion

        #region non-async Task

        [Test]
        public Task TaskSuccess()
        {
            return Task.Run(() => Assert.That(1, Is.EqualTo(1)));
        }

        [Test]
        public Task TaskFailure()
        {
            return Task.Run(() => Assert.That(2, Is.EqualTo(1)));
        }

        [Test]
        public Task TaskError()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public Task TaskPass()
        {
            Assert.Pass();
            throw new Exception("This test expects Assert.Pass() to throw an exception.");
        }

        [Test]
        public Task TaskIgnore()
        {
            Assert.Ignore();
            throw new Exception("This test expects Assert.Ignore() to throw an exception.");
        }

        [Test]
        public Task TaskInconclusive()
        {
            Assert.Inconclusive();
            throw new Exception("This test expects Assert.Inconclusive() to throw an exception.");
        }

        #endregion

        [Test]
        public async Task<int> AsyncTaskResult()
        {
            return await ReturnOne();
        }

        [Test]
        public Task<int> TaskResult()
        {
            return ReturnOne();
        }

        #region async Task<T>

        [TestCase(ExpectedResult = 1)]
        public async Task<int> AsyncTaskResultCheckSuccess()
        {
            return await ReturnOne();
        }

        [TestCase(ExpectedResult = 2)]
        public async Task<int> AsyncTaskResultCheckFailure()
        {
            return await ReturnOne();
        }

        [TestCase(ExpectedResult = 0)]
        public async Task<int> AsyncTaskResultCheckError()
        {
            return await ThrowException();
        }

        #endregion

        #region non-async Task<T>

        [TestCase(ExpectedResult = 1)]
        public Task<int> TaskResultCheckSuccess()
        {
            return ReturnOne();
        }

        [TestCase(ExpectedResult = 2)]
        public Task<int> TaskResultCheckFailure()
        {
            return ReturnOne();
        }

        [TestCase(ExpectedResult = 0)]
        public Task<int> TaskResultCheckError()
        {
            return ThrowException();
        }

        #endregion

        [TestCase(1, 2)]
        public async Task AsyncTaskTestCaseWithParametersSuccess(int a, int b)
        {
            Assert.That(b - a, Is.EqualTo(await ReturnOne()));
        }

        [TestCase(ExpectedResult = null)]
        public async Task<object?> AsyncTaskResultCheckSuccessReturningNull()
        {
            return await Task.Run(() => (object?)null);
        }

        [TestCase(ExpectedResult = null)]
        public Task<object?> TaskResultCheckSuccessReturningNull()
        {
            return Task.Run(() => (object?)null);
        }

        [Test]
        public async Task NestedAsyncTaskSuccess()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task NestedAsyncTaskFailure()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task NestedAsyncTaskError()
        {
            await Task.Run(async () => await ThrowException());

            Assert.Fail("Should never get here");
        }

        [Test]
        public async Task AsyncTaskMultipleSuccess()
        {
            var result = await ReturnOne();

            Assert.That(result, Is.EqualTo(await ReturnOne()));
        }

        [Test]
        public async Task AsyncTaskMultipleFailure()
        {
            var result = await ReturnOne();

            Assert.That(result, Is.EqualTo(await ReturnOne() + 1));
        }

        [Test]
        public async Task AsyncTaskMultipleError()
        {
            await ThrowException();

            Assert.Fail("Should never get here");
        }

        [Test]
        public async Task TaskCheckTestContextAcrossTasks()
        {
            var testName = await GetTestNameFromContext();

            Assert.That(testName, Is.Not.Null);
            Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo(testName));
        }

        [Test]
        public async Task TaskCheckTestContextWithinTestBody()
        {
            var testName = TestContext.CurrentContext.Test.Name;

            await ReturnOne();

            Assert.That(testName, Is.Not.Null);
            Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo(testName));
        }

        private static Task<string> GetTestNameFromContext()
        {
            return Task.Run(() => TestContext.CurrentContext.Test.Name);
        }

        private static Task<int> ReturnOne()
        {
            return Task.Run(() => 1);
        }

        private static Task<int> ThrowException()
        {
            Func<int> throws = () => { throw new InvalidOperationException(); };
            return Task.Run(throws);
        }
    }
}
