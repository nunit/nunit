// ***********************************************************************
// Copyright (c) 2020 Charlie Poole
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

using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class RegexConstraintTests
    {
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
            const string expectedErrorMessage = "  Expected: String matching pattern \"make.*tests.*fail\"\n  But was:  \"Make your tests fail before passing!\"\n";
            const string testMatcher = "make.*tests.*fail";
            const string testPhrase = "Make your tests fail before passing!";

            Assert.That(
                () => Assert.That(testPhrase, Does.Match(testMatcher)),
                Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(expectedErrorMessage));

            Assert.That(
                () => Assert.That(testPhrase, Does.Match(new Regex(testMatcher))),
                Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(expectedErrorMessage));
        }
    }
}
