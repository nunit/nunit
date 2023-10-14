// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture(Description = "Fixture Description")]
    public class DescriptionFixture
    {
        [Test(Description = "Test Description")]
        public void Method()
        {
        }

        [Test]
        public void NoDescriptionMethod()
        {
        }

        [Test]
        [Description("Separate Description")]
        public void SeparateDescriptionMethod()
        {
        }

        [Test, Description("method description")]
        [TestCase(5, Description = "case description")]
        public void TestCaseWithDescription(int x)
        {
        }

        [Test, Description("This is a really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really long description")]
        public void TestWithLongDescription()
        {
        }
    }
}
