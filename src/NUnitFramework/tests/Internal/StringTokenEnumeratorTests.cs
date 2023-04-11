// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    public class StringTokenEnumeratorTests
    {
        [Test]
        public void Empty()
        {
            var tokens = Enumerate("", ',');
            Assert.AreEqual(0, tokens.Count);
        }

        [Test]
        public void Whitespace()
        {
            var tokens = Enumerate("  ", ',');
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual("  ", tokens[0]);
        }

        [Test]
        public void Tokens()
        {
            var tokens = Enumerate("a,b,c", ',');
            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual("a", tokens[0]);
            Assert.AreEqual("b", tokens[1]);
            Assert.AreEqual("c", tokens[2]);
        }

        [TestCase("aaa,,cccdd", ',', false)]
        [TestCase("somestring;another", ';', false)]
        [TestCase(",abc,def,,somelongerstring,", ',', false)]
        [TestCase("aaa,,cccdd", ',', true)]
        [TestCase("somestring;another", ';', true)]
        [TestCase(",abc,def,,somelongerstring,", ',', true)]
        public void CompareAgainstStringSplit(string input, char separator, bool returnEmptyTokens)
        {
            var tokens = Enumerate(input, separator, returnEmptyTokens);
            var tokens2 = input.Split(new[] { separator }, returnEmptyTokens ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
            Assert.That(tokens, Is.EqualTo(tokens2));
        }

        private static List<string> Enumerate(string input, char separator, bool returnEmptyTokens = false)
        {
            var result = new List<string>();
            foreach (var token in new StringTokenEnumerator(input, separator, returnEmptyTokens))
            {
                result.Add(token);
            }

            return result;
        }
    }
}
