// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Common base class for TestCaseData types that expose a Return method.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public abstract class TestCaseDataWithReturnBase<TSelf, TReturn> : TestCaseDataBase<TSelf>
        where TSelf : TestCaseDataWithReturnBase<TSelf, TReturn>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDataBase{TSelf}"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected TestCaseDataWithReturnBase(params object?[]? args)
            : base(args ?? [null])
        {
        }

        internal TestCaseDataWithReturnBase(TestCaseParameters data)
            : base(data)
        {
        }

        #endregion

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

        #endregion
    }
}
