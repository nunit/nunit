// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
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

        /// <summary>
        /// Construct a ParameterSet from an TestCaseParameters instance.
        /// </summary>
        /// <param name="data"></param>
        public TestCaseParameters(TestCaseParameters data) : base(data)
        {
            if (data.HasExpectedResult)
                ExpectedResult = data.ExpectedResult;

            TypeArgs = data.TypeArgs;
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

        internal void AdjustArgumentsForMethod(IMethodInfo method)
        {
            ParameterInfo[] parameters = method.MethodInfo.GetParameters();
            int argsNeeded = parameters.Length;
            int argsProvided = Arguments.Length;

            // Special handling for ExpectedResult (see if it needs to be converted into method return type)
            if (HasExpectedResult
                && ParamAttributeTypeConversions.TryConvert(ExpectedResult, method.ReturnType.Type, out var expectedResultInTargetType))
            {
                ExpectedResult = expectedResultInTargetType;
            }

            if (parameters.Length > 0)
            {
                // Special handling for params and optional arguments
                Arguments = Reflect.PopulateOptionalArgsAndParamsArray(Arguments, parameters);
                argsProvided = argsNeeded;
            }

            // Special handling when sole argument is an object[]
            if (argsNeeded == 1 && parameters[0].ParameterType == typeof(object[]))
            {
                if (argsProvided > 1 ||
                    argsProvided == 1 && Arguments[0]?.GetType() != typeof(object[]))
                {
                    Arguments = [Arguments];
                }
            }

            if (argsProvided == argsNeeded)
            {
                // Performs several special conversions allowed by NUnit in order to
                // permit arguments with types that cannot be used in the constructor
                // of an Attribute such as TestCaseAttribute or to simplify their use.
                for (int i = 0; i < Arguments.Length; i++)
                {
                    object? arg = Arguments[i];
                    Type targetType = parameters[i].ParameterType;
                    if (ParamAttributeTypeConversions.TryConvert(arg, targetType, out var argAsTargetType))
                    {
                        Arguments[i] = argAsTargetType;
                    }
                }
            }
        }
    }
}
