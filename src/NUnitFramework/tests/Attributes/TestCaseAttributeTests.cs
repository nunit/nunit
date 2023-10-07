// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestCaseAttributeFixture;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class TestCaseAttributeTests
    {
        [TestCase(12, 3, 4)]
        [TestCase(12, 2, 6)]
        [TestCase(12, 4, 3)]
        public void IntegerDivisionWithResultPassedToTest(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [TestCase(12, 3, ExpectedResult = 4)]
        [TestCase(12, 2, ExpectedResult = 6)]
        [TestCase(12, 4, ExpectedResult = 3)]
        public int IntegerDivisionWithResultCheckedByNUnit(int n, int d)
        {
            return n / d;
        }

        [TestCase(2, 2, ExpectedResult = 4)]
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
            Assert.That(test.RunState, Is.EqualTo(expectedState));
        }

        [TestCase("12-October-1942")]
        public void CanConvertStringToDateTime(DateTime dt)
        {
            Assert.That(dt.Year, Is.EqualTo(1942));
        }

        [TestCase("1942-10-12")]
        public void CanConvertIso8601DateStringToDateTime(DateTime dt)
        {
            Assert.That(dt, Is.EqualTo(new DateTime(1942, 10, 12)));
        }

        [TestCase("1942-10-12", ExpectedResult = "1942-10-12")]
        public DateTime CanConvertExpectedResultStringToDateTime(DateTime dt)
        {
            return dt;
        }

        [TestCase("4:44:15")]
        public void CanConvertStringToTimeSpan(TimeSpan ts)
        {
            Assert.That(ts.Hours, Is.EqualTo(4));
            Assert.That(ts.Minutes, Is.EqualTo(44));
            Assert.That(ts.Seconds, Is.EqualTo(15));
        }

        [TestCase("4:44:15", ExpectedResult = "4:44:15")]
        public TimeSpan CanConvertExpectedResultStringToTimeSpan(TimeSpan ts)
        {
            return ts;
        }

        [TestCase("2018-10-09 15:15:00+02:30")]
        public void CanConvertStringToDateTimeOffset(DateTimeOffset offset)
        {
            Assert.That(offset.Year, Is.EqualTo(2018));
            Assert.That(offset.Month, Is.EqualTo(10));
            Assert.That(offset.Day, Is.EqualTo(9));

            Assert.That(offset.Hour, Is.EqualTo(15));
            Assert.That(offset.Minute, Is.EqualTo(15));
            Assert.That(offset.Second, Is.EqualTo(0));

            Assert.That(offset.Offset.Hours, Is.EqualTo(2));
            Assert.That(offset.Offset.Minutes, Is.EqualTo(30));
        }

        [TestCase("2018-10-09 15:15:00+02:30", ExpectedResult = "2018-10-09 15:15:00+02:30")]
        public DateTimeOffset CanConvertExpectedResultStringToDateTimeOffset(DateTimeOffset offset)
        {
            return offset;
        }

        [TestCase(null)]
        public void CanPassNullAsFirstArgument(object a)
        {
            Assert.That(a, Is.Null);
        }

        [TestCase(new object[] { 1, "two", 3.0 })]
        [TestCase(new object[] { "zip" })]
        public void CanPassObjectArrayAsFirstArgument(object[] a)
        {
        }

        [TestCase(new object[] { "a", "b" })]
        public void CanPassArrayAsArgument(object[] array)
        {
            Assert.That(array[0], Is.EqualTo("a"));
            Assert.That(array[1], Is.EqualTo("b"));
        }

        [TestCase("a", "b")]
        public void ArgumentsAreCoalescedInObjectArray(object[] array)
        {
            Assert.That(array[0], Is.EqualTo("a"));
            Assert.That(array[1], Is.EqualTo("b"));
        }

        [TestCase(1, "b")]
        public void ArgumentsOfDifferentTypeAreCoalescedInObjectArray(object[] array)
        {
            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[1], Is.EqualTo("b"));
        }

        [TestCase(new object?[] { null })]
        public void NullArgumentsAreCoalescedInObjectArray(object?[] array)
        {
            Assert.That(array, Is.EqualTo(new object?[] { null }));
        }

        [TestCase(ExpectedResult = null)]
        public object? ResultCanBeNull()
        {
            return null;
        }

        [TestCase("a", "b")]
        public void HandlesParamsArrayAsSoleArgument(params string[] array)
        {
            Assert.That(array[0], Is.EqualTo("a"));
            Assert.That(array[1], Is.EqualTo("b"));
        }

        [TestCase("a")]
        public void HandlesParamsArrayWithOneItemAsSoleArgument(params string[] array)
        {
            Assert.That(array[0], Is.EqualTo("a"));
        }

        [TestCase("a", "b", "c", "d")]
        public void HandlesParamsArrayAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.That(s1, Is.EqualTo("a"));
            Assert.That(s2, Is.EqualTo("b"));
            Assert.That(array[0], Is.EqualTo("c"));
            Assert.That(array[1], Is.EqualTo("d"));
        }

        [TestCase("a", "b")]
        public void HandlesParamsArrayWithNoItemsAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.That(s1, Is.EqualTo("a"));
            Assert.That(s2, Is.EqualTo("b"));
            Assert.That(array, Is.Empty);
        }

        [TestCase("a", "b", "c")]
        public void HandlesParamsArrayWithOneItemAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.That(s1, Is.EqualTo("a"));
            Assert.That(s2, Is.EqualTo("b"));
            Assert.That(array[0], Is.EqualTo("c"));
        }

        [TestCase("x", ExpectedResult = new[] { "x", "b", "c" })]
        [TestCase("x", "y", ExpectedResult = new[] { "x", "y", "c" })]
        [TestCase("x", "y", "z", ExpectedResult = new[] { "x", "y", "z" })]
        public string[] HandlesOptionalArguments(string s1, string s2 = "b", string s3 = "c")
        {
            return new[] { s1, s2, s3 };
        }

        [TestCase(ExpectedResult = new[] { "a", "b" })]
        [TestCase("x", ExpectedResult = new[] { "x", "b" })]
        [TestCase("x", "y", ExpectedResult = new[] { "x", "y" })]
        public string[] HandlesAllOptionalArguments(string s1 = "a", string s2 = "b")
        {
            return new[] { s1, s2 };
        }

#pragma warning disable NUnit1004 // The TestCaseAttribute provided too many arguments
        [TestCase("a", "b", Explicit = true)]
        public void ShouldNotRunAndShouldNotFailInConsoleRunner()
        {
            Assert.Fail();
        }
#pragma warning restore NUnit1004 // The TestCaseAttribute provided too many arguments

        [Test]
        public void CanSpecifyDescription()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasDescriptionSpecified)).Tests[0];
            Assert.That(test.Properties.Get(PropertyNames.Description), Is.EqualTo("My Description"));
        }

        [Test]
        public void CanSpecifyTestName_FixedText()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasTestNameSpecified_FixedText)).Tests[0];
            Assert.That(test.Name, Is.EqualTo("XYZ"));
            Assert.That(test.FullName, Is.EqualTo("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture.XYZ"));
        }

        [Test]
        public void CanSpecifyTestName_WithMethodName()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasTestNameSpecified_WithMethodName)).Tests[0];
            var expectedName = "MethodHasTestNameSpecified_WithMethodName+XYZ";
            Assert.That(test.Name, Is.EqualTo(expectedName));
            Assert.That(test.FullName, Is.EqualTo("NUnit.TestData.TestCaseAttributeFixture.TestCaseAttributeFixture." + expectedName));
        }

        [Test]
        public void CanSpecifyCategory()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasSingleCategory)).Tests[0];
            IList categories = test.Properties["Category"];
            Assert.That(categories, Is.EqualTo(new[] { "XYZ" }));
        }

        [Test]
        public void CanSpecifyMultipleCategories()
        {
            Test test = (Test)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), nameof(TestCaseAttributeFixture.MethodHasMultipleCategories)).Tests[0];
            IList categories = test.Properties["Category"];
            Assert.That(categories, Is.EqualTo(new[] { "X", "Y", "Z" }));
        }

        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            var methodName = nameof(TestCaseAttributeFixture.MethodWithIgnoredTestCases);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test? testCase = TestFinder.Find($"{methodName}(1)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find($"{methodName}(2)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));

            testCase = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Don't Run Me!"));
        }

        [Test]
        public void CanIgnoreIndividualTestCasesWithUntilDate()
        {
            var methodName = nameof(TestCaseAttributeFixture.MethodWithIgnoredWithUntilDateTestCases);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);
            Test? testCase = TestFinder.Find($"{methodName}(1)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            string untilDateString = DateTimeOffset.Parse("4242-01-01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString("u");
            testCase = TestFinder.Find($"{methodName}(2)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo($"Ignoring until {untilDateString}. Should not run"));
            Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDateString));

            untilDateString = DateTimeOffset.Parse("1942-01-01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString("u");

            testCase = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDateString));

            untilDateString = DateTimeOffset.Parse("4242-01-01T01:23:45Z", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString("u");

            testCase = TestFinder.Find($"{methodName}(4)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo($"Ignoring until {untilDateString}. Don't Run Me!"));
            Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDateString));

            testCase = TestFinder.Find($"{methodName}(5)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void CanMarkIndividualTestCasesExplicit()
        {
            var methodName = nameof(TestCaseAttributeFixture.MethodWithExplicitTestCases);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test? testCase = TestFinder.Find($"{methodName}(1)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find($"{methodName}(2)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));

            testCase = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
        }

        [Test]
        public void CanIncludePlatform()
        {
            bool isLinux = OSPlatform.CurrentPlatform.IsUnix;
            bool isMacOSX = OSPlatform.CurrentPlatform.IsMacOSX;

            const string methodName = nameof(TestCaseAttributeFixture.MethodWithIncludePlatform);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test? testCase1 = TestFinder.Find($"{methodName}(1)", suite, false);
            Test? testCase2 = TestFinder.Find($"{methodName}(2)", suite, false);
            Test? testCase3 = TestFinder.Find($"{methodName}(3)", suite, false);
            Test? testCase4 = TestFinder.Find($"{methodName}(4)", suite, false);
            Assert.That(testCase1, Is.Not.Null);
            Assert.That(testCase2, Is.Not.Null);
            Assert.That(testCase3, Is.Not.Null);
            Assert.That(testCase4, Is.Not.Null);
            if (isLinux)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
                });
            }
            else if (isMacOSX)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Skipped));
                });
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

            Test? testCase1 = TestFinder.Find($"{methodName}(1)", suite, false);
            Test? testCase2 = TestFinder.Find($"{methodName}(2)", suite, false);
            Test? testCase3 = TestFinder.Find($"{methodName}(3)", suite, false);
            Test? testCase4 = TestFinder.Find($"{methodName}(4)", suite, false);
            Assert.That(testCase1, Is.Not.Null);
            Assert.That(testCase2, Is.Not.Null);
            Assert.That(testCase3, Is.Not.Null);
            Assert.That(testCase4, Is.Not.Null);
            if (isLinux)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
            else if (isMacOSX)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase4.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
        }

        [Test]
        public void CanIncludeRuntime()
        {
            bool isNetCore;
            Type? monoRuntimeType = Type.GetType("Mono.Runtime", false);
            bool isMono = monoRuntimeType is not null;
#if NETCOREAPP
            isNetCore = true;
#else
            isNetCore = false;
#endif

            const string methodName = nameof(TestCaseAttributeFixture.MethodWithIncludeRuntime);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test? testCase1 = TestFinder.Find($"{methodName}(1)", suite, false);
            Test? testCase2 = TestFinder.Find($"{methodName}(2)", suite, false);
            Test? testCase3 = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase1, Is.Not.Null);
            Assert.That(testCase2, Is.Not.Null);
            Assert.That(testCase3, Is.Not.Null);
            if (isNetCore)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                });
            }
            else if (isMono)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                });
            }
        }

        [Test]
        public void CanExcludeRuntime()
        {
            bool isNetCore;
            Type? monoRuntimeType = Type.GetType("Mono.Runtime", false);
            bool isMono = monoRuntimeType is not null;
#if NETCOREAPP
            isNetCore = true;
#else
            isNetCore = false;
#endif

            const string methodName = nameof(TestCaseAttributeFixture.MethodWithExcludeRuntime);
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseAttributeFixture), methodName);

            Test? testCase1 = TestFinder.Find($"{methodName}(1)", suite, false);
            Test? testCase2 = TestFinder.Find($"{methodName}(2)", suite, false);
            Test? testCase3 = TestFinder.Find($"{methodName}(3)", suite, false);
            Assert.That(testCase1, Is.Not.Null);
            Assert.That(testCase2, Is.Not.Null);
            Assert.That(testCase3, Is.Not.Null);
            if (isNetCore)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
            else if (isMono)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Skipped));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase1.RunState, Is.EqualTo(RunState.Skipped));
                    Assert.That(testCase2.RunState, Is.EqualTo(RunState.Runnable));
                    Assert.That(testCase3.RunState, Is.EqualTo(RunState.Runnable));
                });
            }
        }

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
            Assert.That(n / d, Is.EqualTo(q));
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

        [TestCase(1)]
        public void CanConvertIntToLong(long x)
        {
            Assert.That(x, Is.EqualTo(1));
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
            return (short?)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public byte? CanConvertSmallIntsToNullableByte(byte? x, byte? y)
        {
            return (byte?)(x + y);
        }

        [TestCase(5, 2, ExpectedResult = 7)]
        public sbyte? CanConvertSmallIntsToNullableSByte(sbyte? x, sbyte? y)
        {
            return (sbyte?)(x + y);
        }

        [TestCase("12-October-1942")]
        public void CanConvertStringToNullableDateTime(DateTime? dt)
        {
            Assert.That(dt.HasValue);
            Assert.That(dt.Value.Year, Is.EqualTo(1942));
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
            Assert.That(ts.Value.Hours, Is.EqualTo(4));
            Assert.That(ts.Value.Minutes, Is.EqualTo(44));
            Assert.That(ts.Value.Seconds, Is.EqualTo(15));
        }

        [TestCase(null)]
        public void SupportsNullableTimeSpan(TimeSpan? dt)
        {
            Assert.That(dt.HasValue, Is.False);
        }

        [TestCase(1)]
        public void NullableSimpleFormalParametersWithArgument(int? a)
        {
            Assert.That(a, Is.EqualTo(1));
        }

        [TestCase(null)]
        public void NullableSimpleFormalParametersWithNullArgument(int? a)
        {
            Assert.That(a, Is.Null);
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

        [TestCase(1, ExpectedResult = 1)]
        public async Task<T> TestWithAsyncGenericReturnType<T>(T arg1)
        {
            return await Task.Run(() => arg1);
        }

        #endregion
    }
}
