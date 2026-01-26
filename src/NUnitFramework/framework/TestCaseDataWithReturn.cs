// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
    /// <summary>
    /// The TestCaseDataWithReturn class represents a set of
    /// arguments and other parameter info to be used for a
    /// parameterized test case that has a return value. It
    /// is derived from TestCaseParameters and adds a fluent
    /// syntax for use in initializing the test case.
    /// </summary>
    public class TestCaseDataWithReturn<T, TReturn> : TestCaseParameters
    {
        /// <summary>
        /// Construct a TestCaseDataWithReturn with a single argument.
        /// </summary>
        public TestCaseDataWithReturn(T argument)
            : base([argument])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with two arguments.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with three arguments.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2, T argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T)];
        }

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseDataWithReturn</returns>
        public TestCaseDataWithReturn<T, TReturn> Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseDataWithReturn<T, TReturn> SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, bool&gt;(arg)
        ///     .SetArgDisplayNames("argDisplayName");
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T, TReturn> SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, bool&gt;(args)
        ///     .SetArgDisplayNames(testData.Name);
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T, TReturn> SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseDataWithReturn instance.</returns>
        public TestCaseDataWithReturn<T, TReturn> SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseDataWithReturn<T, TReturn> SetCategory(string category)
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
        public TestCaseDataWithReturn<T, TReturn> SetProperty(string propName, string propValue)
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
        public TestCaseDataWithReturn<T, TReturn> SetProperty(string propName, int propValue)
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
        public TestCaseDataWithReturn<T, TReturn> SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseDataWithReturn<T, TReturn> Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseDataWithReturn<T, TReturn> Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, TReturn> : TestCaseParameters
    {
        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T1), typeof(T2)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, TReturn expectedReturnValue)
            : this(argument1, argument2)
        {
            Returns(expectedReturnValue);
        }

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseDataWithReturn</returns>
        public TestCaseDataWithReturn<T1, T2, TReturn> Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseDataWithReturn<T1, T2, TReturn> SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, bool&gt;(arg1, arg2)
        ///     .SetArgDisplayNames("arg1DisplayName", "arg2DisplayName");
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, TReturn> SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, bool&gt;(arg1, arg2)
        ///     .SetArgDisplayNames(testData.Name, testData.Gender);
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, TReturn> SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseDataWithReturn instance.</returns>
        public TestCaseDataWithReturn<T1, T2, TReturn> SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseDataWithReturn<T1, T2, TReturn> SetCategory(string category)
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
        public TestCaseDataWithReturn<T1, T2, TReturn> SetProperty(string propName, string propValue)
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
        public TestCaseDataWithReturn<T1, T2, TReturn> SetProperty(string propName, int propValue)
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
        public TestCaseDataWithReturn<T1, T2, TReturn> SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, TReturn> Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, TReturn> Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, TReturn> : TestCaseParameters
    {
        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3)
        {
            Returns(expectedReturnValue);
        }

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseDataWithReturn</returns>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, bool&gt;(arg1, arg2, arg3)
        ///     .SetArgDisplayNames("arg1DisplayName", "arg2DisplayName", "arg3DisplayName");
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, bool&gt;(arg1, arg2, arg3)
        ///     .SetArgDisplayNames(testData.Name, testData.Gender, testData.Age);
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseDataWithReturn instance.</returns>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetCategory(string category)
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
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetProperty(string propName, string propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetProperty(string propName, int propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, TReturn> Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> : TestCaseParameters
    {
        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
            : base([argument1, argument2, argument3, argument4])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3, argument4)
        {
            Returns(expectedReturnValue);
        }

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseDataWithReturn</returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, decimal, bool&gt;(arg1, arg2, arg3, arg4)
        ///     .SetArgDisplayNames("arg1DisplayName", "arg2DisplayName", "arg3DisplayName", "arg4DisplayName");
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, decimal, bool&gt;(arg1, arg2, arg3, arg4)
        ///     .SetArgDisplayNames(testData.Name, testData.Gender, testData.Age, testData.Salary);
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseDataWithReturn instance.</returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetCategory(string category)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetProperty(string propName, string propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetProperty(string propName, int propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> : TestCaseParameters
    {
        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
            : base([argument1, argument2, argument3, argument4, argument5])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)];
        }

        /// <summary>
        /// Construct a TestCaseDataWithReturn with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3, argument4, argument5)
        {
            Returns(expectedReturnValue);
        }

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseDataWithReturn</returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> Returns(object? result)
        {
            ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <remarks>
        /// Consider using <see cref="SetArgDisplayNames(string[])"/> for setting argument values in the test name.
        /// <see cref="SetArgDisplayNames(string[])"/> allows you to specify the display names for parameters directly without
        /// needing to use tokens like {m}.
        /// </remarks>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, decimal, FileAccess, bool&gt;(arg1, arg2, arg3, arg4, arg5)
        ///     .SetArgDisplayNames("arg1DisplayName", "arg2DisplayName", "arg3DisplayName", "arg4DisplayName", "arg5DisplayName");
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        /// <returns>The modified TestCaseDataWithReturn instance</returns>
        /// <example>
        /// <code>
        /// var testCase = new TestCaseDataWithReturn&lt;string, int, float, decimal, FileAccess, bool&gt;(arg1, arg2, arg3, arg4, arg5)
        ///     .SetArgDisplayNames(testData.Name, testData.Gender, testData.Age, testData.Salary, testData.FileAccess);
        /// </code>
        /// </example>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetArgDisplayNames(params object?[]? displayNames)
        {
            ArgDisplayNames = displayNames is null
                ? null
                : Array.ConvertAll(displayNames, Constraints.MsgUtils.FormatValue);
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseDataWithReturn instance.</returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetDescription(string description)
        {
            Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetCategory(string category)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetProperty(string propName, string propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetProperty(string propName, int propValue)
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
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> SetProperty(string propName, double propValue)
        {
            Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }
}
