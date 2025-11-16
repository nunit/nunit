// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_StringTests
    {
        [Test]
        public void Starts_Ends_EqualsIgnoringCase()
        {
            Assert.StartsWith("ab", "abc");
            Assert.EndsWith("bc", "abc");
            Assert.AreEqualIgnoringCase("ABC", "abc");
        }

        [Test]
        public void RegexMatching_and_Contains()
        {
            Assert.IsMatch("^a.c$", "abc");
            Assert.DoesNotMatch("^z", "abc");

            Assert.StringContains("ell", "hello");
            Assert.DoesNotContain("xyz", "hello");
        }
    }
}
