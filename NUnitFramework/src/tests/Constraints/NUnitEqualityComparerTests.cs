// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class EqualityComparerTests
    {
        private Tolerance tolerance;
        private NUnitEqualityComparer comparer;

        [SetUp]
        public void Setup()
        {
            tolerance = Tolerance.Empty;
            comparer = new NUnitEqualityComparer();
        }

        [Test]
        public void CanCompareArrayContainingSelfToSelf()
        {
            object[] array = new object[1];
            array[0] = array;

            Assert.True(comparer.AreEqual(array, array, ref tolerance));
        }

        [Test]
        public void IEquatableSuccess()
        {
            IEquatableWithoutEqualsOverridden x = new IEquatableWithoutEqualsOverridden(1);
            IEquatableWithoutEqualsOverridden y = new IEquatableWithoutEqualsOverridden(1);

            Assert.IsTrue(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenActualImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.IsTrue(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenExpectedImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.IsTrue(comparer.AreEqual(y, x, ref tolerance));
        }

        [Test]
        public void ReferenceEqualityHasPrecedenceOverIEquatable()
        {
            NeverEqualIEquatable z = new NeverEqualIEquatable();

            Assert.IsTrue(comparer.AreEqual(z, z, ref tolerance));
        }

        [Test]
        public void IEquatableHasPrecedenceOverDefaultEquals()
        {
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals x = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals y = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();

            Assert.IsFalse(comparer.AreEqual(x, y, ref tolerance));
        }
    }

    public class NeverEqualIEquatableWithOverriddenAlwaysTrueEquals : IEquatable<NeverEqualIEquatableWithOverriddenAlwaysTrueEquals>
    {
        public bool Equals(NeverEqualIEquatableWithOverriddenAlwaysTrueEquals other)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Int32IEquatable : IEquatable<int>
    {
        private readonly int value;

        public Int32IEquatable(int value)
        {
            this.value = value;
        }

        public bool Equals(int other)
        {
            return value.Equals(other);
        }
    }

    public class NeverEqualIEquatable : IEquatable<NeverEqualIEquatable>
    {
        public bool Equals(NeverEqualIEquatable other)
        {
            return false;
        }
    }

    public class IEquatableWithoutEqualsOverridden : IEquatable<IEquatableWithoutEqualsOverridden>
    {
        private readonly int value;

        public IEquatableWithoutEqualsOverridden(int value)
        {
            this.value = value;
        }

        public bool Equals(IEquatableWithoutEqualsOverridden other)
        {
            return value.Equals(other.value);
        }
    }
}
