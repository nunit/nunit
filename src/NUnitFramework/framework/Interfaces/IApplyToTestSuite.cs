// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IApplyToTestSuite interface is implemented by self-applying
    /// attributes that modify the state of a test suite in some way.
    /// </summary>
    public interface IApplyToTestSuite
    {
        /// <summary>
        /// Modifies a test suite as defined for the specific attribute.
        /// </summary>
        /// <param name="testSuite">The test to modify</param>
        void ApplyToTestSuite(TestSuite testSuite);
    }
}
