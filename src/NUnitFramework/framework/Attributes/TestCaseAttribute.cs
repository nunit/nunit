// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute : NUnitAttribute, ITestBuilder, ITestCaseData, IImplyFixture
    {
        #region Constructors

        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// This constructor is not CLS-Compliant
        /// </summary>
        /// <param name="arguments"></param>
        public TestCaseAttribute(params object?[]? arguments)
        {
            RunState = RunState.Runnable;

            if (arguments is null)
                Arguments = new object?[] { null };
            else
                Arguments = arguments;

            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a single argument
        /// </summary>
        /// <param name="arg"></param>
        public TestCaseAttribute(object? arg)
        {
            RunState = RunState.Runnable;
            Arguments = new[] { arg };
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public TestCaseAttribute(object? arg1, object? arg2)
        {
            RunState = RunState.Runnable;
            Arguments = new[] { arg1, arg2 };
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public TestCaseAttribute(object? arg1, object? arg2, object? arg3)
        {
            RunState = RunState.Runnable;
            Arguments = new[] { arg1, arg2, arg3 };
            Properties = new PropertyBag();
        }

        #endregion

        #region ITestData Members

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>The name of the test.</value>
        public string? TestName { get; set; }

        /// <summary>
        /// Gets or sets the RunState of this test case.
        /// </summary>
        public RunState RunState { get; private set; }

        /// <summary>
        /// Gets the list of arguments to a test case
        /// </summary>
        public object?[] Arguments { get; }

        /// <summary>
        /// Gets the properties of the test case
        /// </summary>
        public IPropertyBag Properties { get; }

        #endregion

        #region ITestCaseData Members

        /// <summary>
        /// Gets or sets the expected result.
        /// </summary>
        /// <value>The result.</value>
        public object? ExpectedResult
        {
            get => _expectedResult;
            set
            {
                _expectedResult = value;
                HasExpectedResult = true;
            }
        }
        private object? _expectedResult;

        /// <summary>
        /// Returns true if the expected result has been set
        /// </summary>
        public bool HasExpectedResult { get; private set; }

        #endregion

        #region Instance Fields

        private RunState _originalRunState;
        private DateTimeOffset? _untilDate;

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DisallowNull]
        public string? Description
        {
            get => Properties.Get(PropertyNames.Description) as string;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                Properties.Set(PropertyNames.Description, value);
            }
        }

        /// <summary>
        /// The author of this test
        /// </summary>
        [DisallowNull]
        public string? Author
        {
            get => Properties.Get(PropertyNames.Author) as string;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                Properties.Set(PropertyNames.Author, value);
            }
        }

        /// <summary>
        /// The type that this test is testing
        /// </summary>
        [DisallowNull]
        public Type? TestOf
        {
            get => _testOf;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                _testOf = value;
                Properties.Set(PropertyNames.TestOf, value.FullName());
            }
        }
        private Type? _testOf;

        /// <summary>
        /// Gets or sets the reason for ignoring the test
        /// </summary>
        [DisallowNull]
        public string? Ignore
        {
            get => IgnoreReason;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                IgnoreReason = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NUnit.Framework.TestCaseAttribute"/> is explicit.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if explicit; otherwise, <see langword="false"/>.
        /// </value>
        public bool Explicit
        {
            get => RunState == RunState.Explicit;
            set => RunState = value ? RunState.Explicit : RunState.Runnable;
        }

        /// <summary>
        /// Gets or sets the reason for not running the test.
        /// </summary>
        /// <value>The reason.</value>
        [DisallowNull]
        public string? Reason
        {
            get => Properties.Get(PropertyNames.SkipReason) as string;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                Properties.Set(PropertyNames.SkipReason, value);
            }
        }

        /// <summary>
        /// Gets or sets the ignore reason. When set to a non-null
        /// non-empty value, the test is marked as ignored.
        /// </summary>
        /// <value>The ignore reason.</value>
        [DisallowNull]
        public string? IgnoreReason
        {
            get => Reason;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                _originalRunState = RunState;
                RunState = RunState.Ignored;
                Reason = value;
            }
        }

        /// <summary>
        /// Comma-delimited list of platforms to run the test for
        /// </summary>
        public string? IncludePlatform { get; set; }

        /// <summary>
        /// Comma-delimited list of platforms to not run the test for
        /// </summary>
        public string? ExcludePlatform { get; set; }

        /// <summary>
        /// Gets and sets the category for this test case.
        /// May be a comma-separated list of categories.
        /// </summary>
        [DisallowNull]
        public string? Category
        {
            get => Properties.Get(PropertyNames.Category) as string;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));

                foreach (string cat in value.Tokenize(','))
                    Properties.Add(PropertyNames.Category, cat);
            }
        }

        /// <summary>
        /// Gets and sets the ignore until date for this test case.
        /// </summary>
#if NET7_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.DateTimeFormat)]
#endif
        [DisallowNull]
        public string? Until
        {
            get => Properties.Get(PropertyNames.IgnoreUntilDate) as string;
            set
            {
                if (!string.IsNullOrEmpty(IgnoreReason))
                {
                    _untilDate = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    Properties.Set(PropertyNames.IgnoreUntilDate, _untilDate.Value.ToString("u"));
                }
                else
                {
                    RunState = RunState.NotRunnable;
                }
            }
        }

        #endregion

        #region Helper Methods

        private TestCaseParameters GetParametersForTestCase(IMethodInfo method)
        {
            TestCaseParameters parms;

            try
            {
                IParameterInfo[] parameters = method.GetParameters();
                int argsNeeded = parameters.Length;
                int argsProvided = Arguments.Length;

                parms = new TestCaseParameters(this);

                // Special handling for ExpectedResult (see if it needs to be converted into method return type)
                if (parms.HasExpectedResult
                    && ParamAttributeTypeConversions.TryConvert(parms.ExpectedResult, method.ReturnType.Type, out var expectedResultInTargetType))
                {
                    parms.ExpectedResult = expectedResultInTargetType;
                }

                // Special handling for params arguments
                if (argsNeeded > 0 && argsProvided >= argsNeeded - 1)
                {
                    IParameterInfo lastParameter = parameters[argsNeeded - 1];
                    Type lastParameterType = lastParameter.ParameterType;
                    Type elementType = lastParameterType.GetElementType()!;

                    if (lastParameterType.IsArray && lastParameter.IsDefined<ParamArrayAttribute>(false))
                    {
                        if (argsProvided == argsNeeded)
                        {
                            if (!lastParameterType.IsInstanceOfType(parms.Arguments[argsProvided - 1]))
                            {
                                Array array = Array.CreateInstance(elementType, 1);
                                array.SetValue(parms.Arguments[argsProvided - 1], 0);
                                parms.Arguments[argsProvided - 1] = array;
                            }
                        }
                        else
                        {
                            object?[] newArglist = new object?[argsNeeded];
                            for (int i = 0; i < argsNeeded && i < argsProvided; i++)
                                newArglist[i] = parms.Arguments[i];

                            int length = argsProvided - argsNeeded + 1;
                            Array array = Array.CreateInstance(elementType, length);
                            for (int i = 0; i < length; i++)
                                array.SetValue(parms.Arguments[argsNeeded + i - 1], i);

                            newArglist[argsNeeded - 1] = array;
                            parms.Arguments = newArglist;
                            argsProvided = argsNeeded;
                        }
                    }
                }

                //Special handling for optional parameters
                if (parms.Arguments.Length < argsNeeded)
                {
                    var newArgList = new object?[parameters.Length];
                    Array.Copy(parms.Arguments, newArgList, parms.Arguments.Length);

                    //Fill with Type.Missing for remaining required parameters where optional
                    for (var i = parms.Arguments.Length; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOptional)
                        {
                            newArgList[i] = Type.Missing;
                        }
                        else
                        {
                            if (i < parms.Arguments.Length)
                                newArgList[i] = parms.Arguments[i];
                            else
                                throw new TargetParameterCountException($"Method requires {argsNeeded} arguments but TestCaseAttribute only supplied {argsProvided}");
                        }
                    }
                    parms.Arguments = newArgList;
                }

                // Special handling when sole argument is an object[]
                if (argsNeeded == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
                {
                    if (argsProvided > 1 ||
                        argsProvided == 1 && parms.Arguments[0]?.GetType() != typeof(object[]))
                    {
                        parms.Arguments = new object[] { parms.Arguments };
                    }
                }

                if (argsProvided == argsNeeded)
                    PerformSpecialConversions(parms.Arguments, parameters);
            }
            catch (Exception ex)
            {
                parms = new TestCaseParameters(ex);
            }

            return parms;
        }

        /// <summary>
        /// Performs several special conversions allowed by NUnit in order to
        /// permit arguments with types that cannot be used in the constructor
        /// of an Attribute such as TestCaseAttribute or to simplify their use.
        /// </summary>
        /// <param name="arglist">The arguments to be converted</param>
        /// <param name="parameters">The ParameterInfo array for the method</param>
        private static void PerformSpecialConversions(object?[] arglist, IParameterInfo[] parameters)
        {
            for (int i = 0; i < arglist.Length; i++)
            {
                object? arg = arglist[i];
                Type targetType = parameters[i].ParameterType;
                if (ParamAttributeTypeConversions.TryConvert(arg, targetType, out var argAsTargetType))
                {
                    arglist[i] = argAsTargetType;
                }
            }
        }
        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Builds a single test from the specified method and context.
        /// </summary>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
        {
            TestMethod test = new NUnitTestCaseBuilder().BuildTestMethod(method, suite, GetParametersForTestCase(method));

            if (_untilDate.HasValue)
            {
                if (_untilDate > DateTimeOffset.UtcNow)
                {
                    test.RunState = RunState.Ignored;
                    string reason = $"Ignoring until {_untilDate.Value:u}. {IgnoreReason}";
                    test.Properties.Set(PropertyNames.SkipReason, reason);
                }
                else
                {
                    test.RunState = _originalRunState;
                }
            }

            if (IncludePlatform is not null || ExcludePlatform is not null)
            {
                if (test.RunState is RunState.NotRunnable or RunState.Ignored)
                {
                    yield return test;
                    yield break;
                }

                var platformHelper = new PlatformHelper();

                if (!platformHelper.IsPlatformSupported(this))
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, platformHelper.Reason);
                }
            }

            yield return test;
        }

        #endregion
    }
}
