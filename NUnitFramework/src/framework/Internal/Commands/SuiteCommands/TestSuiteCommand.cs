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

namespace NUnit.Framework.Internal.Commands
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestSuiteCommand : SuiteCommand
    {
        private readonly Type fixtureType;
        private readonly Object[] arguments;

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="suite"></param>
        public TestSuiteCommand(TestSuite suite) : base(suite)
        {
            this.fixtureType = Suite.FixtureType;
            this.arguments = Suite.arguments;
        }

        /// <summary>
        /// Does the one time set up for a suite command. Broadly defined,
        /// this includes:
        ///   1. Applying changes specified by attributes to the context
        ///   2. Constructing the user fixture instance
        ///   3. Calling the one time setup methods themselves
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public override void DoOneTimeSetUp(TestExecutionContext context)
        {
            foreach (NUnitAttribute attr in Suite.Attributes)
            {
                IApplyToContext iApply = attr as IApplyToContext;
                if (iApply != null)
                    iApply.ApplyToContext(context);
            }

            if (fixtureType != null)
            {
                // Use pre-constructed fixture if available, otherwise construct it
                if (!IsStaticClass(fixtureType))
                    context.TestObject = Suite.Fixture ?? Reflect.Construct(fixtureType, arguments);

                if (Suite.OneTimeSetUpMethods != null)
                    foreach (MethodInfo method in Suite.OneTimeSetUpMethods)
                        Reflect.InvokeMethod(method, method.IsStatic ? null : context.TestObject);
            }
        }

        /// <summary>
        /// Does the one time tear down for a suite command.
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public override void DoOneTimeTearDown(TestExecutionContext context)
        {
            if (fixtureType != null)
            {
                TestResult suiteResult = context.CurrentResult;

                try
                {
                    if (Suite.OneTimeTearDownMethods != null)
                    {
                        int index = Suite.OneTimeTearDownMethods.Length;
                        while (--index >= 0)
                        {
                            MethodInfo fixtureTearDown = Suite.OneTimeTearDownMethods[index];
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
