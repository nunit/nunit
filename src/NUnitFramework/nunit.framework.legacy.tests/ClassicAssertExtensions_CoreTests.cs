// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_CoreTests
    {
        [Test]
        public void Equality_and_Sameness()
        {
            var o = new object();
            var same = o;

            Assert.AreEqual(42, 42);
            Assert.AreNotEqual(1, 2);

            Assert.AreSame(o, same);
            Assert.AreNotSame(o, new object());
        }

        [Test]
        public void Truthiness_and_Nullability()
        {
            Assert.IsTrue(true);
            Assert.IsFalse(false);

            object? n = null;
            object x = new();
            Assert.IsNull(n);
            Assert.IsNotNull(x);
        }
    }
}
