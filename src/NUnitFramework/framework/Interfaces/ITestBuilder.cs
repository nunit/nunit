// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections.Generic;

// TODO: We shouldn't reference this in the interface

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestBuilder interface is exposed by a class that knows how to
    /// build tests from a specified method. In general, it is exposed
    /// by an attribute which has additional information available to provide
    /// the necessary test parameters to distinguish the test cases built.
    /// </summary>
    public interface ITestBuilder
    {
        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The method to be used as a test</param>
        /// <param name="suite">The TestSuite to which the method will be added</param>
        IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite);
    }
}
