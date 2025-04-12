// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class DictionaryContainsValueConstraintTests
    {
        [Test]
        public void SucceedsWhenValueIsPresent()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("Universe"));
        }

        [Test]
        public void FailsWhenValueIsMissing()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            TestDelegate act = () => Assert.That(dictionary, new DictionaryContainsValueConstraint("Community"));

            Assert.That(act, Throws.Exception.TypeOf<AssertionException>());
        }

        [TestCase("Mundo")]
        [TestCase(null)]
        public void SucceedsWhenValueIsPresentUsingContainValue(string? expectedValue)
        {
            var dictionary = new Dictionary<string, string?> { { "Hello", "World" }, { "Hola", expectedValue } };
            Assert.That(dictionary, Does.ContainValue(expectedValue));
        }

        [Test]
        public void SucceedsWhenValueIsNotPresentUsingContainValue()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.Not.ContainValue("NotValue"));
        }

        [Test]
        public void FailsWhenNotUsedAgainstADictionary()
        {
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>(
                new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } });

            TestDelegate act = () => Assert.That(keyValuePairs, new DictionaryContainsValueConstraint("Community"));

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IDictionary"));
        }

        [Test]
        public void WorksWithNonGenericDictionary()
        {
            var dictionary = new Hashtable { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("Universe"));
        }

        [Test, SetCulture("en-US")]
        public void IgnoreCaseIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("UNIVERSE").IgnoreCase);
        }

        [Test]
        public void IgnoreWhiteSpaceIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("U n i v e r s e").IgnoreWhiteSpace);
        }

        [Test]
        public void NormalizeLineEndingsIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe\r" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("Universe\r\n").NormalizeLineEndings);
        }

        [Test, SetCulture("en-US")]
        public void UsingIsHonored()
        {
            var dictionary = new Dictionary<string, string> { { "Hello", "World" }, { "Hi", "Universe" }, { "Hola", "Mundo" } };

            Assert.That(dictionary,
                new DictionaryContainsValueConstraint("UNIVERSE").Using<string>((x, y) => string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase)));
        }

        [Test]
        public void UsingPropertiesComparerIsHonored()
        {
            var dictionary = new Dictionary<string, XY> { { "5", new(3, 4) }, { "13", new(5, 12) } };
            var value = new XY(5, 12);
            Assert.That(dictionary, Does.Not.ContainValue(value));
            Assert.That(dictionary, Does.ContainValue(value).UsingPropertiesComparer());
        }

        [Test]
        public void UsingCustomComparerIsHonored()
        {
            var dictionary = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

            Assert.That(dictionary, new DictionaryContainsValueConstraint("1").Using<int, string>((actual, expected) => actual.ToString() == expected));
        }

        private sealed class XY
        {
            public XY(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }
        }
    }
}
