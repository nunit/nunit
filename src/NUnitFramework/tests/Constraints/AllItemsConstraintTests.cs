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
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AllItemsConstraintTests
    {
        private readonly string NL = NUnit.Env.NewLine;

        [Test]
        public void AllItemsAreNotNull()
        {
            object[] c = new object[] { 1, "hello", 3, Environment.NewLine };
            Assert.That(c, new AllItemsConstraint(Is.Not.Null));
        }

        [Test]
        public void AllItemsAreNotNullFails()
        {
            object[] c = new object[] { 1, "hello", null, 3 };
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "all items not equal to null" + NL +
                TextMessageWriter.Pfx_Actual + "< 1, \"hello\", null, 3 >" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new NotConstraint(new EqualConstraint(null)))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AllItemsAreInRange()
        {
            int[] c = new int[] { 12, 27, 19, 32, 45, 99, 26 };
            Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100)));
        }

        [Test]
        public void AllItemsAreInRange_UsingIComparer()
        {
            var comparer = new ObjectComparer();
            int[] c = new int[] { 12, 27, 19, 32, 45, 99, 26 };
            Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100).Using(comparer)));
            Assert.That(comparer.WasCalled);
        }

        [Test]
        public void AllItemsAreInRange_UsingGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            int[] c = new int[] { 12, 27, 19, 32, 45, 99, 26 };
            Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100).Using(comparer)));
            Assert.That(comparer.WasCalled);
        }

        [Test]
        public void AllItemsAreInRange_UsingGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            int[] c = new int[] { 12, 27, 19, 32, 45, 99, 26 };
            Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100).Using(comparer.Delegate)));
            Assert.That(comparer.WasCalled);
        }

        [Test]
        public void AllItemsAreInRangeFailureMessage()
        {
            int[] c = new int[] { 12, 27, 19, 32, 107, 99, 26 };
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "all items in range (10,100)" + NL +
                TextMessageWriter.Pfx_Actual + "< 12, 27, 19, 32, 107, 99, 26 >" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AllItemsAreInstancesOfType()
        {
            object[] c = new object[] { 'a', 'b', 'c' };
            Assert.That(c, new AllItemsConstraint(new InstanceOfTypeConstraint(typeof(char))));
        }

        [Test]
        public void AllItemsAreInstancesOfTypeFailureMessage()
        {
            object[] c = new object[] { 'a', "b", 'c' };
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "all items instance of <System.Char>" + NL +
                TextMessageWriter.Pfx_Actual + "< 'a', \"b\", 'c' >" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new InstanceOfTypeConstraint(typeof(char)))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        public void WorksOnICollection()
        {
            var c = new NUnit.TestUtilities.Collections.SimpleObjectCollection(1, 2, 3);
            Assert.That(c, Is.All.Not.Null);
        }
    }
}