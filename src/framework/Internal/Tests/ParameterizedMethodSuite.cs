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

using System.Reflection;
using System.Text;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedMethodSuite holds a collection of individual
    /// TestMethods with their arguments applied.
    /// </summary>
    public class ParameterizedMethodSuite : TestSuite
    {
        private MethodInfo method;

        /// <summary>
        /// Construct from a MethodInfo
        /// </summary>
        /// <param name="method"></param>
        public ParameterizedMethodSuite(MethodInfo method)
            : base(method.ReflectedType.FullName, method.Name)
        {
            this.method = method;
            this.maintainTestOrder = true;
        }

        /// <summary>
        /// Gets the MethodInfo for which this suite is being built.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Creates a ParameterizedMethodResult for this test.
        /// </summary>
        /// <returns>The newly created result.</returns>
        public override TestResult MakeTestResult()
        {
            return new ParameterizedMethodResult(this);
        }

        /// <summary>
        /// Override Run, setting Fixture to that of the Parent.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public override TestResult Run(ITestListener listener)
        {
            if (this.Parent != null)
            {
                this.Fixture = this.Parent.Fixture;
                TestSuite suite = this.Parent as TestSuite;
                if (suite != null)
                {
                    this.setUpMethods = suite.GetSetUpMethods();
                    this.tearDownMethods = suite.GetTearDownMethods();
                }
            }

            //Extensibility.ITestCaseProvider provider = CoreExtensions.Host.TestCaseProviders;

            //if (provider.HasTestCasesFor(this.method))

            //foreach (TestCaseSourceAttribute attr in this.method.GetCustomAttributes(typeof(TestCaseSourceAttribute), false))
            //{
            //}
            
            return base.Run(listener);
        }

        /// <summary>
        /// Override DoOneTimeSetUp to avoid executing any
        /// TestFixtureSetUp method for this suite
        /// </summary>
        /// <param name="suiteResult"></param>
        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
        }

        /// <summary>
        /// Override DoOneTimeTearDown to avoid executing any
        /// TestFixtureTearDown method for this suite.
        /// </summary>
        /// <param name="suiteResult"></param>
        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
        }
    }
}
