// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_CollectionTests
    {
        [Test]
        public void Equivalence_and_Subset()
        {
            var expected = new[] { 1, 2, 3 };
            var actual = new List<int> { 3, 2, 1 };

            Assert.AreEquivalent(expected, actual);
            Assert.IsSubsetOf(new[] { 1, 2 }, actual);
        }

        [Test]
        public void Contains_and_DoesNotContain()
        {
            var data = new[] { 1, 2, 3 }; // arrays are IList/ICollection at runtime
            Assert.Contains(2, data);
            Assert.DoesNotContain(data, 4);
        }

        [Test]
        public void Ordered()
        {
            Assert.IsOrdered(new[] { 1, 2, 3 });
        }
    }
}
