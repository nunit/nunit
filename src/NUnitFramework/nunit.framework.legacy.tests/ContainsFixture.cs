// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class ContainsFixture
    {
        [Test]
        public void Contains()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            ClassicAssert.Contains(3, list);
            ClassicAssert.Contains(3, list, "int");
            ClassicAssert.Contains(3, list, "{0}", "int");
        }

        [Test]
        public void ContainsFails()
        {
            var list = new List<int> { 1, 2, 4, 5 };
            Assert.That(() => ClassicAssert.Contains(3, list, "Contains {0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("Contains int"));
        }
    }
}
