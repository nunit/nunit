// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Tests.Singletons
{
    [TestFixture]
    public class OneTestCase
    {
        public const int Tests = 1;
        public const int Suites = 1;

        [Test]
        public virtual void TestCase() { }
    }
}
