// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.Collections;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class AllItemsConstraintTests
    {
        private static readonly string NL = Environment.NewLine;

        [Test]
        public void AllItemsAreNotNull()
        {
            object[] c = new object[] { 1, "hello", 3, Environment.NewLine };
            Assert.That(c, new AllItemsConstraint(Is.Not.Null));
        }

        [Test]
        public void AllItemsAreNotNullFails()
        {
            object?[] c = new object?[] { 1, "hello", null, 3 };
            var expectedMessage =
                "  Assert.That(c, new AllItemsConstraint(new NotConstraint(new EqualConstraint(null))))" + Environment.NewLine +
                TextMessageWriter.Pfx_Expected + "all items not equal to null" + NL +
                TextMessageWriter.Pfx_Actual + "< 1, \"hello\", null, 3 >" + NL +
                "  First non-matching item at index [2]:  null" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new NotConstraint(new EqualConstraint(null)))));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
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
                "  Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100)))" + Environment.NewLine +
                TextMessageWriter.Pfx_Expected + "all items in range (10,100)" + NL +
                TextMessageWriter.Pfx_Actual + "< 12, 27, 19, 32, 107, 99, 26 >" + NL +
                "  First non-matching item at index [4]:  107" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new RangeConstraint(10, 100))));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
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
                "  Assert.That(c, new AllItemsConstraint(new InstanceOfTypeConstraint(typeof(char))))" + Environment.NewLine +
                TextMessageWriter.Pfx_Expected + "all items instance of <System.Char>" + NL +
                TextMessageWriter.Pfx_Actual + "< 'a', \"b\", 'c' >" + NL +
                "  First non-matching item at index [1]:  \"b\"" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new AllItemsConstraint(new InstanceOfTypeConstraint(typeof(char)))));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void WorksOnICollection()
        {
            var c = new SimpleObjectCollection(1, 2, 3);
            Assert.That(c, Is.All.Not.Null);
        }

        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new AllItemsConstraint(new RangeConstraint(10, 100)));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }
    }
}
