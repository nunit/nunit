// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#region Using Directives

#endregion

namespace NUnit.TestData
{
    [TestFixture(Author = "Rob Prouse"), Author("Charlie Poole", "charlie@poole.org")]
    [Author("NUnit")]
    public class AuthorFixture
    {
        [Test(Author = "Rob Prouse")]
        public void Method()
        { }

        [Test]
        public void NoAuthorMethod()
        { }

        [Test]
        [Author("Rob Prouse")]
        public void SeparateAuthorMethod()
        { }

        [Test]
        [Author("Rob Prouse", "rob@prouse.org")]
        public void SeparateAuthorWithEmailMethod()
        { }

        [Test, Author("Rob Prouse")]
        [TestCase(5, Author = "Charlie Poole")]
        public void TestCaseWithAuthor(int x)
        { }

        [Test, Author("Rob Prouse", "rob@prouse.org")]
        [Author("Charlie Poole", "charlie@poole.org"), Author("NUnit")]
        public void TestMethodMultipleAuthors()
        { }

    }
}
