// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// The TestCaseData class represents a set of arguments
    /// and other parameter info to be used for a parameterized
    /// test case. It is derived from TestCaseParameters and adds a
    /// fluent syntax for use in initializing the test case.
    /// </summary>
    public class TestCaseData : TestCaseDataWithReturnBase<TestCaseData, object?>
    {
        /// <summary>
        /// Return this reference without a typecast.
        /// </summary>
        /// <returns></returns>
        protected override TestCaseData GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TestCaseData(params object?[]? args)
            : base(args ?? [null])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestCaseData(object? arg)
            : base([arg])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestCaseData(object? arg1, object? arg2)
            : base([arg1, arg2])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestCaseData(object? arg1, object? arg2, object? arg3)
            : base([arg1, arg2, arg3])
        {
        }

        internal TestCaseData(TestCaseParameters parameters)
            : base(parameters)
        {
        }

        #endregion

    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public sealed class TestCaseData<T> : TestCaseDataBase<TestCaseData<T>>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseData<T> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a single argument.
        /// </summary>
        public TestCaseData(T argument)
            : base([argument])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseData with two arguments.
        /// </summary>
        public TestCaseData(T argument1, T argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T)];
        }

        /// <summary>
        /// Construct a TestCaseData with three arguments.
        /// </summary>
        public TestCaseData(T argument1, T argument2, T argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T)];
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseData<T1, T2> : TestCaseDataBase<TestCaseData<T1, T2>>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseData<T1, T2> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseData(T1 argument1, T2 argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T1), typeof(T2)];
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseData<T1, T2, T3> : TestCaseDataBase<TestCaseData<T1, T2, T3>>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseData<T1, T2, T3> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseData(T1 argument1, T2 argument2, T3 argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3)];
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseData<T1, T2, T3, T4> : TestCaseDataBase<TestCaseData<T1, T2, T3, T4>>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseData<T1, T2, T3, T4> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseData(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
            : base([argument1, argument2, argument3, argument4])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
        }

        #endregion
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    public class TestCaseData<T1, T2, T3, T4, T5> : TestCaseDataBase<TestCaseData<T1, T2, T3, T4, T5>>
    {
        /// <summary>
        /// Return this pointer without typecasting.
        /// </summary>
        protected override TestCaseData<T1, T2, T3, T4, T5> GetSelf() => this;

        #region Constructors

        /// <summary>
        /// Construct a TestCaseData with a list of arguments.
        /// </summary>
        public TestCaseData(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
            : base([argument1, argument2, argument3, argument4, argument5])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)];
        }

        #endregion
    }
}
