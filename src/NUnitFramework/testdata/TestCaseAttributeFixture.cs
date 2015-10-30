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
using NUnit.Framework;

namespace NUnit.TestData.TestCaseAttributeFixture
{
    [TestFixture]
    public class TestCaseAttributeFixture
    {
        [TestCase("12-Octobar-1942")]
        public void MethodHasInvalidDateFormat(DateTime dt)
        {}

        [TestCase(2,3,4,Description="My Description")]
        public void MethodHasDescriptionSpecified(int x, int y, int z)
        {}

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
 
        [TestCase(2, 2000000, ExpectedResult=4)]
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
        [TestCase(2, Explicit = true)]
        [TestCase(3, Explicit = true, Reason = "Connection failing")]
        public void MethodWithExplicitTestCases(int num)
        {
        }
        
#if !PORTABLE
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
        public void MethodWitExcludePlatform(int num)
        {
        }
#endif
    }
}
