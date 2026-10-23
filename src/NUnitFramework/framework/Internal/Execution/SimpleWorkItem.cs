// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
#if THREAD_ABORT
using System.Threading;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// A SimpleWorkItem represents a single test case and is
    /// marked as completed immediately upon execution. This
    /// class is also used for skipped or ignored test suites.
    /// </summary>
    public class SimpleWorkItem : WorkItem
    {
        private readonly IDebugger _debugger;
        private readonly TestMethod _testMethod;

        /// <summary>
        /// Construct a simple work item for a test.
        /// </summary>
        /// <param name="test">The test to be executed</param>
        /// <param name="filter">The filter used to select this test</param>
        /// <param name="debugger">An <see cref="IDebugger"/> instance</param>
        internal SimpleWorkItem(TestMethod test, ITestFilter filter, IDebugger debugger)
            : base(test, filter)
        {
            _debugger = debugger;
            _testMethod = test;
        }

        /// <summary>
        /// Method that performs actually performs the work.
        /// </summary>
        protected override void PerformWork()
        {
            try
            {
                var testCommand = MakeTestCommand();

                // Isolate the Execute call because the WorkItemComplete below will run one-time teardowns. Execution
                // context values should not flow from a particular test case into the shared one-time teardown.
                Result = ContextUtils.DoIsolated(() => testCommand.Execute(Context));
            }
            catch (Exception ex)
            {
                // Currently, if there are no command wrappers, test
                // actions, setup or teardown, we have to catch any
                // exception from the test here. In addition, since
                // users may create their own command wrappers, etc.
                // we have to protect against unhandled exceptions.

#if THREAD_ABORT
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif

                Context.CurrentResult.RecordException(ex);
                Result = Context.CurrentResult;
            }
            finally
            {
                WorkItemComplete();
            }
        }

        /// <summary>
        /// Creates a test command for use in running this test.
        /// </summary>
        /// <returns>A TestCommand</returns>
        internal TestCommand MakeTestCommand()
        {
            if (Test.RunState == RunState.Runnable ||
                (Test.RunState == RunState.Explicit && Filter.IsExplicitMatch(Test)))
            {
                // Command to execute test
                TestCommand command = new TestMethodCommand(_testMethod);

                var method = MethodInfoCache.Get(_testMethod.Method);

                // Build the chain of IWrapTestMethod wrappers around the test method
                command = command.WrapWith(method.WrapTestMethodAttributes);

                // Build the chain of TestActionCommands for actions that target the test
                command = command.WrapWith(
                    Test.Actions.Where(action => action.Targets == ActionTargets.Default || action.Targets.HasFlag(ActionTargets.Test)),
                    (c, action) => new TestActionCommand(c, action));

                // Try to locate the parent fixture. In current implementations, the test method
                // is either one or two levels below the TestFixture - if this changes,
                // so should the following code.
                TestFixture? parentFixture = Test.Parent as TestFixture ?? Test.Parent?.Parent as TestFixture;

                // In normal operation we should always get the methods from the parent fixture.
                // However, some of NUnit's own tests can create a TestMethod without a parent
                // fixture. Most likely, we should stop doing this, but it affects 100s of cases.
                ITypeInfo typeInfo = Test.TypeInfo!;
                var setUpMethods = parentFixture?.SetUpMethods ?? typeInfo.GetMethodsWithAttribute<SetUpAttribute>(true);
                var tearDownMethods = parentFixture?.TearDownMethods ?? typeInfo.GetMethodsWithAttribute<TearDownAttribute>(true);

                // Build the chain of SetUpTearDownCommands
                var setUpTearDownList = BuildSetUpTearDownList(setUpMethods, tearDownMethods);
                command = command.WrapWith(setUpTearDownList, (c, item) => new SetUpTearDownCommand(c, item));

                // Dispose of fixture if necessary
                var isInstancePerTestCase = Test.HasLifeCycle(LifeCycle.InstancePerTestCase);
                if (isInstancePerTestCase && parentFixture is not null && DisposeHelper.IsDisposable(parentFixture.TypeInfo.Type))
                    command = new DisposeFixtureCommand(command);

                // Build the chain of TestActionCommands for actions inherited from enclosing suites,
                // in reverse order so the actions of the outermost suite run first
                command = command.WrapWith(Enumerable.Reverse(Context.UpstreamActions), (c, action) => new TestActionCommand(c, action));

                // Add wrappers that apply before setup and after teardown
                command = command.WrapWith(method.WrapSetupTearDownAttributes);

                // Build the chain of ApplyChangesToContextCommands from IApplyToContext attributes
                command = command.WrapWith(method.ApplyToContextAttributes, (c, attr) => new ApplyChangesToContextCommand(c, attr));

                // Add a construct command and optionally a dispose command in case of instance per test case.
                if (isInstancePerTestCase)
                {
                    command = new FixturePerTestCaseCommand(command);
                }

                // If a timeout is specified, create a TimeoutCommand or CancelAfterCommand
                // Get Timeout set on this test or set at a higher level
                int timeout = Test.Properties.TryGet(PropertyNames.Timeout, Context.TestCaseTimeout);
                bool useCancellation = Test.Properties.TryGet(PropertyNames.UseCancellation, Context.UseCancellation);

                if (timeout > 0)
                {
                    command = useCancellation
                        ? new CancelAfterCommand(command, timeout, _debugger)
                        : new TimeoutCommand(command, timeout, _debugger);
                }

                // Add wrappers for repeatable tests after timeout so the timeout is reset on each repeat
                command = command.WrapWith(method.RepeatTestAttributes);

                return command;
            }

            return new SkipCommand(_testMethod);
        }
    }
}
