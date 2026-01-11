// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            Arguments = arguments is null ? [null] : arguments;
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a single argument
        /// </summary>
        /// <param name="arg"></param>
        public TestCaseAttribute(object? arg)
        {
            RunState = RunState.Runnable;
            Arguments = [arg];
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public TestCaseAttribute(object? arg1, object? arg2)
        {
            RunState = RunState.Runnable;
            Arguments = [arg1, arg2];
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public TestCaseAttribute(object? arg1, object? arg2, object? arg3)
        {
            RunState = RunState.Runnable;
            Arguments = [arg1, arg2, arg3];
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
                ArgumentNullException.ThrowIfNull(value);
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
                ArgumentNullException.ThrowIfNull(value);
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
                ArgumentNullException.ThrowIfNull(value);
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
                ArgumentNullException.ThrowIfNull(value);
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
                ArgumentNullException.ThrowIfNull(value);
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
                ArgumentNullException.ThrowIfNull(value);
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
        /// Get or set the type arguments for a generic test method.
        /// If not set explicitly, the generic types will be inferred
        /// based on the test case parameters.
        /// </summary>
        public Type[]? TypeArgs { get; set; } = null;

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
                ArgumentNullException.ThrowIfNull(value);

                foreach (string cat in value.Tokenize(','))
                    Properties.Add(PropertyNames.Category, cat);
            }
        }

        /// <summary>
        /// Gets and sets the ignore until date for this test case.
        /// </summary>
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
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
            try
            {
                var parms = new TestCaseParameters(this)
                {
                    TypeArgs = TypeArgs,
                };

                parms.AdjustArgumentsForMethod(method);

                return parms;
            }
            catch (Exception ex)
            {
                return new TestCaseParameters(ex);
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

#if NET6_0_OR_GREATER // Although this compiles for .NET Framework, it fails at runtime with a NotSupportedException : Generic types are not valid.

#pragma warning disable CS3015 // Type has no accessible constructors which use only CLS-compliant types

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute<T> : TestCaseAttribute
    {
        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// </summary>
        public TestCaseAttribute(T argument)
            : base([argument])
        {
            TypeArgs = [typeof(T)];
        }
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute<T1, T2> : TestCaseAttribute
    {
        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// </summary>
        public TestCaseAttribute(T1 argument1, T2 argument2)
            : base([argument1, argument2])
        {
            TypeArgs = [typeof(T1), typeof(T2)];
        }
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute<T1, T2, T3> : TestCaseAttribute
    {
        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// </summary>
        public TestCaseAttribute(T1 argument1, T2 argument2, T3 argument3)
            : base([argument1, argument2, argument3])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3)];
        }
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute<T1, T2, T3, T4> : TestCaseAttribute
    {
        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// </summary>
        public TestCaseAttribute(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
            : base([argument1, argument2, argument3, argument4])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
        }
    }

    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute<T1, T2, T3, T4, T5> : TestCaseAttribute
    {
        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// </summary>
        public TestCaseAttribute(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
            : base([argument1, argument2, argument3, argument4, argument5])
        {
            TypeArgs = [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)];
        }
    }

#pragma warning restore CS3015 // Type has no accessible constructors which use only CLS-compliant types
#endif
}
