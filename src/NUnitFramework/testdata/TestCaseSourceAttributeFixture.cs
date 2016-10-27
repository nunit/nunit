// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.TestData.TestCaseSourceAttributeFixture
{
    [TestFixture]
    public class TestCaseSourceAttributeFixture
    {
        #region Test Calling Assert.Ignore

        [TestCaseSource("source")]
        public void MethodCallsIgnore(int x, int y, int z)
        {
            Assert.Ignore("Ignore this");
        }

#pragma warning disable 414
        private static object[] source = new object[] {
            new TestCaseData( 2, 3, 4 ) };
#pragma warning restore 414

        #endregion

        #region Test With Ignored TestCaseData

        [TestCaseSource("ignored_source")]
        public void MethodWithIgnoredTestCases(int num)
        {
        }

        private static IEnumerable ignored_source
        {
            get
            {
                return new object[] {
                    new TestCaseData(1),
                    new TestCaseData(2).Ignore("Don't Run Me!")
                };
            }
        }

        #endregion

        #region Test With Explicit TestCaseData

        [TestCaseSource("explicit_source")]
        public void MethodWithExplicitTestCases(int num)
        {
        }

        private static IEnumerable explicit_source
        {
            get
            {
                return new object[] {
                    new TestCaseData(1),
                    new TestCaseData(2).Explicit(),
                    new TestCaseData(3).Explicit("Connection failing")
                };
            }
        }

        #endregion

        #region Tests Using Instance Members as Source

        [Test, TestCaseSource("InstanceProperty")]
        public void MethodWithInstancePropertyAsSource(string source)
        {
            Assert.AreEqual("InstanceProperty", source);
        }

        IEnumerable InstanceProperty
        {
            get { return new object[] { new object[] { "InstanceProperty" } }; }
        }

        [Test, TestCaseSource("InstanceMethod")]
        public void MethodWithInstanceMethodAsSource(string source)
        {
            Assert.AreEqual("InstanceMethod", source);
        }

        IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethod" } };
        }

        [Test, TestCaseSource("InstanceField")]
        public void MethodWithInstanceFieldAsSource(string source)
        {
            Assert.AreEqual("InstanceField", source);
        }

#pragma warning disable 414
        object[] InstanceField = { new object[] { "InstanceField" } };
#pragma warning restore 414

        #endregion

        [Test, TestCaseSource(typeof(DivideDataProvider), "MyField", new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingParamsToField(int n, int d, int q)
        {
        }

        [Test, TestCaseSource(typeof(DivideDataProvider), "MyProperty", new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingParamsToProperty(int n, int d, int q)
        {
        }

        [Test, TestCaseSource(typeof(DivideDataProvider), "HereIsTheDataWithParameters", new object[] { 100, 4 })]
        public void SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam(int n, int d, int q)
        {
        }

        [TestCaseSource("exception_source")]
        public void MethodWithSourceThrowingException(string lhs, string rhs)
        {
        }

        static IEnumerable exception_source
        {
            get
            {
                yield return new TestCaseData("a", "a");
                yield return new TestCaseData("b", "b");

                throw new System.Exception("my message");
            }
        }

        class DivideDataProvider
        {
#pragma warning disable 0169, 0649    // x is never assigned
            static object[] myObject;
            public static string MyField;
#pragma warning restore 0169, 0649
            public static int MyProperty { get; set; }
            public static IEnumerable HereIsTheDataWithParameters(int inject1, int inject2, int inject3)
            {
                yield return new object[] { inject1, inject2, inject3 };
            }
            public static IEnumerable HereIsTheData
            {
                get
                {
                    yield return new object[] { 100, 20, 5 };
                    yield return new object[] { 100, 4, 25 };
                }
            }
        }
    }
}