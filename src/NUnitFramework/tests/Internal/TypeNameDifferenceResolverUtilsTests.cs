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
    public class TypeNameDifferenceResolverUtilsTests
    {
        TypeNameDifferenceResolverUtils _differenceHelper;

        [SetUp]
        public void TestSetup()
        {
            _differenceHelper = new TypeNameDifferenceResolverUtils();
        }

        [Test]
        public void TestIsObjectInstanceGeneric()
        {
            var notGeneric = new DifferingNamespace1.Dummy(1);

            Assert.False(_differenceHelper.IsTypeGeneric(notGeneric.GetType()));

            var generic = new DifferingNamespace1.DummyGeneric<DifferingNamespace1.Dummy>(new DifferingNamespace1.Dummy(1));

            Assert.That(_differenceHelper.IsTypeGeneric(generic.GetType()));
        }

        [Test]
        public void TestGetTopLevelGenericName()
        {
            var generic = new DifferingNamespace1.DummyGeneric<int>(1).GetType();

            var expected = "NUnit.Framework.Internal.DifferingNamespace1.DummyGeneric`1";

            var actual = _differenceHelper.GetGenericTypeName(generic);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetTopLevelGenericNameThrowsWhenNotGeneric()
        {
            var notGeneric = new object().GetType();

            Assert.Throws<ArgumentException>(() => _differenceHelper.GetGenericTypeName(notGeneric));
        }



        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectSingleGenericParam()
        {
            var generic = new List<int>();

            var expected = new List<Type>() { typeof(int) };
            var actual = _differenceHelper.GetGenericParams(generic.GetType());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectDoubleGenericParam()
        {
            var generic = new KeyValuePair<string, int>();

            var expected = new List<Type>() { typeof(string), typeof(int) };
            var actual = _differenceHelper.GetGenericParams(generic.GetType());

            CollectionAssert.AreEqual(expected, actual);
        }

        private void TestGetTopLevelFullyQualifiedGenericParams(Type FullyQualifiedGenericTypeName, List<Type> ExpectedOutput)
        {
            var actual = _differenceHelper.GetGenericParams(FullyQualifiedGenericTypeName);

            CollectionAssert.AreEqual(ExpectedOutput, actual);
        }


        [Test]
        public void TestReconstructShortenedGenericTypeName()
        {
            var expected = "KeyValuePair`2[String,Int32]";

            var actual = _differenceHelper.ReconstructGenericTypeName(
                "KeyValuePair`2",
                new List<string>() { "String", "Int32" });

            Assert.AreEqual(expected, actual);
        }

        private void TestShortenTypeNames(object objA, object objB, string shortenedA, string shortenedB)
        {
            string actualA, actualB;

            _differenceHelper.ShortenTypeNames(objA.GetType(), objB.GetType(), out actualA, out actualB);

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

            _differenceHelper.GetShortenedGenericTypes(objA.GetType(), objB.GetType(), out actualA, out actualB);

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
            string actual = _differenceHelper.FullyShortenTypeName(type);

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

        [Test]
        public void TestGetFullyQualifiedGenericParametersOfObjectNestedGenerics()
        {
            TestGetTopLevelFullyQualifiedGenericParams(
                new KeyValuePair<int, KeyValuePair<int, string>>().GetType(),
                new List<Type>() { typeof(int), typeof(KeyValuePair<int, string>) });
        }



    }
}