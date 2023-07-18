// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    #region Mock types

    namespace DifferingNamespace1
    {
        internal class Dummy
        {
            internal readonly int Value;

            public Dummy(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }

        internal class DummyGeneric<T>
        {
            public DummyGeneric(T obj)
            { }
        }
    }

    namespace DifferingNamespace2
    {
        internal class Dummy
        {
            internal readonly int Value;

            public Dummy(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }
    }

    namespace A
    {
        internal class GenA<T>
        { }

        internal class GenB<T>
        { }

        internal class GenC<T, R>
        { }

        namespace B
        {
            internal class GenX<T>
            { }

            internal class GenY<T>
            { }
        }
    }

    namespace B
    {
        internal class GenA<T>
        { }

        internal class GenB<T>
        { }

        internal class GenC<T, R>
        { }

        namespace B
        {
            internal class GenX<T>
            { }

            internal class GenY<T>
            { }
        }
    }

    #endregion

    public class TypeNameDifferenceTests
    {
        #region Mock types

        private class Dummy
        {
            internal readonly int Value;

            public Dummy(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }

        private class Dummy1
        {
            internal readonly int Value;

            public Dummy1(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }

        private class DummyGenericClass<T>
        {
            private readonly object _obj;

            public DummyGenericClass(object obj)
            {
                _obj = obj;
            }

            public override string? ToString()
            {
                return _obj.ToString();
            }
        }

        #endregion

        private TypeNameDifferenceResolver? _differenceGetter;

        [SetUp]
        public void TestSetup()
        {
            _differenceGetter = new TypeNameDifferenceResolver();
        }

        private void TestShortenedNameDifference(object objA, object objB, string expectedA, string expectedB)
        {
            _differenceGetter.ResolveTypeNameDifference(
                 objA, objB, out var actualA, out var actualB);

            Assert.That(actualA, Is.EqualTo(expectedA));
            Assert.That(actualB, Is.EqualTo(expectedB));
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericDifferingTypes()
        {
            TestShortenedNameDifference(
                new Dummy(1),
                new Dummy1(1),
                "TypeNameDifferenceTests+Dummy",
                "TypeNameDifferenceTests+Dummy1");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypes()
        {
            TestShortenedNameDifference(
                new Dummy(1),
                new Dummy(1),
                "TypeNameDifferenceTests+Dummy",
                "TypeNameDifferenceTests+Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypesSingularDiffNamespace()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.Dummy(1),
                new Dummy(1),
                "Dummy",
                "TypeNameDifferenceTests+Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceNonGenericNonDifferingTypesBothDiffNamespace()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.Dummy(1),
                new DifferingNamespace2.Dummy(1),
                "DifferingNamespace1.Dummy",
                "DifferingNamespace2.Dummy");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGeneric()
        {
            TestShortenedNameDifference(
                new DummyGenericClass<Dummy1>(new Dummy(1)),
                new DummyGenericClass<Dummy>(new Dummy(1)),
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy1]",
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGenericDifferingNamespaces()
        {
            TestShortenedNameDifference(
                new DummyGenericClass<Dummy>(new Dummy(1)),
                new DummyGenericClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]",
                "TypeNameDifferenceTests+DummyGenericClass`1[Dummy]");

            TestShortenedNameDifference(
                new DummyGenericClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                new DummyGenericClass<Dummy>(new Dummy(1)),
                "TypeNameDifferenceTests+DummyGenericClass`1[Dummy]",
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]");

            TestShortenedNameDifference(
                new DummyGenericClass<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1)),
                new DummyGenericClass<DifferingNamespace2.Dummy>(new DifferingNamespace2.Dummy(1)),
                "TypeNameDifferenceTests+DummyGenericClass`1[DifferingNamespace1.Dummy]",
                "TypeNameDifferenceTests+DummyGenericClass`1[DifferingNamespace2.Dummy]");
        }

        [Test]
        public void TestResolveTypeNameDifferenceGenericDifferentAmountGenericParams()
        {
            TestShortenedNameDifference(
                new DummyGenericClass<Dummy>(new Dummy(1)),
                new KeyValuePair<int, string>(1, string.Empty),
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]",
                "KeyValuePair`2[Int32,String]");

            TestShortenedNameDifference(
                new KeyValuePair<int, string>(1, string.Empty),
                new DummyGenericClass<Dummy>(new Dummy(1)),
                "KeyValuePair`2[Int32,String]",
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]");
        }

        [Test]
        public void TestResolveNameDifferenceOneIsGenericOtherIsNot()
        {
            TestShortenedNameDifference(
                new DummyGenericClass<Dummy>(new Dummy(1)),
                new Dummy(1),
                "DummyGenericClass`1[Dummy]",
                "Dummy");

            TestShortenedNameDifference(
                new Dummy(1),
                new DummyGenericClass<Dummy>(new Dummy(1)),
                "Dummy",
                "DummyGenericClass`1[Dummy]");

            TestShortenedNameDifference(
                new KeyValuePair<string, int>("str", 0),
                new Dummy(1),
                "KeyValuePair`2[String,Int32]",
                "Dummy");

            TestShortenedNameDifference(
                new Dummy(1),
                new KeyValuePair<string, int>("str", 0),
                "Dummy",
                "KeyValuePair`2[String,Int32]");

            TestShortenedNameDifference(
                new Dummy(1),
                new A.GenA<B.GenA<B.GenC<string, int>>>(),
                "Dummy",
                "GenA`1[GenA`1[GenC`2[String,Int32]]]");

            TestShortenedNameDifference(
                new A.GenA<B.GenA<B.GenC<string, int>>>(),
                new Dummy(1),
                "GenA`1[GenA`1[GenC`2[String,Int32]]]",
                "Dummy");
        }

        [Test]
        public void TestNestedGenerics()
        {
            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<List<string>>(new List<string>()),
                new DifferingNamespace1.DummyGeneric<IEnumerable<string>>(new List<string>()),
                "DummyGeneric`1[List`1[String]]",
                "DummyGeneric`1[IEnumerable`1[String]]");

            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<IEnumerable<string>>(new List<string>()),
                new DifferingNamespace1.DummyGeneric<List<string>>(new List<string>()),
                "DummyGeneric`1[IEnumerable`1[String]]",
                "DummyGeneric`1[List`1[String]]");

            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>>(new KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>()),
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>>(new KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>()),
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace1.Dummy,DifferingNamespace2.Dummy]]",
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace2.Dummy,DifferingNamespace1.Dummy]]");

            TestShortenedNameDifference(
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>>(new KeyValuePair<DifferingNamespace2.Dummy, DifferingNamespace1.Dummy>()),
                new DifferingNamespace1.DummyGeneric<KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>>(new KeyValuePair<DifferingNamespace1.Dummy, DifferingNamespace2.Dummy>()),
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace2.Dummy,DifferingNamespace1.Dummy]]",
                "DummyGeneric`1[KeyValuePair`2[DifferingNamespace1.Dummy,DifferingNamespace2.Dummy]]");

            TestShortenedNameDifference(
                new A.GenA<A.B.GenX<int>>(),
                new B.GenA<A.B.GenX<short>>(),
                "A.GenA`1[GenX`1[Int32]]",
                "B.GenA`1[GenX`1[Int16]]");

            TestShortenedNameDifference(
                new B.GenA<A.B.GenX<short>>(),
                new A.GenA<A.B.GenX<int>>(),
                "B.GenA`1[GenX`1[Int16]]",
                "A.GenA`1[GenX`1[Int32]]");

            TestShortenedNameDifference(
                new A.GenC<int, string>(),
                new B.GenC<int, A.GenA<string>>(),
                "A.GenC`2[Int32,String]",
                "B.GenC`2[Int32,GenA`1[String]]");

            TestShortenedNameDifference(
                new A.GenA<A.GenC<string, int>>(),
                new B.GenC<A.GenA<List<int>>, B.GenC<string, int>>(),
                "GenA`1[GenC`2[String,Int32]]",
                "GenC`2[GenA`1[List`1[Int32]],GenC`2[String,Int32]]");

            TestShortenedNameDifference(
               new B.GenC<A.GenA<List<int>>, B.GenC<string, int>>(),
               new A.GenA<A.GenC<string, int>>(),
               "GenC`2[GenA`1[List`1[Int32]],GenC`2[String,Int32]]",
               "GenA`1[GenC`2[String,Int32]]");

            TestShortenedNameDifference(
               new B.GenC<A.GenA<List<int>>, B.GenC<string, B.GenC<string, int>>>(),
               new A.GenA<B.GenC<string, B.GenC<string, int>>>(),
               "GenC`2[GenA`1[List`1[Int32]],GenC`2[String,GenC`2[String,Int32]]]",
               "GenA`1[GenC`2[String,GenC`2[String,Int32]]]");

            TestShortenedNameDifference(
               new A.GenA<B.GenC<string, B.GenC<string, int>>>(),
               new B.GenC<A.GenA<List<int>>, B.GenC<string, B.GenC<string, int>>>(),
               "GenA`1[GenC`2[String,GenC`2[String,Int32]]]",
               "GenC`2[GenA`1[List`1[Int32]],GenC`2[String,GenC`2[String,Int32]]]");
        }

        [Test]
        public void TestIsObjectInstanceGeneric()
        {
            var notGeneric = new DifferingNamespace1.Dummy(1);

            Assert.That(_differenceGetter.IsTypeGeneric(notGeneric.GetType()), Is.False);

            var generic = new DifferingNamespace1.DummyGeneric<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1));

            Assert.That(_differenceGetter.IsTypeGeneric(generic.GetType()));
        }

        [Test]
        public void TestGetTopLevelGenericName()
        {
            var generic = new DifferingNamespace1.DummyGeneric<int>(1).GetType();

            var expected = "NUnit.Framework.Tests.Internal.DifferingNamespace1.DummyGeneric`1";

            var actual = _differenceGetter.GetGenericTypeName(generic);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetTopLevelGenericNameThrowsWhenNotGeneric()
        {
            var notGeneric = new object().GetType();

            Assert.Throws<ArgumentException>(() => _differenceGetter.GetGenericTypeName(notGeneric));
        }

        [Test]
        public void TestReconstructShortenedGenericTypeName()
        {
            var expected = "KeyValuePair`2[String,Int32]";

            var actual = _differenceGetter.ReconstructGenericTypeName(
                "KeyValuePair`2",
                new List<string>() { "String", "Int32" });

            Assert.That(actual, Is.EqualTo(expected));
        }

        private void TestShortenTypeNames(object objA, object objB, string shortenedA, string shortenedB)
        {
            _differenceGetter.ShortenTypeNames(objA.GetType(), objB.GetType(), out var actualA, out var actualB);

            Assert.That(actualA, Is.EqualTo(shortenedA));
            Assert.That(actualB, Is.EqualTo(shortenedB));
        }

        [Test]
        public void TestShortenTypeNamesDifferingNamespace()
        {
            TestShortenTypeNames(
               new DifferingNamespace1.Dummy(1),
               new DifferingNamespace2.Dummy(1),
               "DifferingNamespace1.Dummy",
               "DifferingNamespace2.Dummy");
        }

        private void TestShortenGenericTopLevelTypeNames(object objA, object objB, string shortenedA, string shortenedB)
        {
            _differenceGetter.GetShortenedGenericTypes(objA.GetType(), objB.GetType(), out var actualA, out var actualB);

            Assert.That(actualA, Is.EqualTo(shortenedA));
            Assert.That(actualB, Is.EqualTo(shortenedB));
        }

        [Test]
        public void TestShortenGenericTopLevelTypes()
        {
            TestShortenGenericTopLevelTypeNames(
                new A.GenA<int>(),
                new B.GenA<int>(),
                "A.GenA`1",
                "B.GenA`1");

            TestShortenGenericTopLevelTypeNames(
                new KeyValuePair<string, int>(),
                new KeyValuePair<int, string>(),
                "KeyValuePair`2",
                "KeyValuePair`2");
        }

        private void TestFullyShortenTypeName(Type type, string expectedOutput)
        {
            string actual = _differenceGetter.FullyShortenTypeName(type);

            Assert.That(actual, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void TestFullyShortenTypeName()
        {
            TestFullyShortenTypeName(
                new A.GenA<A.GenA<int>>().GetType(),
                "GenA`1[GenA`1[Int32]]");

            TestFullyShortenTypeName(
                new A.GenC<B.GenA<int>, A.GenA<int>>().GetType(),
                "GenC`2[GenA`1[Int32],GenA`1[Int32]]");
        }
    }
}
