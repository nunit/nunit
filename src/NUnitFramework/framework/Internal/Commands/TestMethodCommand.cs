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
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class TestMethodCommand : DelegatingTestCommand
    {
        #region Fields

        /// <summary>
        /// The TestMethod for which this is built
        /// </summary>
        private readonly TestMethod test;

        /// <summary>
        /// The test method
        /// </summary>
        internal MethodInfo method;

        /// <summary>
        /// The SetUp method.
        /// </summary>
        private MethodInfo[] setUpMethods;

        /// <summary>
        /// The teardown method
        /// </summary>
        private MethodInfo[] tearDownMethods;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public TestMethodCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            this.test = Test as TestMethod;            
            this.method = test.Method;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject"></param>
        public override TestResult Execute(object testObject)
        {
            try
            {
                ApplySettingsToExecutionContext();

                Test parent = test.Parent as Test;
                if (parent != null)
                {
                    testObject = parent.Fixture;
                    this.setUpMethods = parent.SetUpMethods;
                    this.tearDownMethods = parent.TearDownMethods;
                }

                return innerCommand.Execute(testObject);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                CurrentResult.RecordException(ex);
                return CurrentResult;
            }
        }

        /// <summary>
        /// Applies the culture settings specified on the test
        /// to the TestExecutionContext.
        /// </summary>
        private void ApplySettingsToExecutionContext()
        {
#if !NETCF
            string setCulture = (string)test.Properties.Get(PropertyNames.SetCulture);
            if (setCulture != null)
                TestExecutionContext.CurrentContext.CurrentCulture =
                    new System.Globalization.CultureInfo(setCulture);

            string setUICulture = (string)test.Properties.Get(PropertyNames.SetUICulture);
            if (setUICulture != null)
                TestExecutionContext.CurrentContext.CurrentUICulture =
                    new System.Globalization.CultureInfo(setUICulture);
#endif

#if !NUNITLITE
            if (test.Properties.ContainsKey(PropertyNames.Timeout))
                TestExecutionContext.CurrentContext.TestCaseTimeout = (int)test.Properties.Get(PropertyNames.Timeout);
#endif
        }
    }
}
