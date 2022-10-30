// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Tests.TestAssembly
{
    [TestFixture]
    public class MockTestFixture
    {
        public const int Tests = 1;
        public const int Suites = 1;

        [Test]
        public void MyTest()
        {
        }
    }
}
