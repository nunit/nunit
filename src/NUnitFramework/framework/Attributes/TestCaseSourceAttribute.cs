// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework
{
    /// <summary>
    /// Indicates the source to be used to provide test fixture instances for a test class.
    /// <list>
    /// <listheader>The name parameter is a <see cref="string"/> representing the name of the source used to provide test cases. It has the following characteristics:</listheader>
    /// <item>It must be a static field, property, or method in the same class as the test case.</item>
    /// <item>It must return an <see cref="IEnumerable"/> or a type that implements <see cref="IEnumerable"/>, such as an array, a <c>List</c>, or your own iterator.</item>
    /// <item>Each item returned by the enumerator must be compatible with the signature of the method on which the attribute appears.</item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new();

        #region Constructors

        /// <summary>
        /// Construct with the name of the method, property or field that will provide data
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestCaseSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        /// <param name="methodParams">A set of parameters passed to the method, works only if the Source Name is a method.
        ///                     If the source name is a field or property has no effect.</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName, object?[]? methodParams)
        {
            MethodParams = methodParams;
            SourceType = sourceType;
            SourceName = sourceName;
        }
        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName)
        {
            SourceType = sourceType;
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a name
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        /// <param name="methodParams">A set of parameters passed to the method, works only if the Source Name is a method.
        ///                     If the source name is a field or property has no effect.</param>
        public TestCaseSourceAttribute(string sourceName, object?[]? methodParams)
        {
            SourceName = sourceName;
            MethodParams = methodParams;
        }

        /// <summary>
        /// Construct with a Type
        /// </summary>
        /// <param name="sourceType">The type that will provide data</param>
        public TestCaseSourceAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }

        #endregion

        #region Properties
        /// <summary>
        /// A set of parameters passed to the method, works only if the Source Name is a method.
        /// If the source name is a field or property has no effect.
        /// </summary>
        public object?[]? MethodParams { get; }

        /// <summary>
        /// The name of a the method, property or field to be used as a source
        /// </summary>
        public string? SourceName { get; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type? SourceType { get; }

        /// <summary>
        /// Gets or sets the category associated with every fixture created from
        /// this attribute. May be a single category or a comma-separated list.
        /// </summary>
        public string? Category { get; set; }

        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
        {
            int count = 0;

            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                count++;
                yield return _builder.BuildTestMethod(method, suite, parms);
            }

            // If count > 0, error messages will be shown for each case
            // but if it's 0, we need to add an extra "test" to show the message.
            if (count == 0 && method.GetParameters().Length == 0)
            {
                var parms = new TestCaseParameters();
                parms.RunState = RunState.NotRunnable;
                parms.Properties.Set(PropertyNames.SkipReason, "TestCaseSourceAttribute may not be used on a method without parameters");

                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        #endregion

        #region Helper Methods

        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            List<ITestCaseData> data = new();

            try
            {
                IEnumerable? source = ContextUtils.DoIsolated(() => GetTestCaseSource(method));

                if (source is not null)
                {
                    foreach (object? item in source)
                    {
                        // First handle two easy cases:
                        // 1. Source is null. This is really an error but if we
                        //    throw an exception we simply get an invalid fixture
                        //    without good info as to what caused it. Passing a
                        //    single null argument will cause an error to be
                        //    reported at the test level, in most cases.
                        // 2. User provided an ITestCaseData and we just use it.
                        ITestCaseData? parms = item is null
                            ? new TestCaseParameters([null])
                            : item as ITestCaseData;

                        try
                        {
                            if (parms is null)
                            {
                                object?[]? args = null;

                                // 3. An array was passed, it may be an object[]
                                //    or possibly some other kind of array, which
                                //    TestCaseSource can accept.
                                if (item is Array array)
                                {
                                    // If array has the same number of elements as parameters
                                    // and it does not fit exactly into single existing parameter
                                    // we believe that this array contains arguments, not is a bare
                                    // argument itself.
                                    var parameters = method.GetParameters();
                                    var argsNeeded = parameters.Length;
                                    if (argsNeeded > 0 && (parameters.LastParameterIsParamsArray()
                                        || argsNeeded <= array.Length && parameters[0].ParameterType != array.GetType()))
                                    {
                                        args = new object?[array.Length];
                                        for (var i = 0; i < array.Length; i++)
                                            args[i] = array.GetValue(i);

                                        if (argsNeeded == 1 && parameters[0].ParameterType == typeof(object))
                                        {
                                            // wrap the raw array so that it can be passed as expected
                                            args = [args];
                                        }
                                    }
                                }

                                parms = new TestCaseParameters(args ?? [item]);
                            }

                            if (parms is TestCaseParameters tcParms && parms.RunState == RunState.Runnable)
                            {
                                tcParms.AdjustArgumentsForMethod(method);
                            }
                        }
                        catch (Exception ex)
                        {
                            parms = new TestCaseParameters(ex);
                        }

                        if (Category is not null)
                        {
                            foreach (string cat in Category.Tokenize(','))
                                parms.Properties.Add(PropertyNames.Category, cat);
                        }

                        data.Add(parms);
                    }
                }
                else
                {
                    data.Clear();
                    data.Add(new TestCaseParameters(new Exception("The test case source could not be found.")));
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestCaseParameters(ex));
            }

            return data;
        }

        private IEnumerable? GetTestCaseSource(IMethodInfo method)
        {
            Type sourceType = SourceType ?? method.TypeInfo.Type;

            // Handle Type implementing IEnumerable separately
            if (SourceName is null)
                return Reflect.Construct(sourceType, null) as IEnumerable;

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field is not null)
                {
                    return field.IsStatic
                        ? (MethodParams is null ? (IEnumerable?)field.GetValue(null)
                                                : ReturnErrorAsParameter(ParamGivenToField))
                        : ReturnErrorAsParameter(SourceMustBeStatic);
                }

                var property = member as PropertyInfo;
                if (property is not null)
                {
                    MethodInfo? getMethod = property.GetGetMethod(true);
                    return getMethod?.IsStatic is true
                        ? (MethodParams is null ? (IEnumerable?)property.GetValue(null, null)
                                                : ReturnErrorAsParameter(ParamGivenToProperty))
                        : ReturnErrorAsParameter(SourceMustBeStatic);
                }

                var m = member as MethodInfo;
                if (m is not null)
                {
                    return m.IsStatic
                        ? (MethodParams is null || m.GetParameters().Length == MethodParams.Length
                            ? AsyncEnumerableAdapter.CoalesceToEnumerable(m.InvokeMaybeAwait<object>(MethodParams))
                            : ReturnErrorAsParameter(NumberOfArgsDoesNotMatch))
                        : ReturnErrorAsParameter(SourceMustBeStatic);
                }
            }

            return null;
        }

        private static IEnumerable ReturnErrorAsParameter(string errorMessage)
        {
            var parms = new TestCaseParameters();
            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, errorMessage);
            return new[] { parms };
        }

        private const string SourceMustBeStatic =
            "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";
        private const string ParamGivenToField = "You have specified a data source field but also given a set of parameters. Fields cannot take parameters, " +
                                                 "please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove " +
                                                 "it or specify a method.";
        private const string ParamGivenToProperty = "You have specified a data source property but also given a set of parameters. " +
                                                    "Properties cannot take parameters, please revise the 3rd parameter passed to the " +
                                                    "TestCaseSource attribute and either remove it or specify a method.";
        private const string NumberOfArgsDoesNotMatch = "You have given the wrong number of arguments to the method in the TestCaseSourceAttribute" +
                                                        ", please check the number of parameters passed in the object is correct in the 3rd parameter for the " +
                                                        "TestCaseSourceAttribute and this matches the number of parameters in the target method and try again.";

        #endregion
    }
}
