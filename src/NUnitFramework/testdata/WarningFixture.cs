// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.TestData
{
    [TestFixture]
    public class WarningFixture
    {
        #region Passing Tests

        [Test]
        public void WarnUnless_Passes_Boolean()
        {
            Warn.Unless(2 + 2 == 4);
        }

        [Test]
        public void WarnIf_Passes_Boolean()
        {
            Warn.If(2 + 2 != 4);
        }

        [Test]
        public void WarnUnless_Passes_BooleanWithMessage()
        {
            Warn.Unless(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void WarnIf_Passes_BooleanWithMessage()
        {
            Warn.If(2 + 2 != 4, "Not Equal");
        }

        [Test]
        public void WarnUnless_Passes_BooleanWithMessageAndArgs()
        {
            Warn.Unless(2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void WarnIf_Passes_BooleanWithMessageAndArgs()
        {
            Warn.If(2 + 2 != 4, "Not Equal to {0}", 4);
        }

#if !NET20
        [Test]
        public void WarnUnless_Passes_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(2 + 2 == 4, getExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.If(2 + 2 != 4, getExceptionMessage);
        }

        [Test]
        public void WarnUnless_Passes_BooleanLambda()
        {
            Warn.Unless(() => 2 + 2 == 4);
        }

        [Test]
        public void WarnIf_Passes_BooleanLambda()
        {
            Warn.If(() => 2 + 2 != 4);
        }

        [Test]
        public void WarnUnless_Passes_BooleanLambdaWithMessage()
        {
            Warn.Unless(() => 2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void WarnIf_Passes_BooleanLambdaWithMessage()
        {
            Warn.If(() => 2 + 2 != 4, "Not Equal");
        }

        [Test]
        public void WarnUnless_Passes_BooleanLambdaWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void WarnIf_Passes_BooleanLambdaWithMessageAndArgs()
        {
            Warn.If(() => 2 + 2 != 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void WarnUnless_Passes_BooleanLambdaWithWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(() => 2 + 2 == 4, getExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_BooleanLambdaWithWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.If(() => 2 + 2 != 4, getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_Passes_ActualAndConstraint()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Passes_ActualAndConstraint()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Passes_ActualAndConstraintWithMessage()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarnIf_Passes_ActualAndConstraintWithMessage()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarnUnless_Passes_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarnIf_Passes_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(4), "Should be {0}", 4);
        }

#if !NET20
        [Test]
        public void WarnUnless_Passes_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(2 + 2, Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.If(2 + 2, Is.Not.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarnUnless_Passes_ActualLambdaAndConstraint()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Passes_ActualLambdaAndConstraint()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Passes_ActualLambdaAndConstraintWithMessage()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarnIf_Passes_ActualLambdaAndConstraintWithMessage()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarnUnless_Passes_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarnIf_Passes_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarnUnless_Passes_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.If(() => 2 + 2, Is.Not.EqualTo(4), getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraint()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraint()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFour), Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Message");
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraintWithMessage()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFour), Is.Not.EqualTo(4), "Message");
        }

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFour), Is.Not.EqualTo(4), "Should be {0}", 4);
        }

#if !NET20
        [Test]
        public void WarnUnless_Passes_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.If(new ActualValueDelegate<int>(ReturnsFour), Is.Not.EqualTo(4), getExceptionMessage);
        }
#endif

#if ASYNC
        [Test]
        public void WarnUnless_Passes_Async()
        {
            Warn.Unless(async () => await One(), Is.EqualTo(1));
        }

        [Test]
        public void WarnIf_Passes_Async()
        {
            Warn.If(async () => await One(), Is.Not.EqualTo(1));
        }
#endif

        #endregion

        #region Tests that issue Warnings

        [Test]
        public void CallAssertWarnWithMessage()
        {
            Assert.Warn("MESSAGE");
        }

        [Test]
        public void CallAssertWarnWithMessageAndArgs()
        {
            Assert.Warn("MESSAGE: {0}+{1}={2}", 2, 2, 4);
        }

        [Test]
        public void WarnUnless_Fails_Boolean()
        {
            Warn.Unless(2 + 2 == 5);
        }

        [Test]
        public void WarnIf_Fails_Boolean()
        {
            Warn.If(2 + 2 != 5);
        }

        [Test]
        public void WarnUnless_Fails_BooleanWithMessage()
        {
            Warn.Unless(2 + 2 == 5, "message");
        }

        [Test]
        public void WarnIf_Fails_BooleanWithMessage()
        {
            Warn.If(2 + 2 != 5, "message");
        }

        [Test]
        public void WarnUnless_Fails_BooleanWithMessageAndArgs()
        {
            Warn.Unless(2 + 2 == 5, "got {0}", 5);
        }

        [Test]
        public void WarnIf_Fails_BooleanWithMessageAndArgs()
        {
            Warn.If(2 + 2 != 5, "got {0}", 5);
        }

#if !NET20
        [Test]
        public void WarnUnless_Fails_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.Unless(2 + 2 == 5, getExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.If(2 + 2 != 5, getExceptionMessage);
        }

        [Test]
        public void WarnUnless_Fails_BooleanLambda()
        {
            Warn.Unless(() => 2 + 2 == 5);
        }

        [Test]
        public void WarnIf_Fails_BooleanLambda()
        {
            Warn.If(() => 2 + 2 != 5);
        }

        [Test]
        public void WarnUnless_Fails_BooleanLambdaWithMessage()
        {
            Warn.Unless(() => 2 + 2 == 5, "message");
        }

        [Test]
        public void WarnIf_Fails_BooleanLambdaWithMessage()
        {
            Warn.If(() => 2 + 2 != 5, "message");
        }

        [Test]
        public void WarnUnless_Fails_BooleanLambdaWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2 == 5, "got {0}", 5);
        }

        [Test]
        public void WarnIf_Fails_BooleanLambdaWithMessageAndArgs()
        {
            Warn.If(() => 2 + 2 != 5, "got {0}", 5);
        }

        [Test]
        public void WarnUnless_Fails_BooleanLambdaWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.Unless(() => 2 + 2 == 5, getExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_BooleanLambdaWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.If(() => 2 + 2 != 5, getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_Fails_ActualAndConstraint()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
        }

        [Test]
        public void WarnIf_Fails_ActualAndConstraint()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(5));
        }

        [Test]
        public void WarnUnless_Fails_ActualAndConstraintWithMessage()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5), "Error");
        }

        [Test]
        public void WarnIf_Fails_ActualAndConstraintWithMessage()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(5), "Error");
        }

        [Test]
        public void WarnUnless_Fails_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5), "Should be {0}", 5);
        }

        [Test]
        public void WarnIf_Fails_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.If(2 + 2, Is.Not.EqualTo(5), "Should be {0}", 5);
        }

#if !NET20
        [Test]
        public void WarnUnless_Fails_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.Unless(2 + 2, Is.EqualTo(5), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.If(2 + 2, Is.Not.EqualTo(5), getExceptionMessage);
        }

        [Test]
        public void WarnUnless_Fails_ActualLambdaAndConstraint()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5));
        }

        [Test]
        public void WarnIf_Fails_ActualLambdaAndConstraint()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(5));
        }

        [Test]
        public void WarnUnless_Fails_ActualLambdaAndConstraintWithMessage()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), "Error");
        }

        [Test]
        public void WarnIf_Fails_ActualLambdaAndConstraintWithMessage()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(5), "Error");
        }

        [Test]
        public void WarnUnless_Fails_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), "Should be {0}", 5);
        }

        [Test]
        public void WarnIf_Fails_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.If(() => 2 + 2, Is.Not.EqualTo(5), "Should be {0}", 5);
        }

        [Test]
        public void WarnUnless_Fails_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.If(() => 2 + 2, Is.Not.EqualTo(5), getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraint()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraint()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFive), Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Error");
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraintWithMessage()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFive), Is.Not.EqualTo(4), "Error");
        }

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.If(new ActualValueDelegate<int>(ReturnsFive), Is.Not.EqualTo(4), "Should be {0}", 4);
        }

#if !NET20
        [Test]
        public void WarnUnless_Fails_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 4";
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 4";
            Warn.If(new ActualValueDelegate<int>(ReturnsFive), Is.Not.EqualTo(4), getExceptionMessage);
        }
#endif

#if ASYNC
        [Test]
        public void WarnUnless_Fails_Async()
        {
            Warn.Unless(async () => await One(), Is.EqualTo(2));
        }

        [Test]
        public void WarnIf_Fails_Async()
        {
            Warn.If(async () => await One(), Is.Not.EqualTo(2));
        }
#endif

        #endregion

        #region Warnings followed by terminating Assert

        [Test]
        public void TwoWarningsAndFailure()
        {
            Assert.Warn("First warning");
            Assert.Warn("Second warning");
            Assert.Fail("This fails");
        }

        [Test]
        public void TwoWarningsAndIgnore()
        {
            Assert.Warn("First warning");
            Assert.Warn("Second warning");
            Assert.Ignore("Ignore this");
        }

        [Test]
        public void TwoWarningsAndInconclusive()
        {
            Assert.Warn("First warning");
            Assert.Warn("Second warning");
            Assert.Inconclusive("This is inconclusive");
        }

        #endregion

        #region Helper Methods

        private int ReturnsFour()
        {
            return 4;
        }

        private int ReturnsFive()
        {
            return 5;
        }

#if ASYNC
        private static Task<int> One()
        {
            return Task.Run(() => 1);
        }
#endif

        #endregion

        #region Stack filter tests

        [Test]
        public static void WarningSynchronous()
        {
            Assert.Warn("(Warning message)");
        }

#if NET20
        private delegate void Action();
#endif
        [Test]
        public static void WarningInBeginInvoke()
        {
            using (var finished = new ManualResetEvent(false))
            {
                new Action(() =>
                {
                    try
                    {
                        Assert.Warn("(Warning message)");
                    }
                    finally
                    {
                        finished.Set();
                    }
                }).BeginInvoke(ar => { }, null);

                if (!finished.WaitOne(10000)) Assert.Fail("Timeout while waiting for BeginInvoke to execute.");
            }
        }

        [Test]
        public static void WarningInThreadStart()
        {
            using (var finished = new ManualResetEvent(false))
            {
                new Thread(() =>
                {
                    try
                    {
                        Assert.Warn("(Warning message)");
                    }
                    finally
                    {
                        finished.Set();
                    }
                }).Start();

                if (!finished.WaitOne(10000))
                    Assert.Fail("Timeout while waiting for threadstart to execute.");
            }
        }

        [Test]
        public static void WarningInThreadPoolQueueUserWorkItem()
        {
            using (var finished = new ManualResetEvent(false))
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        Assert.Warn("(Warning message)");
                    }
                    finally
                    {
                        finished.Set();
                    }
                });

                if (!finished.WaitOne(10000))
                    Assert.Fail("Timeout while waiting for Threadpool.QueueUserWorkItem to execute.");
            }
        }

#if ASYNC
        [Test]
        public static void WarningInTaskRun()
        {
            if (!Task.Run(() => Assert.Warn("(Warning message)")).Wait(10000))
                Assert.Fail("Timeout while waiting for Task.Run to execute.");
        }

        [Test]
        public static async System.Threading.Tasks.Task WarningAfterAwaitTaskDelay()
        {
            await Task.Delay(1);
            Assert.Warn("(Warning message)");
        }
#endif

#endregion
    }

    public abstract class WarningInSetUpOrTearDownFixture
    {
        [Test]
        public void WarningPassesInTest()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void WarningFailsInTest()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
        }

        [Test]
        public void ThreeWarningsFailInTest()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
            Warn.Unless(2 + 2, Is.EqualTo(22));
            Warn.Unless(2 + 2, Is.EqualTo(42));
        }
    }

    public class WarningInSetUpPasses : WarningInSetUpOrTearDownFixture
    {
        [SetUp]
        public void SetUp()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4));
        }
    }

    public class WarningInSetUpFails : WarningInSetUpOrTearDownFixture
    {
        [SetUp]
        public void SetUp()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
        }
    }

    public class WarningInTearDownPasses : WarningInSetUpOrTearDownFixture
    {
        [TearDown]
        public void TearDown()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4));
        }
    }

    public class WarningInTearDownFails : WarningInSetUpOrTearDownFixture
    {
        [TearDown]
        public void TearDown()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
        }
    }
}
