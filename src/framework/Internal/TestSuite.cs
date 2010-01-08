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
using System.Text;
using System.Threading;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestSuite represents a composite test, which contains other tests.
    /// </summary>
	[Serializable]
	public class TestSuite : Test
	{
		#region Fields
		/// <summary>
		/// Our collection of child tests
		/// </summary>
		private TestCollection tests = new TestCollection();

        /// <summary>
        /// The fixture setup methods for this suite
        /// </summary>
        protected MethodInfo[] fixtureSetUpMethods;

        /// <summary>
        /// The fixture teardown methods for this suite
        /// </summary>
        protected MethodInfo[] fixtureTearDownMethods;

        /// <summary>
        /// The setup methods for this suite
        /// </summary>
        protected MethodInfo[] setUpMethods;

        /// <summary>
        /// The teardown methods for this suite
        /// </summary>
        protected MethodInfo[] tearDownMethods;

        /// <summary>
        /// Set to true to suppress sorting this suite's contents
        /// </summary>
        protected bool maintainTestOrder;

        /// <summary>
        /// Arguments for use in creating a parameterized fixture
        /// </summary>
        internal object[] arguments;

        /// <summary>
        /// The System.Type of the fixture for this test suite, if there is one
        /// </summary>
        private Type fixtureType;

        /// <summary>
        /// The fixture object, if it has been created
        /// </summary>
        private object fixture;

        #endregion

		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="name">The name of the suite.</param>
		public TestSuite( string name ) 
			: base( name ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="parentSuiteName">Name of the parent suite.</param>
        /// <param name="name">The name of the suite.</param>
		public TestSuite( string parentSuiteName, string name ) 
			: base( parentSuiteName, name ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        public TestSuite(Type fixtureType)
            : this(fixtureType, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="arguments">The arguments.</param>
        public TestSuite(Type fixtureType, object[] arguments)
            : base(fixtureType.FullName)
        {
            string name = TypeHelper.GetDisplayName(fixtureType, arguments);
            this.Name = name;
            
            this.FullName = name;
            string nspace = fixtureType.Namespace;
            if (nspace != null && nspace != "")
                this.FullName = nspace + "." + name;
            this.fixtureType = fixtureType;
            this.arguments = arguments;
        }
        #endregion

		#region Public Methods
        /// <summary>
        /// Sorts tests under this suite.
        /// </summary>
		public void Sort()
		{
            if (!maintainTestOrder)
            {
                this.tests.Sort();

                foreach (Test test in Tests)
                {
                    TestSuite suite = test as TestSuite;
                    if (suite != null)
                        suite.Sort();
                }
            }
		}

#if false
        /// <summary>
        /// Sorts tests under this suite using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public void Sort(IComparer comparer)
        {
			this.tests.Sort(comparer);

			foreach( Test test in Tests )
			{
				TestSuite suite = test as TestSuite;
				if ( suite != null )
					suite.Sort(comparer);
			}
		}
#endif

        /// <summary>
        /// Adds a test to the suite.
        /// </summary>
        /// <param name="test">The test.</param>
		public void Add( Test test ) 
		{
//			if( test.RunState == RunState.Runnable )
//			{
//				test.RunState = this.RunState;
//				test.IgnoreReason = this.IgnoreReason;
//			}
			test.Parent = this;
			tests.Add(test);
		}

        /// <summary>
        /// Adds a pre-constructed test fixture to the suite.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
		public void Add( object fixture )
		{
			Test test = TestFixtureBuilder.BuildFrom( fixture );
			if ( test != null )
				Add( test );
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value></value>
		public override IList Tests 
		{
			get { return tests; }
		}

        /// <summary>
        /// Indicates whether this test is a suite
        /// </summary>
        /// <value></value>
		public override bool IsSuite
		{
			get { return true; }
		}

        /// <summary>
        /// Gets a count of test cases represented by
        /// or contained under this test.
        /// </summary>
        /// <value></value>
		public override int TestCount
		{
			get
			{
				int count = 0;

				foreach(Test test in Tests)
				{
					count += test.TestCount;
				}
				return count;
			}
		}

        /// <summary>
        /// Gets the Type of the fixture used in running this test
        /// </summary>
        /// <value></value>
        public override Type FixtureType
        {
            get { return fixtureType; }
        }

        /// <summary>
        /// Gets or sets a fixture object for running this test
        /// </summary>
        /// <value></value>
        public override object Fixture
        {
            get { return fixture; }
            set { fixture = value; }
        }

        /// <summary>
        /// Gets the set up methods.
        /// </summary>
        /// <returns></returns>
        public MethodInfo[] GetSetUpMethods()
        {
            return setUpMethods;
        }

        /// <summary>
        /// Gets the tear down methods.
        /// </summary>
        /// <returns></returns>
        public MethodInfo[] GetTearDownMethods()
        {
            return tearDownMethods;
        }
        #endregion

		#region Test Overrides
        /// <summary>
        /// Gets a count of test cases that would be run using
        /// the specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
		public override int CountTestCases(TestFilter filter)
		{
			int count = 0;

			if(filter.Pass(this)) 
			{
				foreach(Test test in Tests)
				{
					count += test.CountTestCases(filter);
				}
			}
			return count;
		}

        /// <summary>
        /// Runs the suite under a particular filter, sending
        /// notifications to a listener.
        /// </summary>
        /// <param name="listener">An event listener to receive notifications</param>
        /// <param name="filter">A filter used in running the test</param>
        /// <returns></returns>
		public override TestResult Run(ITestListener listener, TestFilter filter)
		{
			using( new TestContext() )
			{
				TestResult suiteResult = new TestResult( this );

				listener.TestStarted( this );
				long startTime = DateTime.Now.Ticks;

				switch (this.RunState)
				{
					case RunState.Runnable:
					case RunState.Explicit:
                        if (RequiresThread || ApartmentState != GetCurrentApartment())
                            new TestSuiteThread(this).Run(suiteResult, listener, filter);
                        else
                            Run(suiteResult, listener, filter);
						break;

					default:
                    case RunState.Skipped:
				        SkipAllTests(suiteResult, listener, filter);
                        break;
                    case RunState.NotRunnable:
                        MarkAllTestsInvalid( suiteResult, listener, filter);
                        break;
                    case RunState.Ignored:
                        IgnoreAllTests(suiteResult, listener, filter);
                        break;
				}

				long stopTime = DateTime.Now.Ticks;
				double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
				suiteResult.Time = time;

				listener.TestFinished(suiteResult);
				return suiteResult;
			}
		}

        /// <summary>
        /// Runs the suite under a particular filter, sending
        /// notifications to a listener.
        /// </summary>
        /// <param name="suiteResult">The suite result.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="filter">The filter.</param>
        public void Run(TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            suiteResult.Success(); // Assume success
            DoOneTimeSetUp(suiteResult);

            switch( suiteResult.ResultState )
            {
                case ResultState.Failure:
                case ResultState.Error:
                    MarkTestsFailed(Tests, suiteResult, listener, filter);
                    break;
                default:
                    try
                    {
                        RunAllTests(suiteResult, listener, filter);
                    }
                    finally
                    {
                        DoOneTimeTearDown(suiteResult);
                    }
                    break;
            }
        }
		#endregion

		#region Virtual Methods
        /// <summary>
        /// Does the one time set up.
        /// </summary>
        /// <param name="suiteResult">The suite result.</param>
        protected virtual void DoOneTimeSetUp(TestResult suiteResult)
        {
            if (FixtureType != null)
            {
                try
                {
					// In case TestFixture was created with fixture object
					if (Fixture == null && !IsStaticClass( FixtureType ) )
						CreateUserFixture();

                    if (this.Properties["_SETCULTURE"] != null)
                        TestContext.CurrentCulture =
                            new System.Globalization.CultureInfo((string)Properties["_SETCULTURE"]);

                    if (this.Properties["_SETUICULTURE"] != null)
                        TestContext.CurrentUICulture =
                            new System.Globalization.CultureInfo((string)Properties["_SETUICULTURE"]);

                    if (this.fixtureSetUpMethods != null)
                        foreach( MethodInfo fixtureSetUp in fixtureSetUpMethods )
                            Reflect.InvokeMethod(fixtureSetUp, fixtureSetUp.IsStatic ? null : Fixture);
                }
                catch (Exception ex)
                {
                    if (ex is NUnitException || ex is System.Reflection.TargetInvocationException)
                        ex = ex.InnerException;

                    if (ex is NUnit.Framework.IgnoreException)
                    {
                        this.RunState = RunState.Ignored;
                        suiteResult.Ignore(ex.Message);
                        suiteResult.StackTrace = ex.StackTrace;
                        this.IgnoreReason = ex.Message;
                    }
                    else if (ex is NUnit.Framework.AssertionException)
                        suiteResult.Failure(ex.Message, ex.StackTrace);
                    else
                        suiteResult.Error(ex);
                }
            }
        }

        /// <summary>
        /// Creates the user fixture.
        /// </summary>
		protected virtual void CreateUserFixture()
		{
            if (arguments != null && arguments.Length > 0)
                Fixture = Reflect.Construct(FixtureType, arguments);
            else
			    Fixture = Reflect.Construct(FixtureType);
		}

        /// <summary>
        /// Does the one time tear down.
        /// </summary>
        /// <param name="suiteResult">The suite result.</param>
        protected virtual void DoOneTimeTearDown(TestResult suiteResult)
        {
            if ( this.Fixture != null)
            {
                try
                {
                    if (this.fixtureTearDownMethods != null)
                    {
                        int index = fixtureTearDownMethods.Length;
                        while (--index >= 0 )
                        {
                            MethodInfo fixtureTearDown = fixtureTearDownMethods[index];
                            Reflect.InvokeMethod(fixtureTearDown, fixtureTearDown.IsStatic ? null : Fixture);
                        }
                    }

					IDisposable disposable = Fixture as IDisposable;
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


					suiteResult.TearDownError(ex);
				}

                this.Fixture = null;
            }
        }

        #endregion

        #region Helper Methods

        private bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        private void RunAllTests(
			TestResult suiteResult, ITestListener listener, TestFilter filter )
		{
            if (Properties.Contains("Timeout"))
                TestContext.TestCaseTimeout = (int)Properties["Timeout"];

            foreach (Test test in ArrayList.Synchronized(Tests))
            {
                if (filter.Pass(test))
                {
                    RunState saveRunState = test.RunState;

                    if (test.RunState == RunState.Runnable && this.RunState != RunState.Runnable && this.RunState != RunState.Explicit )
                    {
                        test.RunState = this.RunState;
                        test.IgnoreReason = this.IgnoreReason;
                    }

                    TestResult result = test.Run(listener, filter);

                    suiteResult.AddResult(result);

                    if (saveRunState != test.RunState)
                    {
                        test.RunState = saveRunState;
                        test.IgnoreReason = null;
                    }

                    if (result.ResultState == ResultState.Cancelled)
                        break;
                }
            }
		}

        private void SkipAllTests(TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            suiteResult.Skip(this.IgnoreReason);
            MarkTestsNotRun(this.Tests, ResultState.Skipped, this.IgnoreReason, suiteResult, listener, filter);
        }

        private void IgnoreAllTests(TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            suiteResult.Ignore(this.IgnoreReason);
            MarkTestsNotRun(this.Tests, ResultState.Ignored, this.IgnoreReason, suiteResult, listener, filter);
        }

        private void MarkAllTestsInvalid(TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            suiteResult.Invalid(this.IgnoreReason);
            MarkTestsNotRun(this.Tests, ResultState.NotRunnable, this.IgnoreReason, suiteResult, listener, filter);
        }
       
        private void MarkTestsNotRun(
            IList tests, ResultState resultState, string ignoreReason, TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            foreach (Test test in ArrayList.Synchronized(tests))
            {
                if (filter.Pass(test))
                    MarkTestNotRun(test, resultState, ignoreReason, suiteResult, listener, filter);
            }
        }

        private void MarkTestNotRun(
            Test test, ResultState resultState, string ignoreReason, TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            if (test is TestSuite)
            {
                listener.TestStarted(test);
                TestResult result = new TestResult( test );
				result.SetResult( resultState, ignoreReason, null );
                MarkTestsNotRun(test.Tests, resultState, ignoreReason, suiteResult, listener, filter);
                suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
            else
            {
                listener.TestStarted(test);
                TestResult result = new TestResult( test );
                result.SetResult( resultState, ignoreReason, null );
                suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
        }

        private void MarkTestsFailed(
            IList tests, TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            foreach (Test test in ArrayList.Synchronized(tests))
                if (filter.Pass(test))
                    MarkTestFailed(test, suiteResult, listener, filter);
        }

        private void MarkTestFailed(
            Test test, TestResult suiteResult, ITestListener listener, TestFilter filter)
        {
            if (test is TestSuite)
            {
                listener.TestStarted(test);
                TestResult result = new TestResult( test );
				string msg = string.Format( "Parent SetUp failed in {0}", this.FixtureType.Name );
				result.Failure(msg, null);
                MarkTestsFailed(test.Tests, suiteResult, listener, filter);
                suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
            else
            {
                listener.TestStarted(test);
                TestResult result = new TestResult( test );
				string msg = string.Format( "TestFixtureSetUp failed in {0}", this.FixtureType.Name );
				result.Failure(msg, null);
				suiteResult.AddResult(result);
                listener.TestFinished(result);
            }
        }
        #endregion

#if CLR_2_0
        private class TestCollection : System.Collections.Generic.List<Test> { }
#else
        private class TestCollection : ArrayList { }
#endif
    }
}
