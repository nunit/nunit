// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IApplyToTest interface is implemented by self-applying
    /// attributes that modify the state of a test in some way.
    /// </summary>
    public interface IApplyToTest
    {
        /// <summary>
        /// Modifies a test as defined for the specific attribute.
        /// </summary>
        /// <param name="test">The test to modify</param>
        void ApplyToTest(Test test);
    }
}
