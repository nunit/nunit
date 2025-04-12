// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class DictionaryContainsKeyValuePairConstraintTests
    {
        [TestCase("Universe")]
        [TestCase(null)]
        public void SucceedsWhenKeyValuePairIsPresent(string? expectedValue)
        {
            var dictionary = new Dictionary<string, string?> { { "Hello", "World" }, { "Hi", expectedValue }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hi", expectedValue));
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

        [TestCase("Mundo")]
        [TestCase(null)]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValue(string? expectedValue)
        {
            var dictionary = new Dictionary<string, string?> { { "Hello", "World" }, { "Hola", expectedValue } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue(expectedValue));
        }

        [Test]
        public void SucceedsWhenPairIsNotPresentUsingContainKeyWithValue()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.Not.ContainKey("Hello").WithValue("NotValue"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByAnd()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("Mundo").And.ContainKey("Hello").WithValue("World"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByOrBothTrue()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("Mundo").Or.ContainKey("Hello").WithValue("World"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByOrLeftKeyWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("NotKey").WithValue("Mundo").Or.ContainKey("Hello").WithValue("World"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByOrLeftValueWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("NotValue").Or.ContainKey("Hello").WithValue("World"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByOrRightKeyWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("Mundo").Or.ContainKey("NotKey").WithValue("World"));
        }

        [Test]
        public void SucceedsWhenPairIsPresentUsingContainKeyWithValueJoinedByOrRightValueWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("Hola").WithValue("Mundo").Or.ContainKey("Hello").WithValue("NotValue"));
        }

        [Test]
        public void FailsWhenNotUsedAgainstADictionary()
        {
            var keyValuePairs = new List<string> { "Hello", "Hi", "Hola" };

            TestDelegate act = () => Assert.That(keyValuePairs, new DictionaryContainsKeyValuePairConstraint("Hi", "Universe"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IDictionary or IEnumerable<KeyValuePair<,>>"));
        }

        [Test]
        public void FailsWhenPairIsPresentUsingContainKeyWithValueJoinedByAndBothFalse()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            var expression = (IResolveConstraint)Does.ContainKey("NotKeyLeft").WithValue("NotValueLeft").And.ContainKey("NotKeyRight").WithValue("NotValueRight");
            var constraint = expression.Resolve();
            var result = constraint.ApplyTo(dictionary);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void FailsWhenPairIsPresentUsingContainKeyWithValueJoinedByAndLeftKeyWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            var expression = (IResolveConstraint)Does.ContainKey("NotKey").WithValue("Mundo").And.ContainKey("Hello").WithValue("World");
            var constraint = expression.Resolve();
            var result = constraint.ApplyTo(dictionary);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void FailsWhenPairIsPresentUsingContainKeyWithValueJoinedByAndLeftValueWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            var expression = (IResolveConstraint)Does.ContainKey("Hola").WithValue("NotValue").And.ContainKey("Hello").WithValue("World");
            var constraint = expression.Resolve();
            var result = constraint.ApplyTo(dictionary);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void FailsWhenPairIsPresentUsingContainKeyWithValueJoinedByAndRightKeyWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            var expression = (IResolveConstraint)Does.ContainKey("Hola").WithValue("Mundo").And.ContainKey("NotKey").WithValue("World");
            var constraint = expression.Resolve();
            var result = constraint.ApplyTo(dictionary);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void FailsWhenPairIsPresentUsingContainKeyWithValueJoinedByAndRightValueWrong()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };

            var expression = (IResolveConstraint)Does.ContainKey("Hola").WithValue("Mundo").And.ContainKey("Hello").WithValue("NotValue");
            var constraint = expression.Resolve();
            var result = constraint.ApplyTo(dictionary);

            Assert.That(result.IsSuccess, Is.False);
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

        [Test]
        public void IgnoreWhiteSpaceIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi ", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hi", " U n i v e r s e").IgnoreWhiteSpace);
        }

        [Test]
        public void NormalizeLineEndingsIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi\r\n", "Universe\r" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("Hi\r", "Universe\n").NormalizeLineEndings);
        }

        [Test, SetCulture("en-US")]
        public void UsingIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary,
                new DictionaryContainsKeyValuePairConstraint("HI", "UNIVERSE").Using<string>((x, y) => string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase)));
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
            TestDictionary? dictionary = null;

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsKeyValuePairConstraint("1", "World"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("Expected: IDictionary or IEnumerable<KeyValuePair<,>> But was: null"));
        }

        [Test]
        public void WorksWithTypeThatImplementsIEnumerableOfKeyValuePairs()
        {
            var dictionary = new Dictionary<int, string>()
            {
                { 1, "World" },
                { 2, "Universe" },
                { 3, "Mundo" }
            };

            var enumeration = new KVPEnumeration(dictionary);

            Assert.That(enumeration, new DictionaryContainsKeyValuePairConstraint(3, "Mundo"));
            Assert.That(enumeration, Does.ContainKey(2).WithValue("Universe"));
            Assert.That(enumeration, Does.Not.ContainKey(1).WithValue("Universe"));
        }

        private class TestDictionary : IDictionary<int, string>
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

        private sealed class KVPEnumeration : IEnumerable<KeyValuePair<int, string>>
        {
            private readonly Dictionary<int, string> _values;

            public KVPEnumeration(Dictionary<int, string> values)
            {
                _values = values;
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => _values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
