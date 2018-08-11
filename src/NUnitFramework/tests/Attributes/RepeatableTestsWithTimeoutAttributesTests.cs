// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.TestUtilities;

#if THREAD_ABORT

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class RepeatableTestsWithTimeoutAttributesTests
    {
        private int retryOnlyCount;
        private int retryTimeoutCount;
        private int repeatTimeoutCount;

        [Test]
        [Retry(3)]
        public void ShouldPassAfter3Retries()
        {
            retryOnlyCount++;
            Assert.True(retryOnlyCount >= 2);
        }

        [Test]
        [Retry(3), Timeout(30)]
        public void ShouldPassAfter3RetriesAndTimeoutIsResetEachTime()
        {
            retryTimeoutCount++;

            System.Threading.Thread.Sleep(15);
            Assert.True(retryTimeoutCount >= 2);
        }

        [Test]
        [Repeat(2), Timeout(75)]
        public void ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime()
        {
            repeatTimeoutCount++;

            System.Threading.Thread.Sleep(50);
            Assert.True(repeatTimeoutCount >= 1);
        }

        [Test]
        public void ShouldFailOnMultipleRepeatableAttributes()
        {
            var testCase = TestBuilder.MakeTestCase(GetType(), nameof(TestMethodForRepeatAndRetryExpectedFail));
            var workItem = TestBuilder.CreateWorkItem(testCase);
            var result = TestBuilder.ExecuteWorkItem(workItem);
            
            Assert.AreEqual(TestStatus.Failed, result.ResultState.Status);
            Assert.AreEqual(FailureSite.Test, result.ResultState.Site);
            Assert.AreEqual("Invalid", result.ResultState.Label);
        }

        [Test]
        public void ShouldFailOnMultipleRepeatableAttributesIncludingCustom()
        {
            var testCase = TestBuilder.MakeTestCase(GetType(), nameof(TestMethodForRepeatAndCustomRepeatExpectedFail));
            var workItem = TestBuilder.CreateWorkItem(testCase);
            var result = TestBuilder.ExecuteWorkItem(workItem);

            Assert.AreEqual(TestStatus.Failed, result.ResultState.Status);
            Assert.AreEqual(FailureSite.Test, result.ResultState.Site);
            Assert.AreEqual("Invalid", result.ResultState.Label);
        }

        [Repeat(1), Retry(1)]
        public void TestMethodForRepeatAndRetryExpectedFail() { }

        [Repeat(1), CustomRepeater]
        public void TestMethodForRepeatAndCustomRepeatExpectedFail() { }

        #region TestCustomAttribute

        internal class CustomRepeater : Attribute, IRepeatTest
        {
            public TestCommand Wrap(TestCommand command) { return null; }
        }

        #endregion
    }

    [TestFixture]
    public class BaseRepeatableTestFixture
    {
        protected int RepeatCount = 0;

        [Test, Retry(2)]
        public virtual void ShouldBeOveridden()
        {
            RepeatCount++;
            Assert.True(RepeatCount >= 1);
        }
    }

    [TestFixture]
    public class DerivedRepeatableTestFixture : BaseRepeatableTestFixture
    {
        [Test, Retry(3)]
        public override void ShouldBeOveridden()
        {
            RepeatCount++;
            Assert.True(RepeatCount >= 2);
        }
    }

    [TestFixture]
    public class DerivedRepeatableTestFixtureWithNoRepeat : BaseRepeatableTestFixture
    {
        [Test]
        public override void ShouldBeOveridden()
        {
            RepeatCount++;
            Assert.True(RepeatCount == 1);
        }
    }
}

#endif
