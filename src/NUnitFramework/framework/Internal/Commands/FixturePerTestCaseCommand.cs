// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// ConstructFixtureCommand constructs the user test object if necessary.
    /// </summary>
    public class FixturePerTestCaseCommand : BeforeAndAfterTestCommand
    {
        /// <summary>
        /// Handles the construction and disposement of a fixture per test case
        /// </summary>
        /// <param name="innerCommand">The inner command to which the command applies</param>
        public FixturePerTestCaseCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            TestSuite testSuite = null;
            TestFixture testFixture = null;

            ITest currentTest = Test;
            while (currentTest != null && (testSuite == null || testFixture == null))
            {
                testSuite = testSuite ?? currentTest as TestSuite;
                testFixture = testFixture ?? currentTest as TestFixture;
                currentTest = currentTest.Parent;
            }

            Guard.ArgumentValid(testSuite != null, "FixturePerTestCaseCommand must reference a TestSuite", nameof(innerCommand));

            BeforeTest = (context) =>
            {
                ITypeInfo typeInfo = Test.TypeInfo;

                if (typeInfo != null && !typeInfo.IsStaticClass)
                {
                    context.TestObject = typeInfo.Construct(testSuite.Arguments);
                    Test.Fixture = context.TestObject;
                }
            };

            AfterTest = (context) =>
            {
                ITypeInfo typeInfo = Test.TypeInfo;
                if (typeInfo != null && !typeInfo.IsStaticClass && typeof(IDisposable).IsAssignableFrom(typeInfo.Type))
                {
                    try
                    {
                        var disposable = context.TestObject as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                            context.TestObject = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        context.CurrentResult.RecordTearDownException(ex);
                    }
                }
            };
        }
    }
}

