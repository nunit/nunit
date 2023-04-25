// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// SetUpFixture extends TestSuite and supports
    /// Setup and TearDown methods.
    /// </summary>
    public class SetUpFixture : TestSuite, IDisposableFixture
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpFixture"/> class.
        /// </summary>
        public SetUpFixture(ITypeInfo type) : base(type)
        {
            Name = GetName(type);

            OneTimeSetUpMethods = TypeInfo.GetMethodsWithAttribute<OneTimeSetUpAttribute>(true);
            OneTimeTearDownMethods = TypeInfo.GetMethodsWithAttribute<OneTimeTearDownAttribute>(true);

            CheckSetUpTearDownMethods(OneTimeSetUpMethods);
            CheckSetUpTearDownMethods(OneTimeTearDownMethods);
        }

        static string GetName(ITypeInfo type)
        {
            var name = type.Namespace ?? "[default namespace]";
            var index = name.LastIndexOf('.');
            if (index > 0)
                name = name.Substring(index + 1);
            return name;
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="setUpFixture">The <see cref="SetUpFixture"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public SetUpFixture(SetUpFixture setUpFixture, ITestFilter filter)
            : base(setUpFixture, filter)
        {
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
            return new SetUpFixture(this, filter);
        }

        #endregion
    }
}
