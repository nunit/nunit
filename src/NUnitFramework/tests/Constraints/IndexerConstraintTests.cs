// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class IndexerConstraintTests
    {
        private static readonly string NL = Environment.NewLine;

        [Test]
        public void CanMatchIndexerEquality()
        {
            var tester = new IndexerTester();

            Assert.That(tester, Has.ItemAt(42).EqualTo("Answer to the Ultimate Question of Life, the Universe, and Everything"));
            Assert.That(tester, Has.ItemAt(41).EqualTo("Still calculating").And.ItemAt(42).EqualTo("Answer to the Ultimate Question of Life, the Universe, and Everything"));

            Assert.That(tester, Has.ItemAt(string.Empty).EqualTo("Second indexer"));
            Assert.That(tester, Has.ItemAt(1, 2).EqualTo("Third indexer"));
            Assert.That(tester, Has.No.ItemAt(string.Empty).EqualTo("Third indexer"));
        }

        [Test]
        public void IndexerOperatorOnRightSideOfAndOperator()
        {
            var items = new ClassWithName[]
            {
                new("Name 1"),
            };

            Assert.That(items, Has.Exactly(1).Items
                                  .And.ItemAt(0).Property(nameof(ClassWithName.Name)).EqualTo("Name 1"));
        }

        [Test]
        public void CanMatchArrayEquality()
        {
            var tester = new[] { 1, 2, 3 };

            Assert.That(tester, Has.ItemAt(0).EqualTo(1));
            Assert.That(tester, Has.ItemAt(2).EqualTo(3));
        }

        [Test]
        public void CanMatchArrayWithMultiDimensionsEquality()
        {
            var tester = new[,,,] {
                { { { 1 }, { 2 }, { 3 } }, { { 4 }, { 5 }, { 6 } } },
                { { { 7 }, { 8 }, { 9 } }, { { 10 }, { 11 }, { 12 } } }
            };

            Assert.That(tester, Has.ItemAt(0, 0, 0, 0).EqualTo(1));
            Assert.That(tester, Has.ItemAt(1, 1, 2, 0).EqualTo(12));
        }

        [Test]
        public void DoesNotMatchMissingIndexerEquality()
        {
            var expectedErrorMessage = $"  Expected string length 14 but was 13. Strings differ at index 0.{NL}  Expected: \"Second indexer\"{NL}  But was:  \"Third indexer\"{NL}  -----------^{NL}";

            var tester = new IndexerTester();

            var ex = Assert.Throws<AssertionException>(() => Assert.That(tester, Has.ItemAt(4, 2).EqualTo("Second indexer")));
            Assert.That(ex?.Message, Does.Contain(expectedErrorMessage));
        }

        [Test]
        public void DoesNotMatchWhenIndexerValueIsNotExpectedToBeEqual()
        {
            var expectedErrorMessage = $"  Expected: not Default indexer accepting arguments < <string.Empty> > equal to \"Second indexer\"{NL}  But was:  \"Second indexer\"{NL}";

            var tester = new IndexerTester();

            var ex = Assert.Throws<AssertionException>(() => Assert.That(tester, Has.No.ItemAt(string.Empty).EqualTo("Second indexer")));
            Assert.That(ex?.Message, Does.Contain(expectedErrorMessage));
        }

        [Test]
        public void DoesNotMatchWhenIndexerIsNotExpectedToBeEqual()
        {
            var expectedErrorMessage = "Default indexer accepting arguments < 21.0d > was not found on NUnit.Framework.Tests.Constraints.IndexerConstraintTests+IndexerTester.";

            var tester = new IndexerTester();

            var ex = Assert.Throws<ArgumentException>(() => Assert.That(tester, Has.No.ItemAt(21d).EqualTo("Should Throw")));
            Assert.That(ex?.Message, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void CanMatchGenericIndexer()
        {
            var tester = new GenericIndexerTester<int>(100);

            Assert.That(tester, Has.ItemAt(42).EqualTo(100));
        }

        [Test]
        public void CanMatchNamedIndexer()
        {
            var tester = new NamedIndexTester();

            Assert.That(tester, Has.ItemAt(42).EqualTo("A Named Int Indexer"));
            Assert.That(tester, Has.ItemAt(42, 43).EqualTo("A Named Int Int Indexer"));
        }

        [Test]
        public void CanMatchHidingIndexer()
        {
            var tester = new DerivedClassWithoutIndexer();

            Assert.That(tester, Has.ItemAt(string.Empty).EqualTo("New value for Second indexer"));
            Assert.That(tester, Has.ItemAt(1, 2).EqualTo("New value for Third indexer"));
            Assert.That(tester, Has.ItemAt(41).EqualTo("Still calculating"));
        }

        [Test]
        public void CanMatchHidingNamedIndexer()
        {
            var tester = new DerivedClassWithoutNamedIndexer();

            Assert.That(tester, Has.ItemAt(42).EqualTo("New value for Named Int Indexer"));
            Assert.That(tester, Has.ItemAt(42, 43).EqualTo("A Named Int Int Indexer"));
        }

        private class IndexerTester
        {
            public string this[int x] => x == 42 ? "Answer to the Ultimate Question of Life, the Universe, and Everything" : "Still calculating";

            public string this[string x] => "Second indexer";

            protected string this[int x, int y] => "Third indexer";
        }

        private class ClassHidingBaseIndexer : IndexerTester
        {
            public new string this[string x] => "New value for Second indexer";

            protected new string this[int x, int y] => "New value for Third indexer";

            private new string this[int x] => "Should not use this as private members can't be shadowed";
        }

        private class DerivedClassWithoutIndexer : ClassHidingBaseIndexer
        {
        }

        private class GenericIndexerTester<T>
        {
            private readonly T _item;

            public int DummyParam { get; set; }

            public GenericIndexerTester(T item)
            {
                _item = item;
            }

            public T this[int x] => _item;
        }

        private class NamedIndexTester
        {
            [IndexerName("ANamedIndexer")]
            public string this[int x] => "A Named Int Indexer";

            [IndexerName("ANamedIndexer")]
            public string this[int x, int y] => "A Named Int Int Indexer";
        }

        private class ClassHidingBaseNamedIndexer : NamedIndexTester
        {
            [IndexerName("ANamedIndexer")]
            public new string this[int x] => "New value for Named Int Indexer";
        }

        private class DerivedClassWithoutNamedIndexer : ClassHidingBaseNamedIndexer
        {
        }

        private class ClassWithName
        {
            public ClassWithName(string name)
            {
                Name = name;
            }
            public string Name { get; }
        }
    }
}
