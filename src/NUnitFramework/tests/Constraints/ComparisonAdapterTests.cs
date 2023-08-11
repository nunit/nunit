// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    public static class ComparisonAdapterTests
    {
        private static IEnumerable<ComparisonAdapter> ComparisonAdapters()
        {
            return new[]
            {
                ComparisonAdapter.Default,
                ComparisonAdapter.For((IComparer)StringComparer.Ordinal),
                ComparisonAdapter.For((IComparer<string>)StringComparer.Ordinal),
                ComparisonAdapter.For<string>(StringComparer.Ordinal.Compare)
            };
        }

        [TestCaseSource(nameof(ComparisonAdapters))]
        public static void CanCompareWithNull(ComparisonAdapter adapter)
        {
            Assert.Multiple(() =>
            {
                Assert.That(adapter.Compare(null, "a"), Is.LessThan(0));
                Assert.That(adapter.Compare("a", null), Is.GreaterThan(0));
                Assert.That(adapter.Compare(null, null), Is.Zero);
            });
        }
    }
}
