// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData
{
    public class OptionalTestParametersFixture
    {
        [TestCase()]
        public void MethodWithOptionalParams0(int x, int y = 1)
        {
        }

        [TestCase(10)]
        public void MethodWithOptionalParams1(int x, int y = 1)
        {
        }

        [TestCase(10, 20)]
        public void MethodWithOptionalParams2(int x, int y = 1)
        {
        }

        [TestCase(10, 20, 30)]
        public void MethodWithOptionalParams3(int x, int y = 1)
        {
        }

        [TestCase]
        public void MethodWithAllOptionalParams0(int x = 0, int y = 1)
        {
        }

        [TestCase(10)]
        public void MethodWithAllOptionalParams1(int x = 0, int y = 1)
        {
        }

        [TestCase(10, 20)]
        public void MethodWithAllOptionalParams2(int x = 0, int y = 1)
        {
        }

        [TestCase(10, 20, 30)]
        public void MethodWithAllOptionalParams3(int x = 0, int y = 1)
        {
        }
    }
}
