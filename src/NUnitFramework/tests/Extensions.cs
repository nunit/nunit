// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    /// <summary>
    /// Contains this assembly's general extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Asserts that the result state has <see cref="TestStatus.Passed"/>.
        /// </summary>
        public static void AssertPassed(this ITestResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), result.Message);
        }

        /// <summary>
        /// Gets the result message for a failed <see cref="ConstraintResult"/>.
        /// </summary>
        public static string GetFailureMessage(this ConstraintResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot get failure message for a successful result.");

            using var writer = new TextMessageWriter();
            result.WriteMessageTo(writer);

            return writer.ToString();
        }
    }
}
