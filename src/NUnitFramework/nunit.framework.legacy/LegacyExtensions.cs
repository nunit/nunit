// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// Extensions to move legacy code into NUnit.Framework namespace
    /// </summary>
    public static class LegacyExtensions
    {
        /// <summary>
        ///
        /// </summary>
        extension(Assert)
        {
            /// <summary>
            /// Asserts that two integer values are equal. If they are not, throws an assertion exception with the
            /// specified message.
            /// </summary>
            /// <param name="expected">The expected integer value.</param>
            /// <param name="actual">The actual integer value to compare to the expected value.</param>
            /// <param name="message">The message to include in the exception if the assertion fails.</param>
            public static void AreEqual(int expected, int actual, string message) => Legacy.ClassicAssert.AreEqual(expected, actual, message);
        }
    }
}
