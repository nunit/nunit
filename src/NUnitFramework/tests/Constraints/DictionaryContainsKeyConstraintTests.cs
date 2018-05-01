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

            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IDictionary"));
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

            Assert.That(dictionary,
                new DictionaryContainsKeyConstraint("HELLO").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
        }

        [Test, Explicit("Demonstrates issue #2837")]
        public void SucceedsWhenKeyIsPresentWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };

            Assert.That(dictionary, new DictionaryContainsKeyConstraint("hello"));
        }
        [Test, Explicit("Demonstrates issue #2837")]
        public void SucceedsWhenKeyIsPresentUsingContainKeyWhenDictionaryUsingCustomComparer()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hello", "World" }, { "Hola", "Mundo" } };
            Assert.That(dictionary, Does.ContainKey("hola"));
        }
    }
}
