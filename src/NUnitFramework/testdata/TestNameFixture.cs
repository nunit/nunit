// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData
{
    public class TestNameFixture
    {
        [TestCase("ImplicitNull")]
        public void ImplicitNull(string name)
        {
        }

        [TestCase("ExplicitNull", TestName = null)]
        public void ExplicitNull(string name)
        {
        }

        [TestCase("Empty", TestName = "")]
        public void EmptyTest(string name)
        {
        }

        [TestCase("WhiteSpace", TestName = "    ")]
        public void WhiteSpaceTest(string name)
        {
        }

        [TestCase("ProperName", TestName = "ProperName")]
        public void ProperNameTest(string name)
        {
        }
    }
}
