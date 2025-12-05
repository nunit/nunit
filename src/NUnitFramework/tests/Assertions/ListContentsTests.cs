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

        [Test]
        public void ArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(TestArray, Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void EmptyArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(Array.Empty<object>(), Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

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
        public void ArrayListFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new SimpleObjectList(TestArray), Has.Some.EqualTo("def")));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
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

        [Test]
        public void ContainsNullability()
        {
            object nonNullObject = new object();
            object? nullObject = default(object);
            string nonNullString = "Hello World";
            string? nullString = default(string);
            object?[] collection = new object?[] { nonNullObject, nullObject, nonNullString, nullString };

            Assert.That(collection, Contains.Item(nonNullObject));
            Assert.That(collection, Contains.Item(nullObject));
            Assert.That(collection, Contains.Item(nonNullString));
            Assert.That(collection, Contains.Item(nullString));
            Assert.That(collection, Contains.Item(nonNullObject).And
                                            .Contains(nullObject).And
                                            .Contains(nonNullString).And
                                            .Contains(nullString));

            Assert.That(collection, Has.Member(nonNullObject));
            Assert.That(collection, Has.Member(nullObject));
            Assert.That(collection, Has.Member(nonNullString));
            Assert.That(collection, Has.Member(nullString));
            Assert.That(collection, Has.Member(nonNullObject).And
                                       .Member(nullObject).And
                                       .Member(nonNullString).And
                                       .Member(nullString));

            Assert.That(collection, Does.Contain(nonNullObject));
            Assert.That(collection, Does.Contain(nullObject));
            Assert.That(collection, Does.Contain(nonNullString));
            Assert.That(collection, Does.Contain(nullString));
            Assert.That(collection, Does.Contain(nonNullObject).And
                                        .Contain(nullObject).And
                                        .Contain(nonNullString).And
                                        .Contain(nullString));
        }

        [Test]
        public void NotContainsNullability()
        {
            object nonNullObject = new object();
            object? nullObject = default(object);
            string nonNullString = "Hello World";
            string? nullString = default(string);
            object[] collection = Array.Empty<object>();

            Assert.That(collection, Has.No.Member(nonNullObject));
            Assert.That(collection, Has.No.Member(nullObject));
            Assert.That(collection, Has.No.Member(nonNullString));
            Assert.That(collection, Has.No.Member(nullString));
            Assert.That(collection, Has.No.Member(nonNullObject).And
                                       .No.Member(nullObject).And
                                       .No.Member(nonNullString).And
                                       .No.Member(nullString));

            Assert.That(collection, Does.Not.Contain(nonNullObject));
            Assert.That(collection, Does.Not.Contain(nullObject));
            Assert.That(collection, Does.Not.Contain(nonNullString));
            Assert.That(collection, Does.Not.Contain(nullString));
            Assert.That(collection, Does.Not.Contain(nonNullObject).And
                                        .Not.Contain(nullObject).And
                                        .Not.Contain(nonNullString).And
                                        .Not.Contain(nullString));
        }

        [Test]
        public void ContainsUsingPropertiesComparer()
        {
            const string florence = "Florence";
            const string nathan = "Nathan";
            const string kassidy = "Kassidy";

            Person[] persons =
            [
                new() { Name = florence, Age = 42 },
                new() { Name = nathan, Age = 43 },
                new() { Name = kassidy, Age = 44 },
                new() { Name = kassidy, Age = 20 }
            ];

            Assert.That(persons, Does.Contain(new Person { Name = florence, Age = 42 })
                                     .UsingPropertiesComparer());
            Assert.That(persons, Does.Not.Contain(new Person { Name = florence, Age = 43 })
                                     .UsingPropertiesComparer());
            Assert.That(persons, Does.Contain(new Person { Name = florence })
                                     .UsingPropertiesComparer(c => c.Using(x => x.Name)));
            Assert.That(persons, Does.Contain(new Person { Name = florence })
                                     .UsingPropertiesComparer(c => c.Excluding(x => x.Age)));

            Assert.That(persons, Has.Member(new Person { Name = nathan, Age = 43 })
                                    .UsingPropertiesComparer());
            Assert.That(persons, Has.No.Member(new Person { Name = nathan, Age = 40 })
                                    .UsingPropertiesComparer());
            Assert.That(persons, Has.Member(new Person { Name = nathan })
                                    .UsingPropertiesComparer(c => c.Using(x => x.Name)));
            Assert.That(persons, Has.Member(new Person { Name = nathan })
                                    .UsingPropertiesComparer(c => c.Excluding(x => x.Age)));

            Assert.That(persons, Contains.Item(new Person { Name = kassidy, Age = 44 })
                                        .UsingPropertiesComparer());
            Assert.That(persons, Has.Exactly(2).EqualTo(new Person { Name = kassidy })
                                    .UsingPropertiesComparer(c => c.Using(x => x.Name)));
            Assert.That(persons, Has.Some.EqualTo(new Person { Name = kassidy })
                                    .UsingPropertiesComparer(c => c.Excluding(x => x.Age)));

            // Untyped.
            object wanted = new { Name = kassidy };
            Assert.That(persons, Does.Contain(wanted)
                                     .UsingPropertiesComparer<Person>(c => c.AllowDifferentTypes().Using(x => x.Name)));
        }

        private sealed class Person
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }
    }
}
