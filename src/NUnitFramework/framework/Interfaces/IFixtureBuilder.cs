// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

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

    /// <summary>
    /// The IFixtureBuilderWithFilter interface extends IFixtureBuilder by allowing
    /// use of a PreFilter, which is used to select methods as test cases.
    /// </summary>
    public interface IFixtureBuilderWithFilter : IFixtureBuilder2
    {
    }

    /// <summary>
    /// The IFixtureBuilderWithParameters interface extends IFixtureBuilderWithFilter by allowing
    /// retrieval of test fixture data for parameterized test fixtures.
    /// </summary>
    public interface IFixtureBuilderWithParameters : IFixtureBuilderWithFilter
    {
        /// <summary>
        /// Returns a set of ITestFixtureData items for use as arguments
        /// to a parameterized test fixture.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <returns>Enumeration of test fixture data.</returns>
        IEnumerable<ITestFixtureData> GetFixtureData(ITypeInfo typeInfo);
    }

    /// <summary>
    /// The IFixtureBuilderForNestedGeneric interface extends IFixtureBuilderWithFilter by allowing
    /// use of an outer builder's data, which is used to supply type arguments to nested generic classes.
    /// </summary>
    public interface IFixtureBuilderForNestedGeneric : IFixtureBuilderWithParameters
    {
        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <param name="filter">PreFilter to be used to select methods.</param>
        /// <param name="outerTypeArgsSets">Sets of Type Arguments for the outer fixture to be used for nested generic classes.</param>
        IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter, Type[]?[] outerTypeArgsSets);
    }
}
