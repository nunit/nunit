// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestCaseParameters class encapsulates method arguments and
    /// other selected parameters needed for constructing
    /// a parameterized test case.
    /// </summary>
    public class TestFixtureParameters : TestParameters, ITestFixtureData
    {
        #region Constructors

        /// <summary>
        /// Default Constructor creates an empty parameter set
        /// </summary>
        public TestFixtureParameters() { }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public TestFixtureParameters(Exception exception) : base(exception) { }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public TestFixtureParameters(params object?[] args) : base(args) { }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestCaseData
        /// </summary>
        /// <param name="data"></param>
        public TestFixtureParameters(ITestFixtureData data) : base(data)
        {
            TypeArgs = data.TypeArgs;
        }

        #endregion

        #region ITestFixtureData Members

        /// <summary>
        /// Type arguments used to create a generic fixture instance
        /// </summary>
        public Type[]? TypeArgs { get; }

        #endregion
    }
}
