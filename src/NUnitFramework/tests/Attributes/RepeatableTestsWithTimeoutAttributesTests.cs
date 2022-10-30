// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;

#if THREAD_ABORT

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class RepeatableTestsWithTimeoutAttributesTests
    {
        private int retryOnlyCount;

        [Test]
        [Retry(3)]
        public void ShouldPassAfter3Retries()
        {
            retryOnlyCount++;
            Assert.True(retryOnlyCount >= 2);
        }

        public class HelperMethodForTimeoutsClass
        {
            [Test]
            [Retry(3), Timeout(85)]
            public void ShouldPassAfter3RetriesAndTimeoutIsResetEachTime()
            {
            }

            [Test]
            [Repeat(3), Timeout(85)]
            public void ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime()
            {
            }
        }

        [Test]
        public void ShouldPassAfter3RetriesAndTimeoutIsResetEachTime()
        {
            // Rather than testing with sleeps, this tests that the execution will occur in the correct 
            // order by checking which commands are run when. As the retry command comes first, the
            // timeout will be reset each time it runs
            var test = TestBuilder.MakeTestFromMethod(typeof(HelperMethodForTimeoutsClass), nameof(HelperMethodForTimeoutsClass.ShouldPassAfter3RetriesAndTimeoutIsResetEachTime));
            SimpleWorkItem work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            var method = typeof(SimpleWorkItem).GetMethod("MakeTestCommand", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            TestCommand command = (TestCommand)method.Invoke(work, null);

            Assert.That(command, Is.TypeOf(typeof(RetryAttribute.RetryCommand)));
            RetryAttribute.RetryCommand retryCommand = (RetryAttribute.RetryCommand)command;

            command = GetInnerCommand(retryCommand);

            Assert.That(command, Is.TypeOf(typeof(TimeoutCommand)));
            TimeoutCommand timeoutCommand = (TimeoutCommand)command;

            command = GetInnerCommand(timeoutCommand);

            Assert.That(command, Is.TypeOf(typeof(ApplyChangesToContextCommand)));
            ApplyChangesToContextCommand applyChangesToContextCommand = (ApplyChangesToContextCommand)command;

            command = GetInnerCommand(applyChangesToContextCommand);

            Assert.That(command, Is.TypeOf(typeof(TestMethodCommand)));
        }

        [Test]
        public void ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime()
        {
            // Rather than testing with sleeps, this tests that the execution will occur in the correct 
            // order by checking which commands are run when. As the repeat command comes first, the
            // timeout will be reset each time it runs
            var test = TestBuilder.MakeTestFromMethod(typeof(HelperMethodForTimeoutsClass), nameof(HelperMethodForTimeoutsClass.ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime));
            SimpleWorkItem work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            var method = typeof(SimpleWorkItem).GetMethod("MakeTestCommand", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            TestCommand command = (TestCommand)method.Invoke(work, null);

            Assert.That(command, Is.TypeOf(typeof(RepeatAttribute.RepeatedTestCommand)));
            RepeatAttribute.RepeatedTestCommand repeatedCommand = (RepeatAttribute.RepeatedTestCommand)command;

            command = GetInnerCommand(repeatedCommand);

            Assert.That(command, Is.TypeOf(typeof(TimeoutCommand)));
            TimeoutCommand timeoutCommand = (TimeoutCommand)command;

            command = GetInnerCommand(timeoutCommand);

            Assert.That(command, Is.TypeOf(typeof(ApplyChangesToContextCommand)));
            ApplyChangesToContextCommand applyChangesToContextCommand = (ApplyChangesToContextCommand)command;

            command = GetInnerCommand(applyChangesToContextCommand);

            Assert.That(command, Is.TypeOf(typeof(TestMethodCommand)));
        }

        private TestCommand GetInnerCommand(DelegatingTestCommand command)
        {
            return (TestCommand)command.GetType().GetField("innerCommand", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(command);
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
