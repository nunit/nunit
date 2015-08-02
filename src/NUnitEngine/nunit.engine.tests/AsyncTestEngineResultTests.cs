using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Tests
{
    public class AsyncTestEngineResultTests
    {
        [Test]
        public void Result_ThrowsIfNotSet()
        {
            var asyncResult = new AsyncTestEngineResult();
            TestEngineResult result = null;
            Assert.Throws<InvalidOperationException>(() => result = asyncResult.Result);
        }

        [Test]
        public void SetResult_ThrowsIfNull()
        {
            var asyncResult = new AsyncTestEngineResult();
            Assert.Throws<ArgumentNullException>(() => asyncResult.SetResult(null));
        }

        [Test]
        public void SetResult_ThrowsIfSetTwice()
        {
            var asyncResult = new AsyncTestEngineResult();
            var result = new TestEngineResult();
            asyncResult.SetResult(result);
            Assert.Throws<InvalidOperationException>(() => asyncResult.SetResult(result));
        }

        [Test]
        public void IsComplete_FalseIfNotComplete()
        {
            var asyncResult = new AsyncTestEngineResult();
            Assert.IsFalse(asyncResult.IsComplete);
        }

        [Test]
        public void IsComplete_TrueIfComplete()
        {
            var asyncResult = new AsyncTestEngineResult();
            var result = new TestEngineResult();
            asyncResult.SetResult(result);
            Assert.IsTrue(asyncResult.IsComplete);
        }

        [Test]
        public void Wait_ReturnsFalseTillTestCompletes()
        {
            var asyncResult = new AsyncTestEngineResult();
            var result = new TestEngineResult();

            Assert.IsFalse(asyncResult.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be false because test hasn't completed yet");

            asyncResult.SetResult(result);

            Assert.IsTrue(asyncResult.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be true because the test is complete");

            Assert.AreEqual(result, asyncResult.Result);
        }

        [Test]
        public void Wait_AllowsMultipleWaits()
        {
            var asyncResult = new AsyncTestEngineResult();
            asyncResult.SetResult(new TestEngineResult());
            
            Assert.IsTrue(asyncResult.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be true because the test is complete");

            Assert.IsTrue(asyncResult.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected the second wait to be non blocking");
        }

    }
}
