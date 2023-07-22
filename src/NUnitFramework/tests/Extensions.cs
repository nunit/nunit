// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

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
            if (result is null) throw new ArgumentNullException(nameof(result));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), (NUnitString)result.Message);
        }
    }
}
