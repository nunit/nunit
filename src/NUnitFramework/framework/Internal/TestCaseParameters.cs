// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Extensions;

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

        internal void AdjustArgumentsForMethod(IMethodInfo method)
        {
            IParameterInfo[] parameters = method.GetParameters();
            int argsNeeded = parameters.Length;
            int argsProvided = Arguments.Length;

            // Special handling for ExpectedResult (see if it needs to be converted into method return type)
            if (HasExpectedResult
                && ParamAttributeTypeConversions.TryConvert(ExpectedResult, method.ReturnType.Type, out var expectedResultInTargetType))
            {
                ExpectedResult = expectedResultInTargetType;
            }

            // Special handling for CancellationToken
            if (parameters.LastParameterAcceptsCancellationToken() &&
               (!Arguments.LastArgumentIsCancellationToken()))
            {
                // Implicit CancellationToken argument
                argsProvided++;
            }

            // Special handling for params arguments
            if (argsNeeded > 0 && argsProvided >= argsNeeded - 1)
            {
                IParameterInfo lastParameter = parameters[argsNeeded - 1];

                if (parameters.LastParameterIsParamsArray())
                {
                    Type lastParameterType = lastParameter.ParameterType;
                    Type elementType = lastParameterType.GetElementType()!;

                    if (argsProvided == argsNeeded)
                    {
                        if (!lastParameterType.IsInstanceOfType(Arguments[argsProvided - 1]))
                        {
                            Array array = Array.CreateInstance(elementType, 1);
                            array.SetValue(Arguments[argsProvided - 1], 0);
                            Arguments[argsProvided - 1] = array;
                        }
                    }
                    else
                    {
                        object?[] newArglist = new object?[argsNeeded];
                        for (int i = 0; i < argsNeeded && i < argsProvided; i++)
                            newArglist[i] = Arguments[i];

                        int length = argsProvided - argsNeeded + 1;
                        Array array = Array.CreateInstance(elementType, length);
                        for (int i = 0; i < length; i++)
                            array.SetValue(Arguments[argsNeeded + i - 1], i);

                        newArglist[argsNeeded - 1] = array;
                        Arguments = newArglist;
                        argsProvided = argsNeeded;
                    }
                }
            }

            // Special handling for optional parameters
            if (argsProvided < argsNeeded)
            {
                var newArgList = new object?[parameters.Length];
                Array.Copy(Arguments, newArgList, Arguments.Length);

                // Fill with Type.Missing for remaining parameters that are optional
                for (var i = Arguments.Length; i < parameters.Length; i++)
                {
                    if (parameters[i].IsOptional)
                    {
                        newArgList[i] = Type.Missing;
                    }
                    else
                    {
                        throw new TargetParameterCountException($"Method requires {argsNeeded} arguments but only {argsProvided} were supplied");
                    }
                }
                Arguments = newArgList;
            }

            // Special handling when sole argument is an object[]
            if (argsNeeded == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
            {
                if (argsProvided > 1 ||
                    argsProvided == 1 && Arguments[0]?.GetType() != typeof(object[]))
                {
                    Arguments = new object[] { Arguments };
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
