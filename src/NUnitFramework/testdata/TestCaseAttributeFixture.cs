// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.TestCaseAttributeFixture
{
    [TestFixture]
    public class TestCaseAttributeFixture
    {
        [TestCase("12-Octobar-1942")]
        public void MethodHasInvalidDateFormat(DateTime dt)
        { }

        [TestCase(2, 3, 4, Description = "My Description")]
        public void MethodHasDescriptionSpecified(int x, int y, int z)
        { }

        [TestCase(2, 3, 4, TestName = "XYZ")]
        public void MethodHasTestNameSpecified_FixedText(int x, int y, int z)
        { }

        [TestCase(2, 3, 4, TestName = "{m}+XYZ")]
        public void MethodHasTestNameSpecified_WithMethodName(int x, int y, int z)
        { }

        [TestCase(2, 3, 4, Category = "XYZ")]
        public void MethodHasSingleCategory(int x, int y, int z)
        { }

        [TestCase(2, 3, 4, Category = "X,Y,Z")]
        public void MethodHasMultipleCategories(int x, int y, int z)
        { }

        [TestCase(2, 2_000_000, ExpectedResult = 4)]
        public int MethodCausesConversionOverflow(short x, short y)
        {
            return x + y;
        }

        [TestCase(1, ExpectedResult = 1)]
        public void VoidTestCaseWithExpectedResult(int value)
        { }

        [TestCase(2, ExpectedResult = null)]
        public double? TestCaseWithNullableReturnValueAndNullExpectedResult(object input)
        {
            return 2;
        }

        [TestCase(1)]
        [TestCase(2, Ignore = "Should not run")]
        [TestCase(3, IgnoreReason = "Don't Run Me!")]
        public void MethodWithIgnoredTestCases(int num)
        {
        }

        [TestCase(1)]
        [TestCase(2, Ignore = "Should not run", Until = "4242-01-01")]
        [TestCase(3, Ignore = "Run me after 1942", Until = "1942-01-01")]
        [TestCase(4, Ignore = "Don't Run Me!", Until = "4242-01-01T01:23:45Z")]
        [TestCase(5, Until = "This should err!")]
        public void MethodWithIgnoredWithUntilDateTestCases(int num)
        {
        }

        [TestCase(1)]
        [TestCase(2, Explicit = true)]
        [TestCase(3, Explicit = true, Reason = "Connection failing")]
        public void MethodWithExplicitTestCases(int num)
        {
        }

        [TestCase(1, IncludePlatform = "Win")]
        [TestCase(2, IncludePlatform = "Linux")]
        [TestCase(3, IncludePlatform = "MacOSX")]
        [TestCase(4, IncludePlatform = "XBox")]
        public void MethodWithIncludePlatform(int num)
        {
        }

        [TestCase(1, ExcludePlatform = "Win")]
        [TestCase(2, ExcludePlatform = "Linux")]
        [TestCase(3, ExcludePlatform = "MacOSX")]
        [TestCase(4, ExcludePlatform = "XBox")]
        public void MethodWithExcludePlatform(int num)
        {
        }
        [TestCase(1, IncludePlatform = "Net")]
        [TestCase(2, IncludePlatform = "NetCore")]
        [TestCase(3, IncludePlatform = "Mono")]
        public void MethodWithIncludeRuntime(int num)
        {
        }

        [TestCase(1, ExcludePlatform = "Net")]
        [TestCase(2, ExcludePlatform = "NetCore")]
        [TestCase(3, ExcludePlatform = "Mono")]
        public void MethodWithExcludeRuntime(int num)
        {
        }

        [TestCase((object)new object[] { })]
        [TestCase((object)new object[] { 1, "text", null })]
        [TestCase((object)new object[] { 1, new[] { 2, 3 }, 4 })]
        [TestCase((object)new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        public void MethodWithArrayArguments(object o)
        {
        }
    }
}
