#if NET_4_0 || NET_4_5
using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

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

        [Test] // Not Runnable
        public async Task<int> AsyncTaskResultSuccess()
        {
            var result = await ReturnOne();

            Assert.AreEqual(1, result);

            return result;
        }

        [Test] // Not Runnable
        public async Task<int> AsyncTaskResultFailure()
        {
            var result = await ReturnOne();

            Assert.AreEqual(2, result);

            return result;
        }

        [Test] // Not Runnable
        public async Task<int> AsyncTaskResultError()
        {
            await ThrowException();

            Assert.Fail("Should never get here");

            return 0;
        }

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

        [TestCase(ExpectedResult = null)]
        public async Task<object> AsyncTaskResultCheckSuccessReturningNull()
        {
            return await Task.Run(() => (object)null);
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
        public async Task<int> NestedAsyncTaskResultSuccess()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.AreEqual(1, result);

            return result;
        }

        [Test]
        public async Task<int> NestedAsyncTaskResultFailure()
        {
            var result = await Task.Run(async () => await ReturnOne());

            Assert.AreEqual(2, result);

            return result;
        }

        [Test]
        public async Task<int> NestedAsyncTaskResultError()
        {
            var result = await Task.Run(async () => await ThrowException());

            Assert.Fail("Should never get here");

            return result;
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
            var result = await ReturnOne();
            await ThrowException();

            Assert.Fail("Should never get here");
        }

        [TestCase(1, 2)]
        public async System.Threading.Tasks.Task AsyncTaskTestCaseWithParametersSuccess(int a, int b)
        {
            Assert.AreEqual(await ReturnOne(), b - a);
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

        //[Test]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public async void VoidAsyncVoidChildCompletingEarlierThanTest()
        //{
        //    AsyncVoidMethod();

        //    await ThrowExceptionIn(TimeSpan.FromSeconds(1));
        //}

        //[Test]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public async void VoidAsyncVoidChildThrowingImmediately()
        //{
        //    AsyncVoidThrowException();

        //    await Task.Run(() => Assert.Fail("Should never invoke this"));
        //}

        //private static async void AsyncVoidThrowException()
        //{
        //    await Task.Run(() => { throw new InvalidOperationException(); });
        //}

        //private static async Task ThrowExceptionIn(TimeSpan delay)
        //{
        //    await Task.Delay(delay);
        //    throw new InvalidOperationException();
        //}

        //private static async void AsyncVoidMethod()
        //{
        //    await Task.Yield();
        //}

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
#endif
