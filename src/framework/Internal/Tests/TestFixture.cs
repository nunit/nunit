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
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// TestFixture is a surrogate for a user test fixture class,
	/// containing one or more tests.
	/// </summary>
	public class TestFixture : TestSuite
	{
		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        public TestFixture(Type fixtureType)
            : this(fixtureType, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="arguments">The arguments.</param>
        public TestFixture(Type fixtureType, object[] arguments)
            : base(fixtureType, arguments) 
        {
#if !NUNITLITE
            this.fixtureSetUpMethods =      GetSetUpTearDownMethods( typeof(TestFixtureSetUpAttribute) );
            this.fixtureTearDownMethods =   GetSetUpTearDownMethods( typeof( TestFixtureTearDownAttribute) );
#endif
            this.setUpMethods =             GetSetUpTearDownMethods( typeof(SetUpAttribute) );
            this.tearDownMethods =          GetSetUpTearDownMethods( typeof(TearDownAttribute) );
        }

        private MethodInfo[] GetSetUpTearDownMethods(Type attrType)
        {
            MethodInfo[] methods = Reflect.GetMethodsWithAttribute(FixtureType, attrType, true);

            foreach ( MethodInfo method in methods )
                if ( method.IsAbstract ||
                     !method.IsPublic && !method.IsFamily ||
                     method.GetParameters().Length > 0 ||
                     !method.ReturnType.Equals(typeof(void)))
                {
                    this.IgnoreReason = string.Format("Invalid signature for SetUp or TearDown method: {0}", method.Name);
                    this.RunState = RunState.NotRunnable;
                    break;
                }

            return methods;
        }
        #endregion

		#region TestSuite Overrides

        /// <summary>
        /// Creates a TestFixtureResult.
        /// </summary>
        /// <returns>The new TestFixtureResult.</returns>
        public override TestResult MakeTestResult()
        {
            return new TestFixtureResult(this);
        }

#if !NUNITLITE
        /// <summary>
        /// Runs the suite under a particular filter, sending
        /// notifications to a listener.
        /// </summary>
        /// <param name="listener">An event listener to receive notifications</param>
        /// <returns></returns>
        //public override TestResult Run(ITestListener listener)
        //{
        //    using ( new DirectorySwapper( AssemblyHelper.GetDirectoryName( FixtureType.Assembly ) ) )
        //    {
        //        return base.Run(listener);
        //    }
        //}
#endif

        /// <summary>
        /// Does the one time set up.
        /// </summary>
        /// <param name="suiteResult">The suite result.</param>
        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
            base.DoOneTimeSetUp(suiteResult);

            suiteResult.AssertCount = Assert.Counter; ;
        }

        /// <summary>
        /// Does the one time tear down.
        /// </summary>
        /// <param name="suiteResult">The suite result.</param>
        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            base.DoOneTimeTearDown(suiteResult);

            suiteResult.AssertCount += Assert.Counter;
        }
        #endregion
	}
}
