// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !THREAD_ABORT
#pragma warning disable format // Temporary until release of https://github.com/dotnet/roslyn/issues/62612
#endif

#if THREAD_ABORT

using System;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class RepeatableTestsWithTimeoutAttributesTests
    {
        private int _retryOnlyCount;

        [Test]
        [Retry(3)]
        public void ShouldPassAfter3Retries()
        {
            _retryOnlyCount++;
            Assert.That(_retryOnlyCount, Is.GreaterThanOrEqualTo(2));
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
            SimpleWorkItem? work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            TestCommand command = work.MakeTestCommand();

            Assert.That(command, Is.TypeOf(typeof(RetryAttribute.RetryCommand)));
            RetryAttribute.RetryCommand retryCommand = (RetryAttribute.RetryCommand)command;

            command = GetInnerCommand(retryCommand);

            Assert.That(command, Is.TypeOf(typeof(TimeoutCommand)));
            TimeoutCommand timeoutCommand = (TimeoutCommand)command;

            command = GetInnerCommand(timeoutCommand);

            Assert.That(command, Is.TypeOf(typeof(ApplyChangesToContextCommand)));
            ApplyChangesToContextCommand applyChangesToContextCommand = (ApplyChangesToContextCommand)command;

            command = GetInnerCommand(applyChangesToContextCommand);

            Assert.That(command, Is.TypeOf(typeof(HookDelegatingTestCommand)));
            HookDelegatingTestCommand hookDelegatingTestCommandCommand = (HookDelegatingTestCommand)command;

            command = GetInnerCommand(hookDelegatingTestCommandCommand);

            Assert.That(command, Is.TypeOf(typeof(TestMethodCommand)));
        }

        [Test]
        public void ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime()
        {
            // Rather than testing with sleeps, this tests that the execution will occur in the correct
            // order by checking which commands are run when. As the repeat command comes first, the
            // timeout will be reset each time it runs
            var test = TestBuilder.MakeTestFromMethod(typeof(HelperMethodForTimeoutsClass), nameof(HelperMethodForTimeoutsClass.ShouldPassAfter2RepeatsAndTimeoutIsResetEachTime));
            SimpleWorkItem? work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            TestCommand command = work.MakeTestCommand();

            Assert.That(command, Is.TypeOf(typeof(RepeatAttribute.RepeatedTestCommand)));
            RepeatAttribute.RepeatedTestCommand repeatedCommand = (RepeatAttribute.RepeatedTestCommand)command;

            command = GetInnerCommand(repeatedCommand);

            Assert.That(command, Is.TypeOf(typeof(TimeoutCommand)));
            TimeoutCommand timeoutCommand = (TimeoutCommand)command;

            command = GetInnerCommand(timeoutCommand);

            Assert.That(command, Is.TypeOf(typeof(ApplyChangesToContextCommand)));
            ApplyChangesToContextCommand applyChangesToContextCommand = (ApplyChangesToContextCommand)command;

            command = GetInnerCommand(applyChangesToContextCommand);

            Assert.That(command, Is.TypeOf(typeof(HookDelegatingTestCommand)));
            HookDelegatingTestCommand hookDelegatingTestCommandCommand = (HookDelegatingTestCommand)command;

            command = GetInnerCommand(hookDelegatingTestCommandCommand);
        }

        private TestCommand GetInnerCommand(DelegatingTestCommand command)
        {
            FieldInfo? innerCommand = command.GetType().GetField("innerCommand", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(innerCommand, Is.Not.Null);
            return (TestCommand)innerCommand.GetValue(command)!;
        }

        [Test]
        public void ShouldFailOnMultipleRepeatableAttributes()
        {
            var testCase = TestBuilder.MakeTestCase(GetType(), nameof(TestMethodForRepeatAndRetryExpectedFail));
            var workItem = TestBuilder.CreateWorkItem(testCase);
            var result = TestBuilder.ExecuteWorkItem(workItem);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.ResultState.Label, Is.EqualTo("Invalid"));
            });
        }

        [Test]
        public void ShouldFailOnMultipleRepeatableAttributesIncludingCustom()
        {
            var testCase = TestBuilder.MakeTestCase(GetType(), nameof(TestMethodForRepeatAndCustomRepeatExpectedFail));
            var workItem = TestBuilder.CreateWorkItem(testCase);
            var result = TestBuilder.ExecuteWorkItem(workItem);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.ResultState.Label, Is.EqualTo("Invalid"));
            });
        }

        [Repeat(1), Retry(1)]
        private void TestMethodForRepeatAndRetryExpectedFail()
        {
        }

        [Repeat(1), CustomRepeater]
        private void TestMethodForRepeatAndCustomRepeatExpectedFail()
        {
        }

        #region TestCustomAttribute

        internal class CustomRepeater : Attribute, IRepeatTest
        {
            public TestCommand Wrap(TestCommand command)
            {
                return command;
            }
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
            Assert.That(RepeatCount, Is.GreaterThanOrEqualTo(1));
        }
    }

    [TestFixture]
    public class DerivedRepeatableTestFixture : BaseRepeatableTestFixture
    {
        [Test, Retry(3)]
        public override void ShouldBeOveridden()
        {
            RepeatCount++;
            Assert.That(RepeatCount, Is.GreaterThanOrEqualTo(2));
        }
    }

    [TestFixture]
    public class DerivedRepeatableTestFixtureWithNoRepeat : BaseRepeatableTestFixture
    {
        [Test]
        public override void ShouldBeOveridden()
        {
            RepeatCount++;
            Assert.That(RepeatCount, Is.EqualTo(1));
        }
    }
}

#endif
