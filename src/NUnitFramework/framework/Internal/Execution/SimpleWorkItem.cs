// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;
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

        readonly TestMethod _testMethod;

        /// <summary>
        /// Construct a simple work item for a test.
        /// </summary>
        /// <param name="test">The test to be executed</param>
        /// <param name="filter">The filter used to select this test</param>
        [Obsolete("This member will be removed in a future major release.")]
        public SimpleWorkItem(TestMethod test, ITestFilter filter) : this(test, filter, new DebuggerProxy())
        {
        }

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
        private TestCommand MakeTestCommand()
        {
            if (Test.RunState == RunState.Runnable ||
                Test.RunState == RunState.Explicit && Filter.IsExplicitMatch(Test))
            {
                // Command to execute test
                TestCommand command = new TestMethodCommand(_testMethod);

                var method = _testMethod.Method;

                // Add any wrappers to the TestMethodCommand
                foreach (IWrapTestMethod wrapper in method.GetCustomAttributes<IWrapTestMethod>(true))
                    command = wrapper.Wrap(command);

                // Create TestActionCommands using attributes of the method
                foreach (ITestAction action in Test.Actions)
                    if (action.Targets == ActionTargets.Default || action.Targets.HasFlag(ActionTargets.Test))
                        command = new TestActionCommand(command, action); ;

                // Try to locate the parent fixture. In current implementations, the test method
                // is either one or two levels below the TestFixture - if this changes,
                // so should the following code.
                TestFixture parentFixture = Test.Parent as TestFixture ?? Test.Parent?.Parent as TestFixture;

                // In normal operation we should always get the methods from the parent fixture.
                // However, some of NUnit's own tests can create a TestMethod without a parent
                // fixture. Most likely, we should stop doing this, but it affects 100s of cases.
                var setUpMethods = parentFixture?.SetUpMethods ?? Test.TypeInfo.GetMethodsWithAttribute<SetUpAttribute>(true);
                var tearDownMethods = parentFixture?.TearDownMethods ?? Test.TypeInfo.GetMethodsWithAttribute<TearDownAttribute>(true);

                // Wrap in SetUpTearDownCommands
                var setUpTearDownList = BuildSetUpTearDownList(setUpMethods, tearDownMethods);
                foreach (var item in setUpTearDownList)
                    command = new SetUpTearDownCommand(command, item);

                // Dispose of fixture if necessary
                var isInstancePerTestCase = Test.HasLifeCycle(LifeCycle.InstancePerTestCase);
                if (isInstancePerTestCase && parentFixture is IDisposableFixture && typeof(IDisposable).IsAssignableFrom(parentFixture.TypeInfo.Type))
                    command = new DisposeFixtureCommand(command);

                // In the current implementation, upstream actions only apply to tests. If that should change in the future,
                // then actions would have to be tested for here. For now we simply assert it in Debug. We allow
                // ActionTargets.Default, because it is passed down by ParameterizedMethodSuite.
                int index = Context.UpstreamActions.Count;
                while (--index >= 0)
                {
                    ITestAction action = Context.UpstreamActions[index];
                    System.Diagnostics.Debug.Assert(
                        action.Targets == ActionTargets.Default || action.Targets.HasFlag(ActionTargets.Test),
                        "Invalid target on upstream action: " + action.Targets.ToString());

                    command = new TestActionCommand(command, action);
                }

                // Add wrappers that apply before setup and after teardown
                foreach (ICommandWrapper decorator in method.GetCustomAttributes<IWrapSetUpTearDown>(true))
                    command = decorator.Wrap(command);

                // Add command to set up context using attributes that implement IApplyToContext
                foreach (var attr in method.GetCustomAttributes<IApplyToContext>(true))
                    command = new ApplyChangesToContextCommand(command, attr);

                // Add a construct command and optionally a dispose command in case of instance per test case.
                if (isInstancePerTestCase)
                {
                    command = new FixturePerTestCaseCommand(command);
                }
                // If a timeout is specified, create a TimeoutCommand
                // Timeout set at a higher level
                int timeout = Context.TestCaseTimeout;

                // Timeout set on this test
                if (Test.Properties.ContainsKey(PropertyNames.Timeout))
                    timeout = (int)Test.Properties.Get(PropertyNames.Timeout);

                if (timeout > 0)
                    command = new TimeoutCommand(command, timeout, _debugger);

                // Add wrappers for repeatable tests after timeout so the timeout is reset on each repeat
                foreach (var repeatableAttribute in method.GetCustomAttributes<IRepeatTest>(true))
                    command = repeatableAttribute.Wrap(command);

                return command;
            }
            else
            {
                return new SkipCommand(_testMethod);
            }
        }
    }
}
