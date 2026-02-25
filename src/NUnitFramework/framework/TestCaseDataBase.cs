// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Common base class for TestCaseData types.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class TestCaseDataBase<TSelf> : TestCaseParameters
        where TSelf : TestCaseDataBase<TSelf>
    {
        /// <summary>
        /// Return this reference without a typecast.
        /// </summary>
        protected abstract TSelf GetSelf();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDataBase{TSelf}"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected TestCaseDataBase(params object?[]? args)
            : base(args ?? [null])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDataBase{TSelf}"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestCaseDataBase(object? arg)
            : base([arg])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDataBase{TSelf}"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestCaseDataBase(object? arg1, object? arg2)
            : base([arg1, arg2])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDataBase{TSelf}"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestCaseDataBase(object? arg1, object? arg2, object? arg3)
            : base([arg1, arg2, arg3])
        {
        }

        internal TestCaseDataBase(TestCaseParameters data)
            : base(data)
        {
        }

        #endregion

        #region Conversion Operator

        /// <summary>
        /// Convert anything deriving from TestCaseDataBase&lt;TSelf&gt; to TestCaseData.
        /// </summary>
        public static implicit operator TestCaseData(TestCaseDataBase<TSelf> @this)
        {
            return new TestCaseData(@this);
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseData instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TSelf SetName(string? name)
        {
            TestName = name;
            return GetSelf();
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
        public TSelf SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return GetSelf();
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseData instance</returns>
        /// <example>
        /// <code>
        /// TestCaseData testCase = new TestCaseData(args)
        ///     .SetArgDisplayNames(testData.Name, testData.Gender, testData.Age);
        /// </code>
        /// </example>
        public TSelf SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return GetSelf();
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseData instance.</returns>
        public TSelf SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return GetSelf();
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TSelf SetCategory(string category)
        {
            Properties.Add(PropertyNames.Category, category);
            return GetSelf();
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TSelf SetProperty(string propName, string propValue)
        {
            Properties.Add(propName, propValue);
            return GetSelf();
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TSelf SetProperty(string propName, int propValue)
        {
            Properties.Add(propName, propValue);
            return GetSelf();
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TSelf SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return GetSelf();
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TSelf Explicit()
        {
            RunState = RunState.Explicit;
            return GetSelf();
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TSelf Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return GetSelf();
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
