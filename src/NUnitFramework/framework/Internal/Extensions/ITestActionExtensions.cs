// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Extensions
{
    internal static class ITestActionExtensions
    {
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
