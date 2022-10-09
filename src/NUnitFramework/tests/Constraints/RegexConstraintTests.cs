// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class RegexConstraintTests
    {
        private static readonly string NL = Environment.NewLine;

        [Test]
        public void RegExMatchSucceeds()
        {
            const string testMatcher = "Make.*tests.*pass";
            const string testPhrase = "Make your tests fail before passing!";

            Assert.That(testPhrase, Does.Match(testMatcher));
            Assert.That(testPhrase, Does.Match(new Regex(testMatcher)));
        }

        [Test]
        public void RegExCaseInsensitiveMatchSucceeds()
        {
            const string testMatcher = "make.*tests.*PASS";
            const string testPhrase = "Make your tests fail before passing!";

            Assert.That(testPhrase, Does.Match(testMatcher).IgnoreCase);
            Assert.That(testPhrase, Does.Match(new Regex(testMatcher)).IgnoreCase);
            Assert.That(testPhrase, Does.Match(new Regex(testMatcher, RegexOptions.IgnoreCase)));
        }

        [Test]
        public void RegExNegativeMatchSucceeds()
        {
            const string testMatcher = "make.*tests.*fail";
            const string testPhrase = "Make your tests fail before passing!";

            Assert.That(testPhrase, Does.Not.Match(testMatcher));
            Assert.That(testPhrase, Does.Not.Match(new Regex(testMatcher)));
        }

        [Test]
        public void RegExConstraintExpressionMatchesSucceeds()
        {
            const string testMatcher = "Make.*tests.*pass";
            const string testPhrase = "Make your tests fail before passing!";

            Assert.That(testPhrase, Is.Not.Null.And.Matches(testMatcher));
            Assert.That(testPhrase, Is.Not.Null.And.Matches(new Regex(testMatcher)));
        }

        [Test]
        public void RegExCaseMatchFails()
        {
            var expectedErrorMessage = $"  Expected: String matching \"make.*tests.*fail\"{NL}  But was:  \"Make your tests fail before passing!\"{NL}";
            const string testMatcher = "make.*tests.*fail";
            const string testPhrase = "Make your tests fail before passing!";

            var ex = Assert.Throws<AssertionException>(() => Assert.That(testPhrase, Does.Match(testMatcher)));
            Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage));

            ex = Assert.Throws<AssertionException>(() => Assert.That(testPhrase, Does.Match(new Regex(testMatcher))));
            Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage));
        }
    }
}
