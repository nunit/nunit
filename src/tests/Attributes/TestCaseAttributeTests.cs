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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.TestData.TestCaseAttributeFixture;
using NUnit.TestUtilities;
using System.Collections;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public class TestCaseAttributeTests
    {
        [TestCase(12, 3, 4)]
        [TestCase(12, 2, 6)]
        [TestCase(12, 4, 3)]
        [TestCase(12, 0, 0, ExpectedException = typeof(System.DivideByZeroException))]
        [TestCase(12, 0, 0, ExpectedExceptionName = "System.DivideByZeroException")]
        public void IntegerDivisionWithResultPassedToTest(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCase(12, 3, Result = 4)]
        [TestCase(12, 2, Result = 6)]
        [TestCase(12, 4, Result = 3)]
        [TestCase(12, 0, ExpectedException = typeof(System.DivideByZeroException))]
        [TestCase(12, 0, ExpectedExceptionName = "System.DivideByZeroException",
            TestName = "DivisionByZeroThrowsException")]
        public int IntegerDivisionWithResultCheckedByNUnit(int n, int d)
        {
            return n / d;
        }

        [TestCase(2, 2, Result=4)]
        public double CanConvertIntToDouble(double x, double y)
        {
            return x + y;
        }

        [TestCase("2.2", "3.3", Result = 5.5)]
        public decimal CanConvertStringToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [TestCase(2.2, 3.3, Result = 5.5)]
        public decimal CanConvertDoubleToDecimal(decimal x, decimal y)
        {
            return x + y;
        }

        [Test]
		public void ConversionOverflowGivesError()
		{
			Test test = (Test)TestBuilder.MakeTestCase(
				typeof(TestCaseAttributeFixture), "MethodCausesConversionOverflow").Tests[0];
			Assert.AreEqual(RunState.Runnable, test.RunState);
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Error, result.ResultState);
		}

        [TestCase("12-October-1942")]
        public void CanConvertStringToDateTime(DateTime dt)
        {
            Assert.AreEqual(1942, dt.Year);
        }

        [TestCase(42, ExpectedException = typeof(System.Exception),
                   ExpectedMessage = "Test Exception")]
        public void CanSpecifyExceptionMessage(int a)
        {
            throw new System.Exception("Test Exception");
        }

        [TestCase(42, ExpectedException = typeof(System.Exception),
           ExpectedMessage = "Test Exception",
           MatchType=MessageMatch.StartsWith)]
        public void CanSpecifyExceptionMessageAndMatchType(int a)
        {
            throw new System.Exception("Test Exception thrown here");
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

        [Test]
        public void CanSpecifyDescription()
        {
			Test test = (Test)TestBuilder.MakeTestCase(
				typeof(TestCaseAttributeFixture), "MethodHasDescriptionSpecified").Tests[0];
			Assert.AreEqual("My Description", test.Description);
		}

        [Test]
        public void CanSpecifyTestName()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodHasTestNameSpecified").Tests[0];
            Assert.AreEqual("XYZ", test.Name);
            Assert.AreEqual("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture.XYZ", test.FullName);
        }

        [Test]
        public void CanSpecifyExpectedException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsExpectedException").Tests[0];
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Success, result.ResultState);
        }

        [Test]
        public void CanSpecifyExpectedException_WrongException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsWrongException").Tests[0];
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            StringAssert.StartsWith("An unexpected exception type was thrown", result.Message);
        }

        [Test]
        public void CanSpecifyExpectedException_WrongMessage()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsExpectedExceptionWithWrongMessage").Tests[0];
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            StringAssert.StartsWith("The exception message text was incorrect", result.Message);
        }

        [Test]
        public void CanSpecifyExpectedException_NoneThrown()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodThrowsNoException").Tests[0];
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.AreEqual("System.ArgumentNullException was expected", result.Message);
        }

        [Test]
        public void IgnoreTakesPrecedenceOverExpectedException()
        {
            Test test = (Test)TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodCallsIgnore").Tests[0];
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Ignored, result.ResultState);
            Assert.AreEqual("Ignore this", result.Message);
        }

        [Test]
        public void CanIgnoreIndividualTestCase()
        {
            Test test = TestBuilder.MakeTestCase(
                typeof(TestCaseAttributeFixture), "MethodWithIgnoredTestCases");
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);

            ResultSummary summary = new ResultSummary(result);
            Assert.AreEqual(3, summary.ResultCount);
            Assert.AreEqual(2, summary.Ignored);
            Assert.That(result.Results, Has.Some.Message.EqualTo("Don't Run Me!"));
        }
    }
}
