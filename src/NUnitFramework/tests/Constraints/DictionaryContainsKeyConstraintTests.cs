// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.ObjectModel;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DictionaryContainsKeyConstraintTests
    {
        [Test]
        public void SucceedsWhenKeyIsPresent()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("Hello"));
        }
        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKey()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola"));
        }

        [Test]
        public void SucceedsWhenKeyIsNotPresentUsingContainKey()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.Not.ContainKey("NotKey"));
        }

        [Test]
        public void FailsWhenKeyIsMissing()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyConstraint("Hallo"));

            Assert.That(act, Throws.Exception.TypeOf<AssertionException>());
        }

        [Test]
        public void FailsWhenNotUsedAgainstADictionary()
        {
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>(
                new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } });

            TestDelegate act = () => Assert.That(keyValuePairs, new DictionaryContainsKeyConstraint("Hallo"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("ContainsKey"));
        }

        [Test]
        public void WorksWithNonGenericDictionary()
        {
            var dictionary = new Hashtable { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("Hello"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("hello"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainsKeyWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("hola"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainsKeyWhenUsingLookupCustomComparer()
        {
            var list = new List<string> { "ALICE", "BOB", "CATHERINE" };
            ILookup<string, string> lookup = list.ToLookup(x => x, StringComparer.OrdinalIgnoreCase);

            Assert.That(lookup, Does.ContainKey("catherine"));
        }

        [Test]
        public void SucceedsWhenKeyIsNotPresentUsingContainsKeyUsingLookupDefaultComparer()
        {
            var list = new List<string> { "ALICE", "BOB", "CATHERINE" };
            ILookup<string, string> lookup = list.ToLookup(x => x);

            Assert.That(lookup, !Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainsKeyWhenUsingKeyedCollectionCustomComparer()
        {
            var list = new TestKeyedCollection(StringComparer.OrdinalIgnoreCase) { "ALICE", "BOB", "CALUM" };

            Assert.That(list, Does.ContainKey("calum"));
        }

        [Test]
        public void KeyIsNotPresentUsingContainsKeyUsingKeyedCollectionDefaultComparer()
        {
            var list = new TestKeyedCollection { "ALICE", "BOB", "CALUM" };

            Assert.That(list, !Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainsKeyUsingHashtableCustomComparer()
        {
            var table = new Hashtable(StringComparer.OrdinalIgnoreCase) { { "ALICE", "BOB" }, { "CALUM", "DENNIS" } };

            Assert.That(table, Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainsKeyUsingHashtableDefaultComparer()
        {
            var table = new Hashtable { { "ALICE", "BOB" }, { "CALUM", "DENNIS" } };

            Assert.That(table, !Does.ContainKey("calum"));
        }

        [Test]
        public void ShouldCallContainsKeysMethodWithTKeyParameterOnNewMethod()
        {
            var dictionary = new TestDictionaryGeneric<string, string> { { "ALICE", "BOB" }, { "CALUM", "DENNIS" } };

            Assert.That(dictionary, Does.ContainKey("BOB"));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnDictionary()
        {
            var dictionary = new TestDictionary(20);

            Assert.That(dictionary, Does.ContainKey(20));
            Assert.That(dictionary, !Does.ContainKey(10));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnPlainDictionary()
        {
            var dictionary = new TestNonGenericDictionary(99);

            Assert.That(dictionary, Does.ContainKey(99));
            Assert.That(dictionary, !Does.ContainKey(35));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnObject()
        {
            var poco = new TestPlainContainsKey("David");

            Assert.DoesNotThrow(() => Assert.That(poco, Does.ContainKey("David")));
        }

        [Test]
        public void ShouldThrowWhenUsedOnObjectWithNonGenericContains()
        {
            var poco = new TestPlainObjectContainsNonGeneric("Peter");

            Assert.Catch<ArgumentException>(() => Assert.That(poco, Does.ContainKey("Peter")));
        }

        [Test]
        public void ShouldCallContainsWhenUsedOnObjectWithGenericContains()
        {
            var poco = new TestPlainObjectContainsGeneric<string>("Peter");

            Assert.DoesNotThrow(() => Assert.That(poco, Does.ContainKey("Peter")));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnReadOnlyInterface()
        {
            var dictionary = new TestReadOnlyDictionary("BOB");

            Assert.That(dictionary, Does.ContainKey("BOB"));
            Assert.That(dictionary, !Does.ContainKey("ALICE"));
        }

        [Test]
        public void ShouldThrowWhenUsedWithISet()
        {
            var set = new TestSet();

            Assert.Catch<ArgumentException>(() => Assert.That(set, Does.ContainKey("NotHappening")));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnLookupInterface()
        {
            var dictionary = new TestLookup(20);

            Assert.That(dictionary, Does.ContainKey(20));
            Assert.That(dictionary, !Does.ContainKey(43));
        }

        #region Test Assets

        public class TestPlainContainsKey
        {
            private readonly string _key;

            public TestPlainContainsKey(string key)
            {
                _key = key;
            }

            public bool ContainsKey(string key)
            {
                return _key.Equals(key);
            }
        }

        public class TestPlainObjectContainsNonGeneric
        {
            private readonly string _key;

            public TestPlainObjectContainsNonGeneric(string key)
            {
                _key = key;
            }

            public bool Contains(string key)
            {
                return _key.Equals(key);
            }
        }

        public class TestPlainObjectContainsGeneric<TKey>
        {
            private readonly TKey _key;

            public TestPlainObjectContainsGeneric(TKey key)
            {
                _key = key;
            }

            public bool Contains(TKey key)
            {
                return _key.Equals(key);
            }
        }

        public class TestKeyedCollection : KeyedCollection<string, string>
        {
            public TestKeyedCollection() { }

            public TestKeyedCollection(IEqualityComparer<string> comparer) : base(comparer) { }

            protected override string GetKeyForItem(string item)
            {
                return item;
            }
        }

        public class TestDictionaryGeneric<TKey, TItem> : Dictionary<TKey, TItem>
        {
            public new bool ContainsKey(TKey key)
            {
                return base.Values.Any(x => x.Equals(key));
            }
        }

        public class TestDictionary : IDictionary<int, string>
        {
            private readonly int _key;

            public TestDictionary(int key)
            {
                _key = key;
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<int, string> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<int, string> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<int, string> item)
            {
                throw new NotImplementedException();
            }

            public int Count { get; }
            public bool IsReadOnly { get; }
            public bool ContainsKey(int key)
            {
                return key == _key;
            }

            public void Add(int key, string value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(int key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(int key, out string value)
            {
                throw new NotImplementedException();
            }

            public string this[int key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public ICollection<int> Keys { get; }
            public ICollection<string> Values { get; }
        }

        public class TestNonGenericDictionary : IDictionary
        {
            private readonly int _key;

            public TestNonGenericDictionary(int key)
            {
                _key = key;
            }

            public bool Contains(object key)
            {
                return _key == (int)key;
            }

            public void Add(object key, object value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                throw new NotImplementedException();
            }

            public object this[object key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public ICollection Keys { get; }
            public ICollection Values { get; }
            public bool IsReadOnly { get; }
            public bool IsFixedSize { get; }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public int Count { get; }
            public object SyncRoot { get; }
            public bool IsSynchronized { get; }
        }

        public class TestLookup : ILookup<int, string>
        {
            private readonly int _key;

            public TestLookup(int key)
            {
                _key = key;
            }

            public IEnumerator<IGrouping<int, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Contains(int key)
            {
                return key == _key;
            }

            public int Count { get; }

            public IEnumerable<string> this[int key]
            {
                get { throw new NotImplementedException(); }
            }
        }

        public class TestReadOnlyDictionary : IReadOnlyDictionary<string, string>
        {
            private readonly string _key;

            public TestReadOnlyDictionary(string key)
            {
                _key = key;
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count { get; }
            public bool ContainsKey(string key)
            {
                return _key == key;
            }

            public bool TryGetValue(string key, out string value)
            {
                throw new NotImplementedException();
            }

            public string this[string key]
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<string> Keys { get; }
            public IEnumerable<string> Values { get; }
        }

        public class TestSet : ISet<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<int>.Add(int item)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<int> other)
            {
                throw new NotImplementedException();
            }

            bool ISet<int>.Add(int item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(int item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(int item)
            {
                throw new NotImplementedException();
            }

            public int Count { get; }
            public bool IsReadOnly { get; }
        }
        #endregion
    }
}
