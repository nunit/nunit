// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt



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

            Assert.AreEqual(1, result);
        }

#region async Task

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskSuccess()
        {
            var result = await ReturnOne();

            Assert.AreEqual(1, result);
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskFailure()
        {
            var result = await ReturnOne();

            Assert.AreEqual(2, result);
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskError()
        {
            await ThrowException();

            Assert.Fail("Should never get here");
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskPass()
        {
            Assert.Pass();
            throw new Exception("This test expects Assert.Pass() to throw an exception.");
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskIgnore()
        {
            Assert.Ignore();
            throw new Exception("This test expects Assert.Ignore() to throw an exception.");
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskInconclusive()
        {
            Assert.Inconclusive();
            throw new Exception("This test expects Assert.Inconclusive() to throw an exception.");
        }

        #endregion

        #region non-async Task

        [Test]
        public System.Threading.Tasks.Task TaskSuccess()
        {
            return Task.Run(() => Assert.AreEqual(1, 1));
        }

        [Test]
        public System.Threading.Tasks.Task TaskFailure()
        {
            return Task.Run(() => Assert.AreEqual(1, 2));
        }

        [Test]
        public System.Threading.Tasks.Task TaskError()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public System.Threading.Tasks.Task TaskPass()
        {
            Assert.Pass();
            throw new Exception("This test expects Assert.Pass() to throw an exception.");
        }

        [Test]
        public System.Threading.Tasks.Task TaskIgnore()
        {
            Assert.Ignore();
            throw new Exception("This test expects Assert.Ignore() to throw an exception.");
        }

        [Test]
        public System.Threading.Tasks.Task TaskInconclusive()
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
        public async System.Threading.Tasks.Task AsyncTaskTestCaseWithParametersSuccess(int a, int b)
        {
            Assert.AreEqual(await ReturnOne(), b - a);
        }

        [TestCase(ExpectedResult = null)]
        public async Task<object> AsyncTaskResultCheckSuccessReturningNull()
        {
            return await Task.Run(() => (object)null);
        }

        [TestCase(ExpectedResult = null)]
        public Task<object> TaskResultCheckSuccessReturningNull()
        {
            return Task.Run(() => (object)null);
        }

        [Test]
        public async System.Threading.Tasks.Task NestedAsyncTaskSuccess()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.AreEqual(1, result);
        }

        [Test]
        public async System.Threading.Tasks.Task NestedAsyncTaskFailure()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.AreEqual(2, result);
        }

        [Test]
        public async System.Threading.Tasks.Task NestedAsyncTaskError()
        {
            await Task.Run(async () => await ThrowException());

            Assert.Fail("Should never get here");
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskMultipleSuccess()
        {
            var result = await ReturnOne();

            Assert.AreEqual(await ReturnOne(), result);
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskMultipleFailure()
        {
            var result = await ReturnOne();

            Assert.AreEqual(await ReturnOne() + 1, result);
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTaskMultipleError()
        {
            await ThrowException();

            Assert.Fail("Should never get here");
        }

        [Test]
        public async System.Threading.Tasks.Task TaskCheckTestContextAcrossTasks()
        {
            var testName = await GetTestNameFromContext();

            Assert.IsNotNull(testName);
            Assert.AreEqual(testName, TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public async System.Threading.Tasks.Task TaskCheckTestContextWithinTestBody()
        {
            var testName = TestContext.CurrentContext.Test.Name;

            await ReturnOne();

            Assert.IsNotNull(testName);
            Assert.AreEqual(testName, TestContext.CurrentContext.Test.Name);
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
            return Task.Run( throws );
        }
    }
}
