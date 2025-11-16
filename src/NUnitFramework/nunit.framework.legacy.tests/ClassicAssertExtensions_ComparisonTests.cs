// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_ComparisonTests
    {
        [Test]
        public void Greater_Less_Comparisons()
        {
            Assert.Greater(2, 1);
            Assert.GreaterOrEqual(2, 2);
            Assert.Less(1, 2);
            Assert.LessOrEqual(2, 2);
        }

        private sealed class C : System.IComparable
        {
            private readonly int _v;
            public C(int v)
            {
                _v = v;
            }
            public int CompareTo(object? other)
            {
                if (other is C c)
                {
                    return _v.CompareTo(c._v);
                }

                return _v.CompareTo(0);
            }
        }

        [Test]
        public void Comparable_Comparisons()
        {
            var a = new C(1);
            var b = new C(2);

            Assert.Less((System.IComparable)a, (System.IComparable)b);
            Assert.Greater((System.IComparable)b, (System.IComparable)a);
        }
    }
}
