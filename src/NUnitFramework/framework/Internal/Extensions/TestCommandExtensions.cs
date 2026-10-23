// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TestCommandExtensions
    {
        extension(TestCommand command)
        {
            /// <summary>
            /// Wraps the command once for each item, in order.
            /// Each wrap encloses the result of the previous one creating a chain of commands.
            /// </summary>
            public TestCommand WrapWith<T>(IEnumerable<T> items, Func<TestCommand, T, TestCommand> wrap)
            {
                TestCommand wrapped = command;
                foreach (T item in items)
                    wrapped = wrap(wrapped, item);

                return wrapped;
            }

            /// <summary>
            /// Wraps the command with each wrapper, in order.
            /// Each wrap encloses the result of the previous one.
            /// </summary>
            public TestCommand WrapWith(IEnumerable<ICommandWrapper> wrappers)
            {
                TestCommand wrapped = command;
                foreach (ICommandWrapper wrapper in wrappers)
                    wrapped = wrapper.Wrap(wrapped);

                return wrapped;
            }
        }

        extension(ITestAction action)
        {
            /// <summary>
            /// For an action attached to a test method: whether it runs around the test.
            /// It does, unless it targets suites only.
            /// </summary>
            public bool AppliesToTest()
                => action.Targets == ActionTargets.Default || action.Targets.HasFlag(ActionTargets.Test);

            /// <summary>
            /// For an action attached to a suite: whether it runs around the suite itself.
            /// </summary>
            /// <remarks>
            /// A <see cref="ParameterizedMethodSuite"/> and its test cases share one <see cref="System.Reflection.MethodInfo"/>
            /// as the source of attributes. The test cases handle <see cref="ActionTargets.Default"/>,
            /// so the suite must not handle it too.
            /// </remarks>
            public bool AppliesToSuite(Test suite)
                => action.Targets.HasFlag(ActionTargets.Suite)
                || (action.Targets == ActionTargets.Default && suite is not ParameterizedMethodSuite);

            /// <summary>
            /// For an action attached to a suite: whether it is passed down to run around each of its tests.
            /// </summary>
            /// <remarks>
            /// Not for a <see cref="ParameterizedMethodSuite"/>: its test cases get the action
            /// from the shared <see cref="System.Reflection.MethodInfo"/> themselves.
            /// </remarks>
            public bool IsPassedDownToTests(Test suite)
                => action.Targets.HasFlag(ActionTargets.Test) && suite is not ParameterizedMethodSuite;
        }
    }
}
