// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

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
        public void WarnUnless_Passes_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.Unless(2 + 2 == 4, GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.If(2 + 2 != 4, GetExceptionMessage);
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
        public void WarnUnless_Passes_BooleanLambdaWithWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.Unless(() => 2 + 2 == 4, GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_BooleanLambdaWithWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.If(() => 2 + 2 != 4, GetExceptionMessage);
        }

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
        public void WarnUnless_Passes_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.Unless(2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.If(2 + 2, Is.Not.EqualTo(4), GetExceptionMessage);
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
        public void WarnUnless_Passes_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Warn.If(() => 2 + 2, Is.Not.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraint()
        {
            Warn.Unless(ReturnsFour, Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraint()
        {
            Warn.If(ReturnsFour, Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(ReturnsFour, Is.EqualTo(4), "Message");
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraintWithMessage()
        {
            Warn.If(ReturnsFour, Is.Not.EqualTo(4), "Message");
        }

        [Test]
        public void WarnUnless_Passes_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.Unless(ReturnsFour, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Passes_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Warn.If(ReturnsFour, Is.Not.EqualTo(4), GetExceptionMessage);
        }

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

        #endregion

        #region Tests that issue Warnings

        [Test]
        public void CallAssertWarnWithMessage()
        {
            Assert.Warn("MESSAGE");
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
        public void WarnUnless_Fails_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            Warn.Unless(2 + 2 == 5, GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            Warn.If(2 + 2 != 5, GetExceptionMessage);
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
        public void WarnUnless_Fails_BooleanLambdaWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            Warn.Unless(() => 2 + 2 == 5, GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_BooleanLambdaWithMessageStringFunc()
        {
            string GetExceptionMessage() => "got 5";
            Warn.If(() => 2 + 2 != 5, GetExceptionMessage);
        }

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
        public void WarnUnless_Fails_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            Warn.Unless(2 + 2, Is.EqualTo(5), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            Warn.If(2 + 2, Is.Not.EqualTo(5), GetExceptionMessage);
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
        public void WarnUnless_Fails_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 5";
            Warn.If(() => 2 + 2, Is.Not.EqualTo(5), GetExceptionMessage);
        }

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraint()
        {
            Warn.Unless(ReturnsFive, Is.EqualTo(4));
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraint()
        {
            Warn.If(ReturnsFive, Is.Not.EqualTo(4));
        }

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(ReturnsFive, Is.EqualTo(4), "Error");
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraintWithMessage()
        {
            Warn.If(ReturnsFive, Is.Not.EqualTo(4), "Error");
        }

        [Test]
        public void WarnUnless_Fails_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 4";
            Warn.Unless(ReturnsFive, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void WarnIf_Fails_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Should be 4";
            Warn.If(ReturnsFive, Is.Not.EqualTo(4), GetExceptionMessage);
        }

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

        private static Task<int> One()
        {
            return Task.Run(() => 1);
        }

        #endregion

        #region Stack filter tests

        [Test]
        public static void WarningSynchronous()
        {
            Assert.Warn("(Warning message)");
        }

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

                if (!finished.WaitOne(10_000))
                    Assert.Fail("Timeout while waiting for BeginInvoke to execute.");
            }
        }

        [Test]
        public static void WarningInThreadStart()
        {
            using var finished = new ManualResetEvent(false);
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

            if (!finished.WaitOne(10_000))
                Assert.Fail("Timeout while waiting for threadstart to execute.");
        }

        [Test]
        public static void WarningInThreadPoolQueueUserWorkItem()
        {
            using var finished = new ManualResetEvent(false);
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

            if (!finished.WaitOne(10_000))
                Assert.Fail("Timeout while waiting for Threadpool.QueueUserWorkItem to execute.");
        }

        [Test]
        public static void WarningInTaskRun()
        {
            if (!Task.Run(() => Assert.Warn("(Warning message)")).Wait(10_000))
                Assert.Fail("Timeout while waiting for Task.Run to execute.");
        }

        [Test]
        public static async Task WarningAfterAwaitTaskDelay()
        {
            await Task.Delay(1);
            Assert.Warn("(Warning message)");
        }

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
