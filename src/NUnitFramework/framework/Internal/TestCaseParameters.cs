// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestCaseParameters class encapsulates method arguments and
    /// other selected parameters needed for constructing
    /// a parameterized test case.
    /// </summary>
    public class TestCaseParameters : TestParameters, ITestCaseData, IApplyToTest
    {
        #region Instance Fields

        /// <summary>
        /// The expected result to be returned
        /// </summary>
        private object? _expectedResult;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor creates an empty parameter set
        /// </summary>
        public TestCaseParameters()
        {
        }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public TestCaseParameters(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public TestCaseParameters(object?[] args) : base(args)
        {
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestCaseData
        /// </summary>
        /// <param name="data"></param>
        public TestCaseParameters(ITestCaseData data) : base(data)
        {
            if (data.HasExpectedResult)
                ExpectedResult = data.ExpectedResult;
        }

        #endregion

        #region ITestCaseData Members

        /// <summary>
        /// The expected result of the test, which
        /// must match the method return type.
        /// </summary>
        public object? ExpectedResult
        {
            get => _expectedResult;
            set
            {
                _expectedResult = value;
                HasExpectedResult = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an expected result was specified.
        /// </summary>
        public bool HasExpectedResult { get; set; }

        #endregion

        /// <summary>
        /// Get or set the type arguments for a generic test method.
        /// If not set explicitly, the generic types will be inferred
        /// based on the test case parameters.
        /// </summary>
        public Type[]? TypeArgs { get; set; } = null;
    }
}
