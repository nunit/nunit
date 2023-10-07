// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ISuiteBuilder interface is exposed by a class that knows how to
    /// build a single test suite from a specified type.
    /// </summary>
    public interface ISuiteBuilder
    {
        /// <summary>
        /// Examine the type and determine if it is suitable for
        /// this builder to use in building a TestSuite.
        ///
        /// Note that returning false will cause the type to be ignored
        /// in loading the tests. If it is desired to load the suite
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="typeInfo">The type of the fixture to be used</param>
        bool CanBuildFrom(ITypeInfo typeInfo);

        /// <summary>
        /// Builds a single test suite from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type of the fixture to be used</param>
        TestSuite BuildFrom(ITypeInfo typeInfo);

        /// <summary>
        /// Builds a single test suite from the specified type, subject
        /// to a filter that decides which methods are included.
        /// </summary>
        /// <param name="typeInfo">The type of the fixture to be used</param>
        /// <param name="filter">A PreFilter for selecting methods.</param>
        TestSuite BuildFrom(ITypeInfo typeInfo, IPreFilter filter);
    }
}
