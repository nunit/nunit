// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Implementations of fluent instance modifier methods for TestCaseData&lt;&gt; generic types.
    /// </summary>
    public abstract class TestCaseDataWithReturnGenericBase<TSelf, TReturn> : TestCaseParameters
        where TSelf : TestCaseDataWithReturnGenericBase<TSelf, TReturn>
    {
        /// <summary>
        /// Construct the object underlying a generic TestCaseData.
        /// </summary>
        /// <param name="args"></param>
        protected TestCaseDataWithReturnGenericBase(object?[] args)
            : base(args)
        {
        }

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected abstract TSelf GetSelf();

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseData</returns>
        public TSelf Returns(TReturn result)
        {
            ExpectedResult = result;
            return GetSelf();
        }

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
        /// var testCase = new TestCaseData&lt;T&gt;(arg)
        ///     .SetArgDisplayNames("argDisplayName");
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
        /// var testCase = new TestCaseData&lt;T&gt;(arg)
        ///     .SetArgDisplayNames(testData.Name);
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

        #endregion
    }

    /// <summary>
    /// The TestCaseDataWithReturn class represents a set of
    /// arguments and other parameter info to be used for a
    /// parameterized test case that has a return value. It
    /// is derived from TestCaseParameters and adds a fluent
    /// syntax for use in initializing the test case.
    /// </summary>
    public class TestCaseDataWithReturn<T, TReturn> : TestCaseDataWithReturnGenericBase<TestCaseDataWithReturn<T, TReturn>, TReturn>
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

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T, TReturn> GetSelf() => this;
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, TReturn> : TestCaseDataWithReturnGenericBase<TestCaseDataWithReturn<T1, T2, TReturn>, TReturn>
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

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, TReturn> GetSelf() => this;
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, TReturn> : TestCaseDataWithReturnGenericBase<TestCaseDataWithReturn<T1, T2, T3, TReturn>, TReturn>
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

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, TReturn> GetSelf() => this;
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> : TestCaseDataWithReturnGenericBase<TestCaseDataWithReturn<T1, T2, T3, T4, TReturn>, TReturn>
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

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> GetSelf() => this;
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> : TestCaseDataWithReturnGenericBase<TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn>, TReturn>
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

        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> GetSelf() => this;
    }
}
