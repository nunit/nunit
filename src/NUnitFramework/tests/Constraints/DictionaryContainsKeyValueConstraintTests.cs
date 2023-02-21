// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DictionaryContainsKeyValuePairConstraintTests
    {
        [Test]
        public void SucceedsWhenKeyValuePairIsPresent()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hi", "Universe"));
        }

        [Test]
        public void FailsWhenKeyIsMissing()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Bye", "Universe"));

            Assert.That(act, Throws.Exception.TypeOf<AssertionException>());
        }

        [Test]
        public void FailsWhenValueIsMissing()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hello", "Universe"));

            Assert.That(act, Throws.Exception.TypeOf<AssertionException>());
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValue()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("Mundo"));
        }

        [Test]
        public void SucceedsWhenPairIsNotPresentUsingContainKeyWithValue()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.Not.ContainKey("Hello").WithValue("NotValue"));
        }

        [Test]
        public void FailsWhenNotUsedAgainstADictionary()
        {
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>(
                new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } });

            TestDelegate act = () => Assert.That(keyValuePairs, new DictionaryContainsKeyValuePairConstraint("Hi", "Universe"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IDictionary"));
        }

        [Test]
        public void WorksWithNonGenericDictionary()
        {
            var dictionary = new Hashtable { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hi", "Universe"));
        }

        [Test, SetCulture("en-US")]
        public void IgnoreCaseIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("HI", "UNIVERSE").IgnoreCase);
        }

        [Test, SetCulture("en-US")]
        public void UsingIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary,
                new DictionaryContainsKeyValuePairConstraint("HI", "UNIVERSE").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
        }

        [Test]
        public void WorksWithTypeThatImplementsGenericIDictionary()
        {
            var dictionary = new TestDictionary()
            {
                { 1, "World" },
                { 2, "Universe" },
                { 3, "Mundo" }
            };
            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint(3, "Mundo"));
            Assert.That(dictionary, Does.ContainKey(2).WithValue("Universe"));
            Assert.That(dictionary, Does.Not.ContainKey(1).WithValue("Universe"));

        }

        [Test]
        public void FailsWhenNullDictionary()
        {
            TestDictionary dictionary = null;

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("1", "World"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("Expected: IDictionary But was: null"));
        }

        class TestDictionary : IDictionary<int, string>
        {
            private readonly Dictionary<int, string> _internalDictionary = new Dictionary<int, string>();

            public string this[int key]
            {
                get => _internalDictionary[key];
                set => _internalDictionary[key] = value;
            }

            public ICollection<int> Keys => throw new System.NotImplementedException();

            public ICollection<string> Values => throw new System.NotImplementedException();

            public int Count => throw new System.NotImplementedException();

            public bool IsReadOnly => throw new System.NotImplementedException();

            public void Add(int key, string value) => _internalDictionary.Add(key, value);

            public void Add(KeyValuePair<int, string> item)
            {
                throw new System.NotImplementedException();
            }

            public void Clear()
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(KeyValuePair<int, string> item)
            {
                throw new System.NotImplementedException();
            }

            public bool ContainsKey(int key)
            {
                throw new System.NotImplementedException();
            }

            public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(int key)
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(KeyValuePair<int, string> item)
            {
                throw new System.NotImplementedException();
            }

            public bool TryGetValue(int key, out string value)
            {
                throw new System.NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() => _internalDictionary.GetEnumerator();
        }
    }
}
