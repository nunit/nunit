// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Specialized;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Internal.Extensions
{
    public class IEnumerableExtensionTests
    {
        [TestCaseSource(nameof(SortableCollections))]
        public void SortableCollectionsAreSortable(IEnumerable collection)
        {
            Assert.That(collection.IsSortable(), Is.True);
        }

        [TestCaseSource(nameof(UnsortableCollections))]
        public void UnsortableCollectionsAreNotSortable(IEnumerable collection)
        {
            Assert.That(collection.IsSortable(), Is.False);
        }

        public static IEnumerable<TestCaseData> SortableCollections => new[]
        {
            new TestCaseData(new int[] { 1 }).SetArgDisplayNames("int[]"),
            new TestCaseData(new string[] { "1" }).SetArgDisplayNames("string[]"),
            new TestCaseData(Enumerable.Range(0, 10)).SetArgDisplayNames("IEnumerable<int>"),
            new TestCaseData(Enumerable.Range(0, 10).Select(n => n.ToString())).SetArgDisplayNames("IEnumerable<string>"),
            new TestCaseData(new List<int> { 1 }).SetArgDisplayNames("List<int>"),
            new TestCaseData(new List<string> { "1" }).SetArgDisplayNames("List<string>"),
            new TestCaseData(new HashSet<int> { 1 }).SetArgDisplayNames("HashSet<int>"),
            new TestCaseData(new HashSet<string> { "1" }).SetArgDisplayNames("HashSet<string>"),
            new TestCaseData(new StringCollection { "1" }).SetArgDisplayNames("StringCollection"),
        };

        public static IEnumerable<TestCaseData> UnsortableCollections => new[]
        {
            new TestCaseData(null).SetArgDisplayNames("null"),
            new TestCaseData(new ArrayList()).SetArgDisplayNames("ArrayList"),
            new TestCaseData(new Hashtable()).SetArgDisplayNames("Hashtable"),
            new TestCaseData(new Queue()).SetArgDisplayNames("Queue"),
            new TestCaseData(new ListDictionary()).SetArgDisplayNames("ListDictionary"),
            new TestCaseData(new SimpleObjectCollection()).SetArgDisplayNames("SimpleObjectCollection (ICollection)"),
        };
    }
}
