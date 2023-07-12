// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public class StringTokenEnumeratorTests
    {
        [Test]
        public void Empty()
        {
            var tokens = Enumerate("", ',');
            Assert.That(tokens, Is.Empty);
        }

        [Test]
        public void Whitespace()
        {
            var tokens = Enumerate("  ", ',');
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(tokens[0], Is.EqualTo("  "));
        }

        [Test]
        public void Tokens()
        {
            var tokens = Enumerate("a,b,c", ',');
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(tokens[0], Is.EqualTo("a"));
            Assert.That(tokens[1], Is.EqualTo("b"));
            Assert.That(tokens[2], Is.EqualTo("c"));
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
