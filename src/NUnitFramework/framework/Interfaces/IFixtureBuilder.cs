// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    // TODO: These methods should really return IEnumerable<TestFixture>,
    // but that requires changes to the Test hierarchy.

    /// <summary>
    /// The IFixtureBuilder interface is exposed by a class that knows how to
    /// build test fixtures from a specified type. In general, it is exposed
    /// by an attribute, but it may be implemented in a helper class used by the
    /// attribute in some cases.
    /// </summary>
    public interface IFixtureBuilder
    {
        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo);
    }

    /// <summary>
    /// The IFixtureBuilder2 interface extends IFixtureBuilder by allowing
    /// use of a PreFilter, which is used to select methods as test cases.
    /// </summary>
    public interface IFixtureBuilder2 : IFixtureBuilder
    {
        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <param name="filter">PreFilter to be used to select methods.</param>
        IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter);
    }
}
