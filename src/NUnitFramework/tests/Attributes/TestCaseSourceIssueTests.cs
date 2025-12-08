// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

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
            new("0", 0),
            new("1", 1)
        ];

        [TestCaseSource(nameof(Cases))]
        public void TestA(string a, float b)
        {
            Assert.That(float.Parse(a), Is.EqualTo(b));
        }

        [TestCaseSource(nameof(Cases))]
        public void TestB(string a, int b)
        {
            Assert.That(int.Parse(a), Is.EqualTo(b));
        }

        [TestCaseSource(nameof(Cases))]
        public void TestC(string a, int b, CancellationToken token)
        {
            Assert.That(int.Parse(a), Is.EqualTo(b));
            Assert.That(token, Is.EqualTo(CancellationToken.None));
        }

        [TestCaseSource(nameof(Cases))]
        public void TestD(string a, double b, int c = 4)
        {
            Assert.That(double.Parse(a), Is.EqualTo(b));
            Assert.That(c, Is.EqualTo(4));
        }

#pragma warning disable NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method
        [TestCaseSource(nameof(Cases))]
#pragma warning restore NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method
        public void TestE(params object[] a)
        {
            Assert.That(a.Length, Is.EqualTo(2));
            Assert.That(a[0], Is.InstanceOf<string>());
            Assert.That(int.Parse((string)a[0]), Is.EqualTo(a[1]));
        }
    }
}
