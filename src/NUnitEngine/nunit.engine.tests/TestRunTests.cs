using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace NUnit.Engine.Tests
{
    public class TestRunTests
    {
        [Test]
        public void Result_ThrowsIfNotSet()
        {
            var testRun = new TestRun(null);
            XmlNode result = null;
            Assert.Throws<InvalidOperationException>(() => result = testRun.Result);
        }

        [Test]
        public void SetResult_ThrowsIfNull()
        {
            var testRun = new TestRun(null);
            Assert.Throws<ArgumentNullException>(() => testRun.SetResult(null));
        }

        [Test]
        public void SetResult_ThrowsIfSetTwice()
        {
            var testRun = new TestRun(null);
            var result = new TestEngineResult();
            testRun.SetResult(result);
            Assert.Throws<InvalidOperationException>(() => testRun.SetResult(result));
        }

        [Test]
        public void IsComplete_FalseIfNotComplete()
        {
            var testRun = new TestRun(null);
            Assert.IsFalse(testRun.IsComplete);
        }

        [Test]
        public void IsComplete_TrueIfComplete()
        {
            var testRun = new TestRun(null);
            var result = new TestEngineResult();
            testRun.SetResult(result);
            Assert.IsTrue(testRun.IsComplete);
        }

        [Test]
        public void Wait_ReturnsFalseTillTestCompletes()
        {
            var testRun = new TestRun(null);
            var result = new TestEngineResult("<test-assembly />");

            Assert.IsFalse(testRun.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be false because test hasn't completed yet");

            testRun.SetResult(result);

            Assert.IsTrue(testRun.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be true because the test is complete");

            Assert.AreEqual(result.Xml, testRun.Result);
        }

        [Test]
        public void Wait_AllowsMultipleWaits()
        {
            var testRun = new TestRun(null);
            testRun.SetResult(new TestEngineResult());
            
            Assert.IsTrue(testRun.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected wait to be true because the test is complete");

            Assert.IsTrue(testRun.Wait(TimeSpan.FromMilliseconds(0)),
                "Expected the second wait to be non blocking");
        }

    }
}
