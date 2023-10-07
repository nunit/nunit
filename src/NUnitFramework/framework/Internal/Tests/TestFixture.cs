// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestFixture is a surrogate for a user test fixture class,
    /// containing one or more tests.
    /// </summary>
    public class TestFixture : TestSuite, IDisposableFixture
    {
        /// <summary>
        /// The life cycle specified for the current test fixture.
        /// </summary>
        public LifeCycle LifeCycle { get; set; }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="arguments">Arguments used to instantiate the test fixture, or null if none used</param>
        public TestFixture(ITypeInfo fixtureType, object?[]? arguments = null) : base(fixtureType, arguments)
        {
            SetUpMethods = TypeInfo.GetMethodsWithAttribute<SetUpAttribute>(true);
            TearDownMethods = TypeInfo.GetMethodsWithAttribute<TearDownAttribute>(true);
            OneTimeSetUpMethods = TypeInfo.GetMethodsWithAttribute<OneTimeSetUpAttribute>(true);
            OneTimeTearDownMethods = TypeInfo.GetMethodsWithAttribute<OneTimeTearDownAttribute>(true);

            CheckSetUpTearDownMethods(OneTimeSetUpMethods);
            CheckSetUpTearDownMethods(OneTimeTearDownMethods);
            CheckSetUpTearDownMethods(SetUpMethods);
            CheckSetUpTearDownMethods(TearDownMethods);
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="fixture">The <see cref="TestFixture"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        private TestFixture(TestFixture fixture, ITestFilter filter)
            : base(fixture, filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class that failed to load.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="ex">Exception that was thrown during test discovery.</param>
        public TestFixture(ITypeInfo fixtureType, Exception ex) : base(fixtureType, null)
        {
            MakeInvalid(ex, "Failure building TestFixture");
        }

        #endregion

        #region Test Suite Overrides

        /// <summary>
        /// Gets the TypeInfo of the fixture used in running this test.
        /// </summary>
        public new ITypeInfo TypeInfo => base.TypeInfo!;

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new TestFixture(this, filter);
        }

        #endregion
    }
}
