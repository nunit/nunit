// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Summary description for ListContentsTests.
    /// </summary>
    [TestFixture]
    public class ListContentsTests
    {
        private static readonly object[] testArray = { "abc", 123, "xyz" };

        [Test]
        public void ArraySucceeds()
        {
            Assert.Contains("abc", testArray);
            Assert.Contains(123, testArray);
            Assert.Contains("xyz", testArray, "expected array containing '{0}'", "xyz");
        }

        [Test]
        public void ArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains("def", testArray));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EmptyArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains( "def", Array.Empty<object>() ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NullArrayIsError()
        {
            Assert.Throws<ArgumentException>(() => Assert.Contains( "def", null ));
        }

        [Test]
        public void ArrayListSucceeds()
        {
            var list = new SimpleObjectList( testArray );

            Assert.Contains( "abc", list );
            Assert.Contains( 123, list );
            Assert.Contains( "xyz", list );
        }

        [Test]
        public void ArrayListFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains( "def", new SimpleObjectList( testArray ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DifferentTypesMayBeEqual()
        {
            Assert.Contains( 123.0, new SimpleObjectList( testArray ) );
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
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Does.Contain(7).Using(myIntComparer);
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
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Contains.Item(7).Using(myIntComparer);
            Assert.That(collection1, constraint);
        }
    }
}
