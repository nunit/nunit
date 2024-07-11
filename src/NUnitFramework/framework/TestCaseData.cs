// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// The TestCaseData class represents a set of arguments
    /// and other parameter info to be used for a parameterized
    /// test case. It is derived from TestCaseParameters and adds a
    /// fluent syntax for use in initializing the test case.
    /// </summary>
    public class TestCaseData : TestCaseParameters
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TestCaseData(params object?[]? args)
            : base(args ?? new object?[] { null })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestCaseData(object? arg)
            : base(new[] { arg })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestCaseData(object? arg1, object? arg2)
            : base(new[] { arg1, arg2 })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestCaseData(object? arg1, object? arg2, object? arg3)
            : base(new[] { arg1, arg2, arg3 })
        {
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseData</returns>
        public TestCaseData Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseData instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames"/>for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseData SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseData instance</returns>
        /// <example>
        /// <code>
        /// TestCaseData testCase = new TestCaseData(args)
        ///     .SetArgDisplayNames("arg1DisplayName", "arg2DisplayName");
        /// </code>
        /// </example>
        public TestCaseData SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseData instance.</returns>
        public TestCaseData SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseData SetCategory(string category)
        {
            Properties.Add(PropertyNames.Category, category);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, string propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, int propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseData Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseData Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        /// <summary>
        /// Ignores this TestCase, specifying the reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public IgnoredTestCaseData Ignore(string reason)
        {
            RunState prevRunState = RunState;
            RunState = RunState.Ignored;
            Properties.Set(PropertyNames.SkipReason, reason);
            var ignoredData = new IgnoredTestCaseData(this, prevRunState);
            return ignoredData;
        }

        #endregion
    }
}
