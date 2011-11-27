// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using NUnit.Framework.Api;
using System.Diagnostics;

namespace NUnit.Framework.Internal.Commands
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestSuiteCommand : TestCommand
    {
        private readonly TestSuite suite;
        private readonly Type fixtureType;
        private readonly Object[] arguments;

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="test"></param>
        public TestSuiteCommand(TestSuite test) : base(test)
        {
            this.suite = test;
            this.fixtureType = test.FixtureType;
            this.arguments = test.arguments;
        }

        /// <summary>
        /// TODO: Documentation needed for method
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            throw new NotImplementedException("Execute is not implemented for TestSuiteCommand");
        }

        /// <summary>
        /// Does the one time set up for a suite command.
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public virtual void DoOneTimeSetUp(TestExecutionContext context)
        {
            if (fixtureType != null)
            {
                if (context.TestObject == null && !IsStaticClass(fixtureType))
                    context.TestObject = Reflect.Construct(fixtureType, arguments);

                if (suite.OneTimeSetUpMethods != null)
                    foreach (MethodInfo method in suite.OneTimeSetUpMethods)
                        Reflect.InvokeMethod(method, method.IsStatic ? null : context.TestObject);
            }
        }

        /// <summary>
        /// Does the one time tear down for a suite command.
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public virtual void DoOneTimeTearDown(TestExecutionContext context)
        {
            if (fixtureType != null)
            {
                TestSuiteResult suiteResult = context.CurrentResult as TestSuiteResult;

                try
                {
                    if (suite.OneTimeTearDownMethods != null)
                    {
                        int index = suite.OneTimeTearDownMethods.Length;
                        while (--index >= 0)
                        {
                            MethodInfo fixtureTearDown = suite.OneTimeTearDownMethods[index];
                            Reflect.InvokeMethod(fixtureTearDown, fixtureTearDown.IsStatic ? null : context.TestObject);
                        }
                    }

                    IDisposable disposable = context.TestObject as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
                catch (Exception ex)
                {
                    // Error in TestFixtureTearDown or Dispose causes the
                    // suite to be marked as a error, even if
                    // all the contained tests passed.
                    NUnitException nex = ex as NUnitException;
                    if (nex != null)
                        ex = nex.InnerException;

                    // TODO: Can we move this logic into TestResult itself?
                    string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
                    if (suiteResult.Message != null)
                        message = suiteResult.Message + NUnit.Env.NewLine + message;

#if !NETCF_1_0
                    string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
                    if (suiteResult.StackTrace != null)
                        stackTrace = suiteResult.StackTrace + NUnit.Env.NewLine + stackTrace;

                    // TODO: What about ignore exceptions in teardown?
                    suiteResult.SetResult(ResultState.Error, message, stackTrace);
#else
                    Result.SetResult(ResultState.Error, message);
#endif
                }
            }
        }

        private static bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }
    }
}
