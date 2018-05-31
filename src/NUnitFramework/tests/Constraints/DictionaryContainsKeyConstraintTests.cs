// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DictionaryContainsKeyConstraintTests
    {
        [Test]
        public void SucceedsWhenKeyIsPresent()
        {
            var dictionary = new Dictionary<string, string> {{"Hello", "World"}, {"Hola", "Mundo"}};

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
            var dictionary = new Dictionary<string, string> {{"Hello", "World"}, {"Hola", "Mundo"}};

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyConstraint("Hallo"));

            Assert.That(act, Throws.Exception.TypeOf<AssertionException>());
        }

        [Test]
        public void FailsWhenNotUsedAgainstADictionary()
        {
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>(
                new Dictionary<string, string> {{"Hello", "World"}, {"Hola", "Mundo"}});

            TestDelegate act = () => Assert.That(keyValuePairs, new DictionaryContainsKeyConstraint("Hallo"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("ContainsKey"));
        }

#if !NETCOREAPP1_1
        [Test]
        public void WorksWithNonGenericDictionary()
        {
            var dictionary = new Hashtable { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("Hello"));
        }
#endif

        [Test]
        public void IgnoreCaseIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("HELLO").IgnoreCase);
        }

        [Test]
        public void UsingIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("HELLO").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("hello"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKeyWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("hola"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKeyWhenUsingLookupCustomComparer()
        {
            var list = new List<string> { "ALICE", "BOB", "CATHERINE" };
            ILookup<string, string> lookup = list.ToLookup(x => x, StringComparer.OrdinalIgnoreCase);

            Assert.That(lookup, Does.ContainKey("catherine"));
        }

        [Test]
        public void SucceedsWhenKeyIsNotPresentUsingContainKeyUsingLookupDefaultComparer()
        {
            var list = new List<string> { "ALICE", "BOB", "CATHERINE" };
            ILookup<string, string> lookup = list.ToLookup(x => x);

            Assert.That(lookup, !Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKeyWhenUsingKeyedCollectionCustomComparer()
        {
            var list = new TestKeyedCollection(StringComparer.OrdinalIgnoreCase) { "ALICE", "BOB", "CALUM" };

            Assert.That(list, Does.ContainKey("calum"));
        }

        [Test]
        public void SucceedsWhenKeyIsNotPresentUsingContainKeyUsingKeyedCollectionDefaultComparer()
        {
            var list = new TestKeyedCollection {"ALICE", "BOB", "CALUM"};
            
            Assert.That(list, !Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKeyUsingHastableCustomComparer()
        {
            var table = new Hashtable(StringComparer.OrdinalIgnoreCase) { { "ALICE", "BOB" }, { "CALUM", "DENNIS" } };

            Assert.That(table, Does.ContainKey("alice"));
        }

        [Test]
        public void SucceedsWhenKeyIsPresentUsingContainKeyUsingHastableDefaultComparer()
        {
            var table = new Hashtable {{ "ALICE", "BOB"}, {"CALUM", "DENNIS"} };

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
            var dictionary = new TestPlainDictionary(99);

            Assert.Catch<ArgumentException>(() => Assert.That(dictionary, Does.ContainKey(99)));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnObject()
        {
            var poco = new TestPlainObjectContainsKey("David");

            Assert.Catch<ArgumentException>(() => Assert.That(poco, Does.ContainKey("David")));
        }

        [Test]
        public void ShouldCallContainsMethodOnObject()
        {
            var poco = new TestPlainObjectContains("Peter");

            Assert.Catch<ArgumentException>(() => Assert.That(poco, Does.ContainKey("Peter")));
        }

#if NET45

        [Test]
        public void ShouldCallContainsKeysMethodOnReadOnlyInterface()
        {
            var dictionary = new TestReadOnlyDictionary("BOB");

            Assert.That(dictionary, Does.ContainKey("BOB"));
            Assert.That(dictionary, !Does.ContainKey("ALICE"));
        }

        [Test]
        public void ShouldCallContainsKeysMethodOnSet()
        {
            var set = new TestSet();

            Assert.Catch<ArgumentException>(() => Assert.That(set, Does.ContainKey("NotHappening")));
        }

#endif

#if !NET20

        [Test]
        public void ShouldCallContainsKeysMethodOnLookupInterface()
        {
            var dictionary = new TestLookup(20);

            Assert.That(dictionary, Does.ContainKey(20));
            Assert.That(dictionary, !Does.ContainKey(43));
        }

#endif

        #region Test Assets

        public class TestPlainObjectContainsKey
        {
            private readonly string _key;

            public TestPlainObjectContainsKey(string key)
            {
                _key = key;
            }

            public bool ContainsKey(string key)
            {
                return _key.Equals(key);
            }
        }

        public class TestPlainObjectContains
        {
            private readonly string _key;

            public TestPlainObjectContains(string key)
            {
                _key = key;
            }

            public bool Contains(string key)
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

        public class TestPlainDictionary : IDictionary
        {
            private readonly int _key;

            public TestPlainDictionary(int key)
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

#if !NET20
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
#endif

#if NET45
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
#endif

        #endregion
    }
}
