// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Specialized;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class EmptyConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new EmptyConstraint();
            ExpectedDescription = "<empty>";
            StringRepresentation = "<empty>";
        }

        static object[] SuccessData = new object[]
        {
            string.Empty,
            Array.Empty<object>(),
            new ArrayList(),
            new System.Collections.Generic.List<int>(),
            Guid.Empty,
            new SingleElementCollection<int>(),
            new NameValueCollection(),
            System.Collections.Immutable.ImmutableArray<int>.Empty,
            new EnumerableWithIndependentCount(1, Array.Empty<int>())
        };

        private static object[] FailureData = new object[]
        {
            new TestCaseData("Hello", "\"Hello\""),
            new TestCaseData(new object[] { 1, 2, 3 }, "< 1, 2, 3 >"),
            new TestCaseData(new Guid("12345678-1234-1234-1234-123456789012"), "12345678-1234-1234-1234-123456789012"),
            new TestCaseData(new SingleElementCollection<int>(1), "<1>"),
            new TestCaseData(new NameValueCollection { ["Hello"] = "World" }, "< \"Hello\" >"),
            new TestCaseData(System.Collections.Immutable.ImmutableArray.Create(1), "< 1 >"),
            new TestCaseData(new EnumerableWithIndependentCount(0, new[] { 1 }), "< 1 >"),
        };

        [TestCase(null)]
        [TestCase(5)]
        public void InvalidDataThrowsArgumentException(object data)
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(data));
        }

        public void InvalidDataThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(default(SingleElementCollection<int>)));
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(new NotReallyACollection()));
        }

        [Test]
        public void NullStringGivesFailureResult()
        {
            string actual = null;
            var result = TheConstraint.ApplyTo(actual);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
        }

        [Test]
        public void NullNullableGuidGivesFailureResult()
        {
            Guid? actual = null;
            var result = TheConstraint.ApplyTo(actual);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
        }

        [Test]
        public void NullArgumentExceptionMessageContainsTypeName()
        {
            int? testInput = null;
            Assert.That(() => TheConstraint.ApplyTo(testInput),
               Throws.ArgumentException.With.Message.Contains("System.Int32"));
        }

        private class SingleElementCollection<T>
        {
            private readonly T _element;

            public int Count { get; private set; }

            public SingleElementCollection()
            {
            }

            public SingleElementCollection(T element)
            {
                _element = element;
                Count = 1;
            }

            public T Get()
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Collection is empty");
                }

                Count = 0;
                return _element;
            }

            public override string ToString()
            {
                return Count == 0 ? "<empty>" : _element.ToString();
            }
        }

        private class NotReallyACollection
        {
#pragma warning disable CA1822 // Mark members as static
            public double Count => 0;
#pragma warning restore CA1822 // Mark members as static
        }

        private class EnumerableWithIndependentCount : IEnumerable<int>
        {
            private readonly IReadOnlyCollection<int> _items;
            public int Count { get; }

            public EnumerableWithIndependentCount(int count, IReadOnlyCollection<int> items)
            {
                _items = items;
                Count = count;
            }

            public IEnumerator<int> GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    [TestFixture]
    public class EmptyStringConstraintTest : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new EmptyStringConstraint();
            ExpectedDescription = "<empty>";
            StringRepresentation = "<emptystring>";
        }

        static object[] SuccessData = new object[]
        {
            string.Empty
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( "Hello", "\"Hello\"" ),
            new TestCaseData( null, "null")
        };
    }

    [TestFixture]
    public class EmptyDirectoryConstraintTest
    {
        [Test]
        public void EmptyDirectory()
        {
            using (var testDir = new TestDirectory())
            {
                Assert.That(testDir.Directory, Is.Empty);
            }
        }

        [Test]
        public void NotEmptyDirectory_ContainsFile()
        {
            using (var testDir = new TestDirectory())
            {
                File.Create(Path.Combine(testDir.Directory.FullName, "DUMMY.FILE")).Dispose();

                Assert.That(testDir.Directory, Is.Not.Empty);
            }
        }

        [Test]
        public void NotEmptyDirectory_ContainsDirectory()
        {
            using (var testDir = new TestDirectory())
            {
                Directory.CreateDirectory(Path.Combine(testDir.Directory.FullName, "DUMMY_DIR"));

                Assert.That(testDir.Directory, Is.Not.Empty);
            }
        }
    }

    [TestFixture]
    public class EmptyGuidConstraintTest
    {
        [Test]
        public void EmptyGuid()
        {
            Assert.That(Guid.Empty, Is.Empty);
        }

        [Test]
        public void EmptyNullableGuid()
        {
            Guid? empty = Guid.Empty;
            Assert.That(empty, Is.Empty);
        }

        [Test]
        public void NonEmptyGuid()
        {
            Guid nonEmpty = new Guid("10000000-0000-0000-0000-000000000000");
            Assert.That(nonEmpty, Is.Not.Empty);
        }

        [Test]
        public void NonEmptyNullableGuid()
        {
            Guid? nonEmpty = new Guid("10000000-0000-0000-0000-000000000000");
            Assert.That(nonEmpty, Is.Not.Empty);
        }

        [Test]
        public void NullNullableGuid()
        {
            Guid? nonEmpty = null;
            Assert.That(nonEmpty, Is.Not.Empty);
        }
    }
}
