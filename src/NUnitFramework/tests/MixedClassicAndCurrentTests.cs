// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests
{
    public class MixedClassicAndCurrentTests
    {
        [Test]
        public void TestWithClassicAsserts()
        {
            Classic.Assert.AreEqual(1, 1);
            Assert.That(1, Is.EqualTo(1));
        }
    }
}
