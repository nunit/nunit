// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Internal.Tests
{
    [TestFixture]
    public class StackFilterTests
    {
        #region Test Data

        private static readonly string NL = NUnit.Env.NewLine;

        private static readonly string shortTrace_Assert =
    @"   at NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 56" + NL +
    @"   at NUnit.Framework.Assert.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 50" + NL +
    @"   at NUnit.Framework.Assert.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 19" + NL +
    @"   at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12" + NL;

        private static readonly string shortTrace_Assume =
    @"   at NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 56" + NL +
    @"   at NUnit.Framework.Assume.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 50" + NL +
    @"   at NUnit.Framework.Assume.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 19" + NL +
    @"   at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12" + NL;

        private static readonly string shortTrace_Result =
    @"at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12" + NL;

        // NOTE: In most cases, NUnit does not have to deal with traces
        // like this because the InnerException of a TargetInvocationException
        // only includes the methods called from the point of invocation.
        // However, in the compact framework, such long traces may arise.
        private static readonly string longTrace =
    @"  at NUnit.Framework.Assert.Fail(String message, Object[] args)" + NL +
    @"  at MyNamespace.MyAppsTests.AssertFailTest()" + NL +
    @"  at System.Reflection.RuntimeMethodInfo.InternalInvoke(RuntimeMethodInfo rtmi, Object obj, BindingFlags invokeAttr, Binder binder, Object parameters, CultureInfo culture, Boolean isBinderDefault, Assembly caller, Boolean verifyAccess, StackCrawlMark& stackMark)" + NL +
    @"  at System.Reflection.RuntimeMethodInfo.InternalInvoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean verifyAccess, StackCrawlMark& stackMark)" + NL +
    @"  at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)" + NL +
    @"  at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)" + NL +
    @"  at NUnitLite.ProxyTestCase.InvokeMethod(MethodInfo method, Object[] args)" + NL +
    @"  at NUnit.Framework.TestCase.RunTest()" + NL +
    @"  at NUnit.Framework.TestCase.RunBare()" + NL +
    @"  at NUnit.Framework.TestCase.Run(TestResult result, TestListener listener)" + NL +
    @"  at NUnit.Framework.TestCase.Run(TestListener listener)" + NL +
    @"  at NUnit.Framework.TestSuite.Run(TestListener listener)" + NL +
    @"  at NUnit.Framework.TestSuite.Run(TestListener listener)" + NL +
    @"  at NUnitLite.Runner.TestRunner.Run(ITest test)" + NL +
    @"  at NUnitLite.Runner.ConsoleUI.Run(ITest test)" + NL +
    @"  at NUnitLite.Runner.TestRunner.Run(Assembly assembly)" + NL +
    @"  at NUnitLite.Runner.ConsoleUI.Run()" + NL +
    @"  at NUnitLite.Runner.ConsoleUI.Main(String[] args)" + NL +
    @"  at OpenNETCF.Linq.Demo.Program.Main(String[] args)" + NL;

        private static readonly string longTrace_Result =
    @"at MyNamespace.MyAppsTests.AssertFailTest()" + NL;

        #endregion

        // NOTE: Using individual tests rather than test cases 
        // to make the error messages easier to read.

        [Test]
        public void FilterShortTraceWithAssert()
        {
            Assert.That(StackFilter.Filter(shortTrace_Assert), Is.EqualTo(shortTrace_Result));
        }

        [Test]
        public void FilterShortTraceWithAssume_Trace1()
        {
            Assert.That(StackFilter.Filter(shortTrace_Assume), Is.EqualTo(shortTrace_Result));
        }

        [Test]
        public void FilterLongTrace()
        {
            Assert.That(StackFilter.Filter(longTrace), Is.EqualTo(longTrace_Result));
        }
    }
}
