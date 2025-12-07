// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Attributes
{
    /// <summary>
    /// Verify that Issue3125 https://github.com/nunit/nunit/issues/3125 is resolved.
    /// Before 4.5 this will fail/crash due to arguments not being converted correctly.
    /// </summary>
    public class TestCaseSourceIssue3125Tests
    {
        private static readonly TestCaseData[] Cases =
        [
            new(string.Empty, 0),
            new(string.Empty, 1)
        ];

        [TestCaseSource(nameof(Cases))]
        public void TestA(string a, float b)
        {
            Assert.Pass();
        }

        [TestCaseSource(nameof(Cases))]
        public void TestB(string a, int b)
        {
            Assert.Pass();
        }
    }
}
