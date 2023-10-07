// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    public static class EqualityAdapterTests
    {
        private static IEnumerable<EqualityAdapter> EqualityAdapters()
        {
            return new[]
            {
                EqualityAdapter.For((IEqualityComparer)StringComparer.Ordinal),
                EqualityAdapter.For((IEqualityComparer<string>)StringComparer.Ordinal),
                EqualityAdapter.For<string, string>(StringComparer.Ordinal.Equals),
                EqualityAdapter.For((IComparer)StringComparer.Ordinal),
                EqualityAdapter.For((IComparer<string>)StringComparer.Ordinal),
                EqualityAdapter.For<string>(StringComparer.Ordinal.Compare)
            };
        }

        // The NUnitFramework will never calls these with 'null'.
        // NUnitEqualityComparer.AreEqual will test for 'null' first before doing more tests.
        [TestCaseSource(nameof(EqualityAdapters))]
        public static void CanCompareWithNull(EqualityAdapter adapter)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.That(adapter.AreEqual(null, "a"), Is.False);
            Assert.That(adapter.AreEqual("a", null), Is.False);
            Assert.That(adapter.AreEqual(null, null), Is.True);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestCaseSource(nameof(EqualityAdapters))]
        public static void CanCompare(EqualityAdapter adapter)
        {
            Assert.That(adapter.AreEqual("a", "a"), Is.True);
        }
    }
}
