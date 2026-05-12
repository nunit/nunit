// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class ValueSourceFixture
    {
        public static readonly string ValidSource = "Rumpelstilzchen";
        private static readonly string? NullSource;

        public static IEnumerable<int>? NullDataSourceProperty => null;
        private static IEnumerable<int>? NullDataSourceProvider()
        {
            return null;
        }

        public class ValueProvider
        {
            public static IEnumerable<int> IntegerProvider()
            {
                return new List<int>() { 1, 2, 4, 8 };
            }

            public static IEnumerable<int>? ForeignNullResultProvider()
            {
                return null;
            }
        }

        [Test]
        public void Valid([ValueSource(nameof(ValidSource))] char sourceItem)
        {
            Assert.That(ValidSource, Does.Contain(sourceItem));
        }

        [Test]
        public void UsingNullSources(
            [ValueSource(nameof(NullSource))] string nullSource,
            [ValueSource(nameof(NullDataSourceProvider))] string nullDataSourceProvided,
            [ValueSource(typeof(ValueProvider), nameof(ValueProvider.ForeignNullResultProvider))] string nullDataSourceProvider,
            [ValueSource(typeof(object), sourceName: null)] string typeNotImplementingIEnumerableAndNullSourceName,
            [ValueSource(nameof(NullDataSourceProperty))] int nullDataSourceProperty,
#pragma warning disable NUnit1025 // The ValueSource argument does not specify an existing member
            [ValueSource("SomeNonExistingMemberSource")] int nonExistingMember)
#pragma warning restore NUnit1025 // The ValueSource argument does not specify an existing member
        {
            Assert.Fail();
        }
    }
}
