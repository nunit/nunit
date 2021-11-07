// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Interface to be implemented by filters applied to tests.
    /// The filter applies when running the test, after it has been
    /// loaded, since this is the only time an ITest exists.
    /// </summary>
    public interface ITestFilter : IXmlNodeBuilder
    {
        /// <summary>
        /// Determine if a particular test passes the filter criteria. Pass
        /// may examine the parents and/or descendants of a test, depending
        /// on the semantics of the particular filter
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        bool Pass(ITest test);

        /// <summary>
        /// Determine if a test matches the filter explicitly. That is, it must
        /// be a direct match of the test itself or one of its children.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test matches the filter explicitly, otherwise false</returns>
        bool IsExplicitMatch(ITest test);
    }
}
