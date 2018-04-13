// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    #region Mock types

    namespace DifferingNamespace1
    {
        class Dummy
        {
            internal readonly int value;

            public Dummy(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }

        class DummyGeneric<T>
        {
            public DummyGeneric(T obj)
            { }
        }
    }

    namespace DifferingNamespace2
    {
        class Dummy
        {
            internal readonly int value;

            public Dummy(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }
    }

    namespace A
    {
        class GenA<T>
        { }

        class GenB<T>
        { }

        class GenC<T, R>
        { }

        namespace B
        {
            class GenX<T>
            { }

            class GenY<T>
            { }
        }
    }

    namespace B
    {
        class GenA<T>
        { }

        class GenB<T>
        { }

        class GenC<T, R>
        { }

        namespace B
        {
            class GenX<T>
            { }

            class GenY<T>
            { }
        }
    }

    #endregion

    public class TypeNameDifferenceTests
    {
        #region Mock types

        class Dummy
        {
            internal readonly int value;

            public Dummy(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }

        class Dummy1
        {
            internal readonly int value;

            public Dummy1(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return "Dummy " + value;
            }
        }

        class DummyGenericClass<T>
        {
            private readonly object _obj;

            public DummyGenericClass(object obj)
            {
                _obj = obj;
            }

            public override string ToString()
            {
                return _obj.ToString();
            }
        }

        #endregion

        TypeNameDifferenceResolver _differenceGetter;

        [SetUp]
        public void TestSetup()
        {
            _differenceGetter = new TypeNameDifferenceResolver();
        }

        private void TestShortenedNameDifference(object objA, object objB, string expectedA, string expectedB)
        {
            string actualA, actualB;

            _differenceGetter.ResolveTypeNameDifference(
                 objA, objB, out actualA, out actualB);

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
                new KeyValuePair<int, string>(1, ""),
                "TypeNameDifferenceTests+DummyGenericClass`1[TypeNameDifferenceTests+Dummy]",
                "KeyValuePair`2[Int32,String]");

            TestShortenedNameDifference(
                new KeyValuePair<int, string>(1, ""),
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
               new A.GenA<B.GenC<string, B.GenC<string,int>>>(),
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

            Assert.False(_differenceGetter.IsTypeGeneric(notGeneric.GetType()));

            var generic = new DifferingNamespace1.DummyGeneric<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1));

            Assert.That(_differenceGetter.IsTypeGeneric(generic.GetType()));
        }

        [Test]
        public void TestGetTopLevelGenericName()
        {
            var generic = new DifferingNamespace1.DummyGeneric<int>(1).GetType();

            var expected = "NUnit.Framework.Internal.DifferingNamespace1.DummyGeneric`1";

            var actual = _differenceGetter.GetGenericTypeName(generic);

            Assert.AreEqual(expected, actual);
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

            Assert.AreEqual(expected, actual);
        }

        private void TestShortenTypeNames(object objA, object objB, string shortenedA, string shortenedB)
        {
            string actualA, actualB;

            _differenceGetter.ShortenTypeNames(objA.GetType(), objB.GetType(), out actualA, out actualB);

            Assert.AreEqual(shortenedA, actualA);
            Assert.AreEqual(shortenedB, actualB);
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
            string actualA, actualB;

            _differenceGetter.GetShortenedGenericTypes(objA.GetType(), objB.GetType(), out actualA, out actualB);

            Assert.AreEqual(shortenedA, actualA);
            Assert.AreEqual(shortenedB, actualB);
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

            Assert.AreEqual(expectedOutput, actual);
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
