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

#if NUNITLITE
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Static class used by NUnitLite to build test cases.
    /// </summary>
    public class NUnitLiteTestCaseBuilder
    {
        /// <summary>
        /// Determines whether the specified method is a test method.
        /// </summary>
        /// <param name="method">The method to examine.</param>
        /// <returns>
        /// 	<c>true</c> if the specified method is a test method; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTestMethod(MethodInfo method)
        {
            return method.IsDefined(typeof(TestAttribute), true)
                || method.IsDefined(typeof(TestCaseAttribute), true)
                || method.IsDefined(typeof(TestCaseSourceAttribute), true);
        }

        /// <summary>
        /// Builds a test from a specified method
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static Test BuildFrom(MethodInfo method)
        {
            IList testdata = GetTestCaseData(method);

            if (testdata.Count == 0)
                return BuildTestMethod(method, null);

            ParameterizedMethodSuite testcases = new ParameterizedMethodSuite(method);

            foreach (ITestCaseData testCase in testdata)
                testcases.Add(BuildTestMethod(method, testCase.Arguments));

            return testcases;
        }

        private static IList GetTestCaseData(MethodInfo method)
        {
            ObjectList testCases = new ObjectList();

            foreach (ITestCaseSource source in method.GetCustomAttributes(typeof(ITestCaseSource), false))
            {
                foreach (ITestCaseData testCase in source.GetTestCasesFor(method))
                    testCases.Add(testCase);
            }

            return testCases;
        }

        private static Test BuildTestMethod(MethodInfo method, object[] args)
        {
            TestMethod testMethod = new TestMethod(method);

            testMethod.arguments = args;

            if (HasValidSignature(testMethod, method, args))
            {
                testMethod.ApplyCommonAttributes(method);

                ExpectedExceptionAttribute[] attributes =
                    (ExpectedExceptionAttribute[])method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false);

                if (attributes.Length > 0)
                    testMethod.ExceptionProcessor = new ExpectedExceptionProcessor(testMethod, attributes[0]);
            }

            return testMethod;
        }

        /// <summary>
        /// Determines whether the method has a valid signature and sets
        /// the RunState to NotRunnable if it does not.
        /// </summary>
        /// <param name="test">The test method being checked</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// 	<c>true</c> if the signature is valid; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasValidSignature(TestMethod test, MethodInfo method, object[] args)
        {
            if (method.ReturnType != typeof(void))
            {
                test.RunState = RunState.NotRunnable;
                test.IgnoreReason = "A TestMethod must return void";
                return false;
            }

            int argsNeeded = method.GetParameters().Length;
            int argsPassed = args == null ? 0 : args.Length;

            if (argsNeeded == 0 && argsPassed > 0)
            {
                test.RunState = RunState.NotRunnable;
                test.IgnoreReason = "Arguments may not be specified for a method with no parameters";
                return false;
            }

            if (argsNeeded > 0 && argsPassed == 0)
            {
                test.RunState = RunState.NotRunnable;
                test.IgnoreReason = "No arguments provided for a method requiring them";
                return false;
            }

            if (argsNeeded != argsPassed)
            {
                test.RunState = RunState.NotRunnable;
                test.IgnoreReason = string.Format("Expected {0} arguments, but received {1}", argsNeeded, argsPassed);
                return false;
            }

            return true;
        }
    }
}
#endif

