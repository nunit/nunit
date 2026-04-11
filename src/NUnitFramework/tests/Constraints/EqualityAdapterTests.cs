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

        [TestCaseSource(nameof(EqualityAdapters))]
        public static void CanCompareWithNull(EqualityAdapter adapter)
        {
            Assert.That(adapter.AreEqual(null, "a"), Is.False);
            Assert.That(adapter.AreEqual("a", null), Is.False);
            Assert.That(adapter.AreEqual(null, null), Is.True);
        }

        [TestCaseSource(nameof(EqualityAdapters))]
        public static void CanCompare(EqualityAdapter adapter)
        {
            Assert.That(adapter.AreEqual("a", "a"), Is.True);
        }
    }
}
