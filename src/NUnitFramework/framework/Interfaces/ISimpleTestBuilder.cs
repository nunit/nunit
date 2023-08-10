// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal; // TODO: We shouldn't reference this in the interface

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ISimpleTestBuilder interface is exposed by a class that knows how to
    /// build a single tests from a specified method. In general,
    /// it is exposed by an attribute, but it may be implemented in a helper class
    /// used by the attribute in some cases.
    /// </summary>
    public interface ISimpleTestBuilder
    {
        /// <summary>
        /// Builds a single test from the specified method and context.
        /// </summary>
        /// <param name="method">The method to be used as a test</param>
        /// <param name="suite">The TestSuite to which the method will be added</param>
        TestMethod BuildFrom(IMethodInfo method, Test? suite);
    }
}
