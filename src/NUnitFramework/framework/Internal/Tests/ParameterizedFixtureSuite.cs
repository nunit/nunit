// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedFixtureSuite serves as a container for the set of test
    /// fixtures created from a given Type using various parameters.
    /// </summary>
    public class ParameterizedFixtureSuite : TestSuite
    {
        private readonly bool _genericFixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedFixtureSuite"/> class.
        /// </summary>
        /// <param name="typeInfo">The ITypeInfo for the type that represents the suite.</param>
        public ParameterizedFixtureSuite(ITypeInfo typeInfo) : base(typeInfo.Namespace, typeInfo.GetDisplayName())
        {
            _genericFixture = typeInfo.ContainsGenericParameters;
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="suite">The <see cref="ParameterizedFixtureSuite"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public ParameterizedFixtureSuite(ParameterizedFixtureSuite suite, ITestFilter filter)
            : base(suite, filter)
        {
        }

        /// <summary>
        /// Gets a string representing the type of test
        /// </summary>
        public override string TestType
        {
            get
            {
                return _genericFixture
                    ? "GenericFixture"
                    : "ParameterizedFixture";
            }
        }

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new ParameterizedFixtureSuite(this, filter);
        }
    }
}
