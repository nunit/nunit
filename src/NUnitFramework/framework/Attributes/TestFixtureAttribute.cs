// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks the class as a TestFixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class TestFixtureAttribute : NUnitAttribute, IFixtureBuilder2, ITestFixtureData
    {
        private readonly NUnitTestFixtureBuilder _builder = new NUnitTestFixtureBuilder();

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestFixtureAttribute() : this(Internal.TestParameters.NoArguments) { }

        /// <summary>
        /// Construct with a object[] representing a set of arguments.
        /// The arguments may later be separated into type arguments and constructor arguments.
        /// </summary>
        /// <param name="arguments"></param>
        public TestFixtureAttribute(params object?[]? arguments)
        {
            RunState = RunState.Runnable;
            Arguments = arguments ?? new object?[] { null };
            TypeArgs = Array.Empty<Type>();
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
        /// Gets or sets the RunState of this test fixture.
        /// </summary>
        public RunState RunState { get; private set; }

        /// <summary>
        /// The arguments originally provided to the attribute
        /// </summary>
        public object?[] Arguments { get; }

        /// <summary>
        /// Properties pertaining to this fixture
        /// </summary>
        public IPropertyBag Properties { get; }

        #endregion

        #region ITestFixtureData Members

        /// <summary>
        /// Get or set the type arguments. If not set
        /// explicitly, any leading arguments that are
        /// Types are taken as type arguments.
        /// </summary>
        public Type[] TypeArgs { get; set; }

        #endregion

        #region Other Properties

        /// <summary>
        /// Descriptive text for this fixture
        /// </summary>
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
        /// The author of this fixture
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
        /// The type that this fixture is testing
        /// </summary>
        [DisallowNull]
        public Type? TestOf
        {
            get => _testOf;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                _testOf = value;
                Properties.Set(PropertyNames.TestOf, value.FullName);
            }
        }
        private Type? _testOf;

        /// <summary>
        /// Gets or sets the ignore reason. May set RunState as a side effect.
        /// </summary>
        /// <value>The ignore reason.</value>
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
        /// Gets or sets the reason for not running the fixture.
        /// </summary>
        /// <value>The reason.</value>
        [DisallowNull]
        public string? Reason
        {
            get => this.Properties.Get(PropertyNames.SkipReason) as string;
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                this.Properties.Set(PropertyNames.SkipReason, value);
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
                RunState = RunState.Ignored;
                Reason = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NUnit.Framework.TestFixtureAttribute"/> is explicit.
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
        /// Gets and sets the category for this fixture.
        /// May be a comma-separated list of categories.
        /// </summary>
        [DisallowNull]
        public string? Category
        {
            get
            {
                //return Properties.Get(PropertyNames.Category) as string;
                var catList = Properties[PropertyNames.Category];
                if (catList == null)
                    return null;

                switch (catList.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return catList[0] as string;
                    default:
                        var cats = new string[catList.Count];
                        int index = 0;
                        foreach (string cat in catList)
                            cats[index++] = cat;

                        return string.Join(",", cats);
                }
            }
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));

                foreach (string cat in value.Split(new char[] { ',' }))
                    Properties.Add(PropertyNames.Category, cat);
            }
        }

        #endregion

        #region IFixtureBuilder Members

        /// <summary>
        /// Builds a single test fixture from the specified type.
        /// </summary>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            return this.BuildFrom(typeInfo, PreFilter.Empty);
        }

        #endregion

        #region IFixtureBuilder2 Members

        /// <summary>
        /// Builds a single test fixture from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <param name="filter">Filter used to select methods as tests.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
        {
            var fixture = _builder.BuildFrom(typeInfo, filter, this);
            fixture.ApplyAttributesToTest(new AttributeProviderWrapper<FixtureLifeCycleAttribute>(typeInfo.Type.Assembly));
            fixture.ApplyAttributesToTest(typeInfo.Type);

            yield return fixture;
        }

        #endregion
    }
}
