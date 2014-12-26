// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    #region ComparisonConstraintTestBase

    public abstract class ComparisonConstraintTestBase : ConstraintTestBase
    {
        protected ComparisonConstraint comparisonConstraint;

        [TestCase(null)]
        [TestCase("xxx")]
        public void InvalidDataThrowsArgumentException(object data)
        {
            Assert.Throws<ArgumentException>(() => theConstraint.ApplyTo(data));
        }

        [Test]
        public void UsesProvidedIComparer()
        {
            var comparer = new ObjectComparer();
            comparisonConstraint.Using(comparer).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            comparisonConstraint.Using(comparer).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            comparisonConstraint.Using(comparer.Delegate).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            comparisonConstraint.Using(comparer).ApplyTo(0);
        }
    }

    #endregion

    #region Comparison Test Classes

    class ClassWithIComparable : IComparable
    {
        private int val;

        public ClassWithIComparable(int val)
        {
            this.val = val;
        }

        public int CompareTo(object x)
        {
            ClassWithIComparable other = x as ClassWithIComparable;
            if (x is ClassWithIComparable)
                return val.CompareTo(other.val);

            throw new ArgumentException();
        }
    }

    class ClassWithIComparableOfT : IComparable<ClassWithIComparableOfT>
    {
        private int val;

        public ClassWithIComparableOfT(int val)
        {
            this.val = val;
        }

        public int CompareTo(ClassWithIComparableOfT other)
        {
            return val.CompareTo(other.val);
        }
    }

    #endregion
}