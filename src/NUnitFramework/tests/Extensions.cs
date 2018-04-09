// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
    /// <summary>
    /// Contains this assembly's general extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Asserts that the result state has <see cref="TestStatus.Passed"/>.
        /// </summary>
        public static void AssertPassed(this ITestResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), result.Message);
        }

        public static FixtureMethod GetFixtureMethod(this Type type, string methodName)
        {
            return new FixtureMethod(type, type.GetTypeInfo().GetMethod(methodName));
        }

        public static FixtureMethod GetFixtureMethod(this Type type, string methodName, BindingFlags bindingAttr)
        {
            return new FixtureMethod(type, type.GetTypeInfo().GetMethod(methodName, bindingAttr));
        }
    }
}
