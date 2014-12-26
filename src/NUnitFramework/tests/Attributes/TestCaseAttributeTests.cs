// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.TestCaseAttributeFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestCaseAttributeTests
    {
        [TestCase(12, 3, 4)]
        [TestCase(12, 2, 6)]
        [TestCase(12, 4, 3)]
        public void IntegerDivisionWithResultPassedToTest(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCase(12, 3, ExpectedResult = 4)]
        [TestCase(12, 2, ExpectedResult = 6)]
        [TestCase(12, 4, ExpectedResult = 3)]
        public int IntegerDivisionWithResultCheckedByNUnit(int n, int d)
        {
            return n / d;
        }

        [TestCase(2, 2, ExpectedResult=4)]
        public double CanConvertIntToDouble(double x, double y)
        {
            return x + y;
        }

        [TestCase("2.2", "3.3", ExpectedResult = 5.5)]
        public decimal CanConvertStringToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [TestCase(2.2, 3.3, ExpectedResult = 5.5)]
        public decimal CanConvertDoubleToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public decimal CanConvertIntToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public short CanConvertSmallIntsToShort(short x, short y)
        {
            return (short)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public byte CanConvertSmallIntsToByte(byte x, byte y)
        {
            return (byte)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public sbyte CanConvertSmallIntsToSByte(sbyte x, sbyte y)
        {
            return (sbyte)(x + y);
        }

        [TestCase("MethodCausesConversionOverflow", RunState.NotRunnable)]
        [TestCase("VoidTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("TestCaseWithNullableReturnValueAndNullExpectedResult", RunState.Runnable)]
        public void TestCaseRunnableState(string methodName, RunState expectedState)
        {
            var test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName).Tests[0];
            Assert.AreEqual(expectedState, test.RunState);
        }

        [TestCase("12-October-1942")]
        public void CanConvertStringToDateTime(DateTime dt)
        {
            Assert.AreEqual(1942, dt.Year);
        }

        [TestCase(null)]
        public void CanPassNullAsFirstArgument(object a)
        {
            Assert.IsNull(a);
        }

        [TestCase(new object[] { 1, "two", 3.0 })]
        [TestCase(new object[] { "zip" })]
        public void CanPassObjectArrayAsFirstArgument(object[] a)
        {
        }
  
        [TestCase(new object[] { "a", "b" })]
        public void CanPassArrayAsArgument(object[] array)
        {
            Assert.AreEqual("a", array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase("a", "b")]
        public void ArgumentsAreCoalescedInObjectArray(object[] array)
        {
            Assert.AreEqual("a", array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase(1, "b")]
        public void ArgumentsOfDifferentTypeAreCoalescedInObjectArray(object[] array)
        {
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase(ExpectedResult = null)]
        public object ResultCanBeNull()
        {
            return null;
        }

        [TestCase("a", "b")]
        public void HandlesParamsArrayAsSoleArgument(params string[] array)
        {
            Assert.AreEqual("a", array[0]);
            Assert.AreEqual("b", array[1]);
        }

        [TestCase("a")]
        public void HandlesParamsArrayWithOneItemAsSoleArgument(params string[] array)
        {
            Assert.AreEqual("a", array[0]);
        }

        [TestCase("a", "b", "c", "d")]
        public void HandlesParamsArrayAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.AreEqual("a", s1);
            Assert.AreEqual("b", s2);
            Assert.AreEqual("c", array[0]);
            Assert.AreEqual("d", array[1]);
        }

        [TestCase("a", "b")]
        public void HandlesParamsArrayWithNoItemsAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.AreEqual("a", s1);
            Assert.AreEqual("b", s2);
            Assert.AreEqual(0, array.Length);
        }

        [TestCase("a", "b", "c")]
        public void HandlesParamsArrayWithOneItemAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.AreEqual("a", s1);
            Assert.AreEqual("b", s2);
            Assert.AreEqual("c", array[0]);
        }

        [TestCase(1)]
        public void NullableSimpleFormalParametersWithArgument(int? a)
        {
            Assert.AreEqual(1, a);
        }

        [TestCase(null)]
        public void NullableSimpleFormalParametersWithNullArgument(int? a)
        {
            Assert.IsNull(a);
        }

        [TestCase(null, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = 1)]
        public int? TestCaseWithNullableReturnValue(int? a)
        {
            return a;
        }

        [Test]
        public void CanSpecifyDescription()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodHasDescriptionSpecified").Tests[0];
            Assert.AreEqual("My Description", test.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void CanSpecifyTestName()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodHasTestNameSpecified").Tests[0];
            Assert.AreEqual("XYZ", test.Name);
            Assert.AreEqual("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture.XYZ", test.FullName);
        }

        [Test]
        public void CanSpecifyCategory()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodHasSingleCategory").Tests[0];
            IList categories = test.Properties["Category"];
            Assert.AreEqual(new string[] { "XYZ" }, categories);
        }
 
        [Test]
        public void CanSpecifyMultipleCategories()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodHasMultipleCategories").Tests[0];
            IList categories = test.Properties["Category"];
            Assert.AreEqual(new string[] { "X", "Y", "Z" }, categories);
        }
 
        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodWithIgnoredTestCases");

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
                typeof(TestCaseAttributeFixture), "MethodWithExplicitTestCases");

            Test testCase = TestFinder.Find("MethodWithExplicitTestCases(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
 
            testCase = TestFinder.Find("MethodWithExplicitTestCases(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
 
            testCase = TestFinder.Find("MethodWithExplicitTestCases(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
        }
    }
}
