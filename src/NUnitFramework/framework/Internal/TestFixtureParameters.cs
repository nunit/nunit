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
    public class TestFixtureParameters : TestParameters, ITestFixtureData
    {
        #region Constructors

        /// <summary>
        /// Default Constructor creates an empty parameter set
        /// </summary>
        public TestFixtureParameters()
        {
        }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public TestFixtureParameters(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public TestFixtureParameters(params object?[] args) : base(args)
        {
        }

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
        public Type[]? TypeArgs { get; set; }

        #endregion

        /// <summary>
        /// Deduce type arguments from the provided arguments, if possible.
        /// This is used when a generic fixture is specified with arguments that can be used to determine the type arguments.
        /// </summary>
        /// <param name="type"></param>
        public void DeduceTypeArgumentsFromArguments(Type type)
        {
            if (TypeArgs is null || TypeArgs.Length == 0 && Arguments.Length > 0)
            {
                object?[]? arguments = Arguments;
                if (type.GetTypeArgumentsFromArguments(ref arguments, out Type[]? typeArgs))
                {
                    Arguments = arguments;
                    TypeArgs = typeArgs;
                }
            }
        }
    }
}
