// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.TestCaseSourceAttributeFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestCaseSourceTests
    {
        [Test, TestCaseSource("StaticProperty")]
        public void SourceCanBeStaticProperty(string source)
        {
            Assert.AreEqual("StaticProperty", source);
        }

        static IEnumerable StaticProperty
        {
            get { return new object[] { new object[] { "StaticProperty" } }; }
        }

        [Test, TestCaseSource("InstanceProperty")]
        public void SourceCanBeInstanceProperty(string source)
        {
            Assert.AreEqual("InstanceProperty", source);
        }

        IEnumerable InstanceProperty
        {
            get { return new object[] { new object[] { "InstanceProperty" } }; }
        }

        [Test, TestCaseSource("StaticMethod")]
        public void SourceCanBeStaticMethod(string source)
        {
            Assert.AreEqual("StaticMethod", source);
        }

        static IEnumerable StaticMethod()
        {
            return new object[] { new object[] { "StaticMethod" } };
        }

        [Test, TestCaseSource("InstanceMethod")]
        public void SourceCanBeInstanceMethod(string source)
        {
            Assert.AreEqual("InstanceMethod", source);
        }

        IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethod" } };
        }

        [Test, TestCaseSource("StaticField")]
        public void SourceCanBeStaticField(string source)
        {
            Assert.AreEqual("StaticField", source);
        }

        static object[] StaticField =
            { new object[] { "StaticField" } };

        [Test, TestCaseSource("InstanceField")]
        public void SourceCanBeInstanceField(string source)
        {
            Assert.AreEqual("InstanceField", source);
        }

        static object[] InstanceField =
            { new object[] { "InstanceField" } };

        [Test, TestCaseSource(typeof(DataSourceClass))]
        public void SourceCanBeInstanceOfIEnumerable(string source)
        {
            Assert.AreEqual("DataSourceClass", source);
        }

        [Test, TestCaseSource(typeof(DataSourceClass), null, "foo")]
        public void SourceCanBeInstanceOfIEnumerableWithParameters(string source)
        {
            Assert.AreEqual("foo", source);
        }

        class DataSourceClass : IEnumerable
        {
            public DataSourceClass()
            {
            }

            private readonly string enumeratorOverride;

            public DataSourceClass(string enumeratorOverride)
            {
                this.enumeratorOverride = enumeratorOverride;
            }

            public IEnumerator GetEnumerator()
            {
                yield return this.enumeratorOverride ?? "DataSourceClass";
            }
        }

        [Test, TestCaseSource(typeof(DataSourceClassWithMethod), "GetStuff", "foo")]
        public void SourceCanHaveMethodProducingIEnumerableWithParameters(string source)
        {
            string[] items = new[] { "foo1", "foo2", "foo3" };
            bool found = false;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == source)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }

        class DataSourceClassWithMethod
        {
            private readonly string enumeratorPrepend;

            public DataSourceClassWithMethod(string enumeratorPrepend)
            {
                this.enumeratorPrepend = enumeratorPrepend;
            }
            public IEnumerable<string> GetStuff()
            {
                return new[] { enumeratorPrepend + "1", enumeratorPrepend + "2", enumeratorPrepend + "3" };
            }
        }

        [Test, TestCaseSource("MyData")]
        public void SourceMayReturnArgumentsAsObjectArray(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCaseSource("MyData")]
        public void TestAttributeIsOptional(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource("MyIntData")]
        public void SourceMayReturnArgumentsAsIntArray(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource("EvenNumbers")]
        public void SourceMayReturnSinglePrimitiveArgumentAlone(int n)
        {
            Assert.AreEqual(0, n % 2);
        }

        [Test, TestCaseSource("Params")]
        public int SourceMayReturnArgumentsAsParamSet(int n, int d)
        {
            return n / d;
        }

        [Test]
        [TestCaseSource("MyData")]
        [TestCaseSource("MoreData", Category="Extra")]
        [TestCase(12, 2, 6)]
        public void TestMayUseMultipleSourceAttributes(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource("FourArgs")]
        public void TestWithFourArguments(int n, int d, int q, int r)
        {
            Assert.AreEqual(q, n / d);
            Assert.AreEqual(r, n % d);
        }

        [Test, Category("Top"), TestCaseSource(typeof(DivideDataProvider), "HereIsTheData")]
        public void SourceMayBeInAnotherClass(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource(typeof(DivideDataProviderWithReturnValue), "TestCases")]
        public int SourceMayBeInAnotherClassWithReturn(int n, int d)
        {
            return n / d;
        }

        [Test]
        public void IgnoreTakesPrecedenceOverExpectedException()
        {
            ITestResult result = TestBuilder.RunParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), "MethodCallsIgnore").Children[0];
            Assert.AreEqual(ResultState.Ignored, result.ResultState);
            Assert.AreEqual("Ignore this", result.Message);
        }

        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), "MethodWithIgnoredTestCases");

            Test testCase = TestFinder.Find("MethodWithIgnoredTestCases(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
 
            testCase = TestFinder.Find("MethodWithIgnoredTestCases(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
 
            testCase = TestFinder.Find("MethodWithIgnoredTestCases(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Don't Run Me!"));
        }

        [Test]
        public void CanMarkIndividualTestCasesExplicit()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), "MethodWithExplicitTestCases");

            Test testCase = TestFinder.Find("MethodWithExplicitTestCases(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
 
            testCase = TestFinder.Find("MethodWithExplicitTestCases(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
 
            testCase = TestFinder.Find("MethodWithExplicitTestCases(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
        }

        [Test]
        public void HandlesExceptionInTestCaseSource()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), "MethodWithSourceThrowingException").Tests[0];
            Assert.AreEqual(RunState.NotRunnable, testMethod.RunState);
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
            Assert.AreEqual("System.Exception : my message", result.Message);
        }

        [TestCaseSource("exception_source"), Explicit]
        public void HandlesExceptioninTestCaseSource_GuiDisplay(string lhs, string rhs)
        {
            Assert.AreEqual(lhs, rhs);
        }

        object[] testCases =
        {
            new TestCaseData(
                new string[] { "A" },
                new string[] { "B" })
        };

        [Test, TestCaseSource("testCases")]
        public void MethodTakingTwoStringArrays(string[] a, string[] b)
        {
            Assert.That(a, Is.TypeOf(typeof(string[])));
            Assert.That(b, Is.TypeOf(typeof(string[])));
        }

        #region Sources used by the tests
        static object[] MyData = new object[] {
            new object[] { 12, 3, 4 },
            new object[] { 12, 4, 3 },
            new object[] { 12, 6, 2 } };

        static object[] MyIntData = new object[] {
            new int[] { 12, 3, 4 },
            new int[] { 12, 4, 3 },
            new int[] { 12, 6, 2 } };

        static object[] FourArgs = new object[] {
            new TestCaseData( 12, 3, 4, 0 ),
            new TestCaseData( 12, 4, 3, 0 ),
            new TestCaseData( 12, 5, 2, 2 ) };

        static int[] EvenNumbers = new int[] { 2, 4, 6, 8 };

        static object[] MoreData = new object[] {
            new object[] { 12, 1, 12 },
            new object[] { 12, 2, 6 } };

        static object[] Params = new object[] {
            new TestCaseData(24, 3).Returns(8),
            new TestCaseData(24, 2).Returns(12) };

        private class DivideDataProvider
        {
            public static IEnumerable HereIsTheData
            {
                get
                {
                    //yield return new TestCaseData(0, 0, 0)
                    //    .SetName("ThisOneShouldThrow")
                    //    .SetDescription("Demonstrates use of ExpectedException")
                    //    .SetCategory("Junk")
                    //    .SetProperty("MyProp", "zip")
                    //    .Throws(typeof(System.DivideByZeroException));
                    yield return new object[] { 100, 20, 5 };
                    yield return new object[] { 100, 4, 25 };
                }
            }
        }

        public class DivideDataProviderWithReturnValue
        {
            public static IEnumerable TestCases
            {
                get
                {
                    return new object[] {
                        new TestCaseData(12, 3).Returns(4).SetName("TC1"),
                        new TestCaseData(12, 2).Returns(6).SetName("TC2"),
                        new TestCaseData(12, 4).Returns(3).SetName("TC3")
                    };
                }
            }
        }

        private static IEnumerable exception_source
        {
            get
            {
                yield return new TestCaseData("a", "a");
                yield return new TestCaseData("b", "b");

                throw new System.Exception("my message");
            }
        }
        #endregion
    }
}
