// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.TestUtilities.Collections;

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// Summary description for ListContentsTests.
    /// </summary>
    [TestFixture]
    public class ListContentsTests
    {
        private static readonly object[] TestArray = { "abc", 123, "xyz" };

        [Test]
        public void ArraySucceeds()
        {
            Assert.That(TestArray, Has.Some.EqualTo("abc"));
            Assert.That(TestArray, Has.Some.EqualTo(123));
            Assert.That(TestArray, Has.Some.EqualTo("xyz"), "expected array containing 'xyz'");
        }
#if NET5_0_OR_GREATER
        [Test]
        public void ArrayFails()
        {
            var expectedMessage =
                "  Assert.That(TestArray, Has.Some.EqualTo(\"def\"))" + Environment.NewLine +
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(TestArray, Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EmptyArrayFails()
        {
            var expectedMessage =
                "  Assert.That(Array.Empty<object>(), Has.Some.EqualTo(\"def\"))" + Environment.NewLine +
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(Array.Empty<object>(), Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ArrayListFails()
        {
            var expectedMessage =
                "  Assert.That(new SimpleObjectList(TestArray), Has.Some.EqualTo(\"def\"))" + Environment.NewLine +
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new SimpleObjectList(TestArray), Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }
#endif
        [Test]
        public void NullArrayIsError()
        {
            string[]? nullList = null;
            Assert.Throws<ArgumentException>(() => Assert.That(nullList, Has.Some.EqualTo("def")));
        }

        [Test]
        public void ArrayListSucceeds()
        {
            var list = new SimpleObjectList(TestArray);

            Assert.That(list, Has.Some.EqualTo("abc"));
            Assert.That(list, Has.Some.EqualTo(123));
            Assert.That(list, Has.Some.EqualTo("xyz"));
        }

        [Test]
        public void DifferentTypesMayBeEqual()
        {
            Assert.That(new SimpleObjectList(TestArray), Has.Some.EqualTo(123.0));
        }

        [Test]
        public void DoesContainMultipleItemsString()
        {
            var collection = new[] { "test1", "test2", "test3" };
            Assert.That(collection, Does.Contain("test1").And.Contains("test2").And.Contains("test3"));
        }

        [Test]
        public void ContainsMultipleItemsString()
        {
            var collection = new[] { "test1", "test2", "test3" };
            Assert.That(collection, Contains.Item("test1").And.Contains("test2").And.Contains("test3"));
        }

        [Test]
        public void DoesContainMultipleItemsInt()
        {
            var collection = new[] { 1, 2, 3 };
            Assert.That(collection, Does.Contain(1).And.Contains(2).And.Contains(3));
        }

        [Test]
        public void DoesContainAnd()
        {
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Does.Contain(7).And.Member(2);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void DoesContainUsing()
        {
            bool MyIntComparer(int x, int y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Does.Contain(7).Using((Func<int, int, bool>)MyIntComparer);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ContainsMultipleItemsInt()
        {
            var collection = new[] { 1, 2, 3 };
            Assert.That(collection, Contains.Item(1).And.Contains(2).And.Contains(3));
        }

        [Test]
        public void ContainsItemAnd()
        {
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Contains.Item(7).And.Member(3);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ContainsItemUsing()
        {
            bool MyIntComparer(int x, int y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Contains.Item(7).Using((Func<int, int, bool>)MyIntComparer);
            Assert.That(collection1, constraint);
        }
    }
}
