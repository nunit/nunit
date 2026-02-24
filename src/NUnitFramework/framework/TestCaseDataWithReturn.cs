// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
   /// <summary>
    /// The TestCaseDataWithReturn class represents a set of
    /// arguments and other parameter info to be used for a
    /// parameterized test case that has a return value.  It
    /// is derived from TestCaseParameters and adds a fluent
    /// syntax for use in initializing the test case.
    /// </summary>
    public sealed class TestCaseDataWithReturn<T, TReturn> : TestCaseDataWithReturnBase<TestCaseDataWithReturn<T, TReturn>, TReturn>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T, TReturn> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a single argument.
        /// </summary>
        public TestCaseDataWithReturn(T argument)
            : base([argument])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseData with a single argument and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T argument, TReturn expectedReturnValue)
            : this(argument)
        {
            Returns(expectedReturnValue);
        }

        /// <summary>
        /// Construct a TestCaseData with two arguments.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseData with two arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2, TReturn expectedReturnValue)
            : this(argument1, argument2)
        {
            Returns(expectedReturnValue);
        }

        /// <summary>
        /// Construct a TestCaseData with three arguments.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2, T argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseData with three arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T argument1, T argument2, T argument3, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3)
        {
            Returns(expectedReturnValue);
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, TReturn> : TestCaseDataWithReturnBase<TestCaseDataWithReturn<T1, T2, TReturn>, TReturn>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, TReturn> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T1), typeof(T2)];
        }

        /// <summary>
        /// Construct a TestCaseData with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, TReturn expectedReturnValue)
            : this(argument1, argument2)
        {
            Returns(expectedReturnValue);
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, TReturn> : TestCaseDataWithReturnBase<TestCaseDataWithReturn<T1, T2, T3, TReturn>, TReturn>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, TReturn> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3)];
        }

        /// <summary>
        /// Construct a TestCaseData with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3)
        {
            Returns(expectedReturnValue);
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> : TestCaseDataWithReturnBase<TestCaseDataWithReturn<T1, T2, T3, T4, TReturn>, TReturn>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, T4, TReturn> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
            : base([argument1, argument2, argument3, argument4])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
        }

        /// <summary>
        /// Construct a TestCaseData with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3, argument4)
        {
            Returns(expectedReturnValue);
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> : TestCaseDataWithReturnBase<TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn>, TReturn>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseDataWithReturn<T1, T2, T3, T4, T5, TReturn> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
            : base([argument1, argument2, argument3, argument4, argument5])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)];
        }

        /// <summary>
        /// Construct a TestCaseData with a list of arguments and an expected return value.
        /// </summary>
        public TestCaseDataWithReturn(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, TReturn expectedReturnValue)
            : this(argument1, argument2, argument3, argument4, argument5)
        {
            Returns(expectedReturnValue);
        }

        #endregion
    }
}
