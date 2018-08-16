// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.TestCaseAttributeFixture;
using NUnit.TestUtilities;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif

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

        [TestCase(nameof(TestCaseAttributeFixture.MethodCausesConversionOverflow), RunState.NotRunnable)]
        [TestCase(nameof(TestCaseAttributeFixture.VoidTestCaseWithExpectedResult), RunState.NotRunnable)]
        [TestCase(nameof(TestCaseAttributeFixture.TestCaseWithNullableReturnValueAndNullExpectedResult), RunState.Runnable)]
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

        [TestCase("1942-10-12")]
        public void CanConvertIso8601DateStringToDateTime(DateTime dt)
        {
            Assert.AreEqual(new DateTime(1942,10,12), dt);
        }

        [TestCase("1942-10-12", ExpectedResult = "1942-10-12")]
        public DateTime CanConvertExpectedResultStringToDateTime(DateTime dt)
        {
            return dt;
        }

        [TestCase("4:44:15")]
        public void CanConvertStringToTimeSpan(TimeSpan ts)
        {
            Assert.AreEqual(4, ts.Hours);
            Assert.AreEqual(44, ts.Minutes);
            Assert.AreEqual(15, ts.Seconds);
        }

        [TestCase("4:44:15", ExpectedResult = "4:44:15")]
        public TimeSpan CanConvertExpectedResultStringToTimeSpan(TimeSpan ts)
        {
            return ts;
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

        [TestCase("x", ExpectedResult = new []{"x", "b", "c"})]
        [TestCase("x", "y", ExpectedResult = new[] { "x", "y", "c" })]
        [TestCase("x", "y", "z", ExpectedResult = new[] { "x", "y", "z" })]
        public string[] HandlesOptionalArguments(string s1, string s2 = "b", string s3 = "c")
        {
            return new[] {s1, s2, s3};
        }

        [TestCase(ExpectedResult = new []{"a", "b"})]
        [TestCase("x", ExpectedResult = new[] { "x", "b" })]
        [TestCase("x", "y", ExpectedResult = new[] { "x", "y" })]
        public string[] HandlesAllOptionalArguments(string s1 = "a", string s2 = "b")
        {
            return new[] {s1, s2};
        }

        [TestCase("a", "b", Explicit = true)]
        public void ShouldNotRunAndShouldNotFailInConsoleRunner()
        {
            Assert.Fail();
        }

        [Test]
        public void CanSpecifyDescription()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasDescriptionSpecified)).Tests[0];
            Assert.AreEqual("My Description", test.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void CanSpecifyTestName_FixedText()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasTestNameSpecified_FixedText)).Tests[0];
            Assert.AreEqual("XYZ", test.Name);
            Assert.AreEqual("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture.XYZ", test.FullName);
        }

        [Test]
        public void CanSpecifyTestName_WithMethodName()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasTestNameSpecified_WithMethodName)).Tests[0];
            var expectedName = "MethodHasTestNameSpecified_WithMethodName+XYZ";
            Assert.AreEqual(expectedName, test.Name);
            Assert.AreEqual("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture." + expectedName, test.FullName);
        }

        [Test]
        public void CanSpecifyCategory()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasSingleCategory)).Tests[0];
            IList categories = test.Properties["Category"];
            Assert.AreEqual(new string[] { "XYZ" }, categories);
        }
 
        [Test]
        public void CanSpecifyMultipleCategories()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasMultipleCategories)).Tests[0];
            IList categories = test.Properties["Category"];
            Assert.AreEqual(new string[] { "X", "Y", "Z" }, categories);
        }
 
        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            var methodName = nameof(TestCaseAttributeFixture.MethodWithIgnoredTestCases);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test testCase = TestFinder.Find($"{methodName}(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
 
            testCase = TestFinder.Find($"{methodName}(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
 
            testCase = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Don't Run Me!"));
        }

        [Test]
        public void CanMarkIndividualTestCasesExplicit()
        {
            var methodName = nameof(TestCaseAttributeFixture.MethodWithExplicitTestCases);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test testCase = TestFinder.Find($"{methodName}(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
 
            testCase = TestFinder.Find($"{methodName}(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
 
            testCase = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
        }

#if PLATFORM_DETECTION
        [Test]
        public void CanIncludePlatform()
        {
            bool isLinux = OSPlatform.CurrentPlatform.IsUnix;
            bool isMacOSX = OSPlatform.CurrentPlatform.IsMacOSX;
            
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), "MethodWithIncludePlatform");

            Test testCase1 = TestFinder.Find("MethodWithIncludePlatform(1)", suite, false);
            Test testCase2 = TestFinder.Find("MethodWithIncludePlatform(2)", suite, false);
            Test testCase3 = TestFinder.Find("MethodWithIncludePlatform(3)", suite, false);
            Test testCase4 = TestFinder.Find("MethodWithIncludePlatform(4)", suite, false);
            if (isLinux)
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
            }
            else if (isMacOSX)
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
            }
            else
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
            }
        }

        [Test]
        public void CanExcludePlatform()
        {
            bool isLinux = OSPlatform.CurrentPlatform.IsUnix;
            bool isMacOSX = OSPlatform.CurrentPlatform.IsMacOSX;

            const string methodName = nameof(TestCaseAttributeFixture.MethodWithExcludePlatform);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test testCase1 = TestFinder.Find($"{methodName}(1)", suite, false);
            Test testCase2 = TestFinder.Find($"{methodName}(2)", suite, false);
            Test testCase3 = TestFinder.Find($"{methodName}(3)", suite, false);
            Test testCase4 = TestFinder.Find($"{methodName}(4)", suite, false);
            if (isLinux)
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
            }
            else if (isMacOSX)
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
            }
            else
            {
                Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
            }
        }
#endif

        [Test]
        public void TestNameIntrospectsArrayValues()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodWithArrayArguments));

            Assert.That(suite.TestCaseCount, Is.EqualTo(4));
            var expectedNames = new[]
            {
                @"MethodWithArrayArguments([])",
                @"MethodWithArrayArguments([1, ""text"", null])",
                @"MethodWithArrayArguments([1, Int32[], 4])",
                @"MethodWithArrayArguments([1, 2, 3, 4, 5, ...])"
            };
            Assert.That(suite.Tests.Select(t => t.Name), Is.EquivalentTo(expectedNames));
        }


        #region Nullable<> tests

        [TestCase(12, 3, 4)]
        [TestCase(12, 2, 6)]
        [TestCase(12, 4, 3)]
        public void NullableIntegerDivisionWithResultPassedToTest(int? n, int? d, int? q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCase(12, 3, ExpectedResult = 4)]
        [TestCase(12, 2, ExpectedResult = 6)]
        [TestCase(12, 4, ExpectedResult = 3)]
        public int? NullableIntegerDivisionWithResultCheckedByNUnit(int? n, int? d)
        {
            return n / d;
        }

        [TestCase(2, 2, ExpectedResult = 4)]
        public double? CanConvertIntToNullableDouble(double? x, double? y)
        {
            return x + y;
        }

        [TestCase(1)]
        public void CanConvertIntToNullableShort(short? x)
        {
            Assert.That(x.HasValue);
            Assert.That(x.Value, Is.EqualTo(1));
        }

        [TestCase(1)]
        public void CanConvertIntToNullableByte(byte? x)
        {
            Assert.That(x.HasValue);
            Assert.That(x.Value, Is.EqualTo(1));
        }

        [TestCase(1)]
        public void CanConvertIntToNullableSByte(sbyte? x)
        {
            Assert.That(x.HasValue);
            Assert.That(x.Value, Is.EqualTo(1));
        }

        [TestCase(1)]
        public void CanConvertIntToNullableLong(long? x)
        {
            Assert.That(x.HasValue);
            Assert.That(x.Value, Is.EqualTo(1));
        }

        [TestCase("2.2", "3.3", ExpectedResult = 5.5)]
        public decimal? CanConvertStringToNullableDecimal(decimal? x, decimal? y)
        {
            Assert.That(x.HasValue);
            Assert.That(y.HasValue);
            return x.Value + y.Value;
        }

        [TestCase(null)]
        public void SupportsNullableDecimal(decimal? x)
        {
            Assert.That(x.HasValue, Is.False);
        }

        [TestCase(2.2, 3.3, ExpectedResult = 5.5)]
        public decimal? CanConvertDoubleToNullableDecimal(decimal? x, decimal? y)
        {
            return x + y;
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public decimal? CanConvertIntToNullableDecimal(decimal? x, decimal? y)
        {
            return x + y;
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public short? CanConvertSmallIntsToNullableShort(short? x, short? y)
        {
            return (short)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public byte? CanConvertSmallIntsToNullableByte(byte? x, byte? y)
        {
            return (byte)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public sbyte? CanConvertSmallIntsToNullableSByte(sbyte? x, sbyte? y)
        {
            return (sbyte)(x + y);
        }

        [TestCase("12-October-1942")]
        public void CanConvertStringToNullableDateTime(DateTime? dt)
        {
            Assert.That(dt.HasValue);
            Assert.AreEqual(1942, dt.Value.Year);
        }

        [TestCase(null)]
        public void SupportsNullableDateTime(DateTime? dt)
        {
            Assert.That(dt.HasValue, Is.False);
        }

        [TestCase("4:44:15")]
        public void CanConvertStringToNullableTimeSpan(TimeSpan? ts)
        {
            Assert.That(ts.HasValue);
            Assert.AreEqual(4, ts.Value.Hours);
            Assert.AreEqual(44, ts.Value.Minutes);
            Assert.AreEqual(15, ts.Value.Seconds);
        }

        [TestCase(null)]
        public void SupportsNullableTimeSpan(TimeSpan? dt)
        {
            Assert.That(dt.HasValue, Is.False);
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

        [TestCase(1, ExpectedResult = 1)]
        public T TestWithGenericReturnType<T>(T arg1)
        {
            return arg1;
        }

#if ASYNC
        [TestCase(1, ExpectedResult = 1)]
        public async Task<T> TestWithAsyncGenericReturnType<T>(T arg1)
        {
            return await Task.Run(() => arg1);
        }
#endif

        #endregion
    }
}
