// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestParameters is the abstract base class for all classes
    /// that know how to provide data for constructing a test.
    /// </summary>
    public abstract class TestParameters : ITestData, IApplyToTest, IAlignArguments
    {
        #region Constants
        private static readonly Array ArrayUndefined = new object[0];

        internal const string ExceptionParameterCount = "Method expects {0} arguments but test case supplied {1}.";
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor inititalizes default state.
        /// </summary>
        protected TestParameters()
        {
            RunState = RunState.Runnable;
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a parameter set with a list of <paramref name="args">arguments</paramref>.
        /// </summary>
        protected TestParameters(object[] args)
            : this()
        {
            InitializeArguments(args);
        }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying the provider
        /// <paramref name="exception">exception</paramref> that made it invalid.
        /// </summary>
        protected TestParameters(Exception exception)
            : this()
        {
            RunState = RunState.NotRunnable;

            Properties.Set(PropertyNames.SkipReason, ExceptionHelper.BuildMessage(exception));
            Properties.Set(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
        }

        /// <summary>
        /// Construct a ParameterSet from an <paramref name="data">object</paramref>
        /// implementing <see cref="ITestData"/>.
        /// </summary>
        protected TestParameters(ITestData data)
            : this()
        {
            RunState = data.RunState;
            TestName = data.TestName;

            InitializeArguments(data.Arguments);

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }

        private void InitializeArguments(object[] args)
        {
            OriginalArguments = args;

            // We need to copy args, since we may change them
            var numArgs = args.Length;
            Arguments = new object[numArgs];
            Array.Copy(args, Arguments, numArgs);
        }

        #endregion

        #region ITestData Members

        /// <summary>
        /// The RunState for this set of parameters.
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// The arguments to be used in running the test,
        /// which must match the method signature.
        /// </summary>
        public object[] Arguments { get; internal set; }

        /// <summary>
        /// A name to be used for this test case in lieu
        /// of the standard generated name containing
        /// the argument list.
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets the property dictionary for this test
        /// </summary>
        public IPropertyBag Properties { get; private set; }

        #endregion

        #region IApplyToTest Members

        /// <summary>
        /// Applies ParameterSet values to the test itself.
        /// </summary>
        /// <param name="test">A test.</param>
        public void ApplyToTest(Test test)
        {
            if (this.RunState != RunState.Runnable)
                test.RunState = this.RunState;

            foreach (string key in Properties.Keys)
            foreach (object value in Properties[key])
                test.Properties.Add(key, value);
        }

        #endregion

        #region IAlignArguments Members
        /// <inheritdoc />
        void IAlignArguments.Align(IParameterInfo[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var paramsCount = parameters.Length;
            var argsCount = Arguments.Length;

            // Special handling when sole parameter is an object[]
            if (paramsCount == 1 && parameters[0].ParameterType == typeof(object[]) && Arguments[0] != null)
            {
                if (argsCount > 1 || argsCount == 1 && Arguments[0].GetType() != typeof(object[]))
                {
                    Arguments = new object[] { Arguments };

                    return;
                }
            }

            // According to C# spec: "Only the last parameter of a method can be a parameter
            // array". Also we have CS1737: "Optional parameters must appear after all required
            // parameters". So all arguments is a sequence of three blocks: required, optional,
            // array. For easier handling we will scan it in backwards direction.
            if (paramsCount > 0)
            {
                var lastParam = parameters[paramsCount - 1];

                // If we have a param array, then write down this fact and save the last
                // non-array index.
                var hasArray = lastParam.IsDefined<ParamArrayAttribute>(false);
                var lastNonArray = paramsCount - (hasArray ? 1 : 0);

                if (!hasArray && argsCount > lastNonArray)
                {
                    throw new TargetParameterCountException(
                        string.Format(ExceptionParameterCount, lastNonArray, argsCount)
                    );
                }

                // Scan optional params in backwards directions from the last non-array to
                // the last required to figure out the optional block.
                int index = lastNonArray - 1, paramsRequired = lastNonArray;
                for (; index >= 0 && parameters[index].IsOptional; index--)
                {
                    paramsRequired = index;
                }

                // Having number of required params (w/o optional and array) we can compare it to
                // the number of provided arguments and throw exception if not enough.
                if (argsCount < paramsRequired)
                {
                    throw new TargetParameterCountException(
                        string.Format(ExceptionParameterCount, paramsRequired, argsCount)
                    );
                }

                // If number of arguments differs from number of parameters we recreate the
                // list of arguments by copying what we have. When copying we exclude the last
                // "params" argument if it exists and non-provided optional arguments.
                if (argsCount != paramsCount)
                {
                    var args = new object[paramsCount];
                    Array.Copy(Arguments, args, argsCount < lastNonArray ? argsCount : lastNonArray);
                    Arguments = args;
                }

                // Fill optional params those are after the last provided argument and before
                // parameter array if it exists.
                for (index = argsCount; index < lastNonArray; index++)
                {
                    Arguments[index] = parameters[index].ParameterInfo.DefaultValue;
                }

                // Parameter array is a subsequence in our test case arguments. So we should
                // extract it, wrap into array and provide as the last argument to the method.
                if (hasArray)
                {
                    var array = ArrayUndefined;
                    var lastParamType = lastParam.ParameterType;
                    var elementType = lastParamType.GetElementType();

                    // Specific case is when the last argument is already an array of the same
                    // type. Then there is no need to wrap and we just pass the same array.
                    // Subcase is when the last argument is null.
                    if (argsCount == paramsCount)
                    {
                        var lastArgument = OriginalArguments[argsCount - 1];
                        if (lastArgument != null)
                        {
                            var lastArgumentType = lastArgument.GetType();
                            if (lastArgumentType.IsArray)
                            {
                                if (lastParam.ParameterType.IsAssignableFrom(lastArgumentType))
                                {
                                    array = (Array)lastArgument;
                                }
                            }
                        }
                        else
                        {
                            array = null;
                        }
                    }

                    // If previously no specific cases were found, we wrapping a parameter array
                    // subsequence into array.
                    if (array == ArrayUndefined)
                    {
                        var length = argsCount - paramsCount + 1;
                        array = Array.CreateInstance(elementType, length < 0 ? 0 : length);

                        for (var iParam = paramsCount - 1; iParam < argsCount; iParam++)
                        {
                            object value = OriginalArguments[iParam];

                            if (TypeHelper.TryConvert(ref value, elementType))
                            {
                                array.SetValue(value, iParam - paramsCount + 1);
                            }
                            else
                            {
                                var paramName = lastParam.ParameterInfo.Name;

                                throw new ArgumentException(
                                    string.Format(
                                        "Argument #{0} ({1} {2}) is incompatible with parameter array \"{3}\" of {4}[]",
                                        iParam,
                                        value.GetType(),
                                        value,
                                        paramName,
                                        elementType
                                    ),
                                    paramName
                                );
                            }
                        }
                    }

                    Arguments[paramsCount - 1] = array;
                }
            }
            else if (argsCount > 0)
            {
                throw new TargetParameterCountException(
                    string.Format(ExceptionParameterCount, paramsCount, argsCount)
                );
            }

            // Perform type conversions id there are any type mismatches.
            for (var index = 0; index < paramsCount; index++)
            {
                var parameter = parameters[index];
                var parameterType = parameter.ParameterType;

                if (!TypeHelper.TryConvert(ref Arguments[index], parameterType))
                {
                    // Skip generic parameters
                    if (!parameterType.IsGenericParameter)
                    {
                        var parameterName = parameter.ParameterInfo.Name;

                        throw new ArgumentException(
                            string.Format(
                                "Argument #{0} ({1}) is incompatible with parameter \"{2}\" of {3}",
                                index,
                                Arguments[index],
                                parameterName,
                                parameter.ParameterType
                            ),
                            parameterName
                        );
                    }
                }
            }
        }
        #endregion

        #region Other Public Properties

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object[] OriginalArguments { get; private set; }

        #endregion
    }
}
