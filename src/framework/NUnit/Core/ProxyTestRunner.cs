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

namespace NUnit.Core
{
	using System;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// DelegatingTestRUnner is the abstract base for core TestRunner
	/// implementations that operate by controlling a downstream
	/// TestRunner. All calls are simply passed on to the
	/// TestRunner that is provided to the constructor.
	/// 
	/// Although the class is abstract, it has no abstract 
	/// methods specified because each implementation will
	/// need to override different methods. All methods are
	/// specified using interface syntax and the derived class
	/// must explicitly implement TestRunner in order to 
	/// redefine the selected methods.
	/// </summary>
	public abstract class ProxyTestRunner : MarshalByRefObject, TestRunner
	{
		#region Instance Variables

		/// <summary>
		/// Our runner ID
		/// </summary>
		protected int runnerID;

		/// <summary>
		/// The downstream TestRunner
		/// </summary>
		private TestRunner testRunner;

		/// <summary>
		/// The event listener for the currently running test
		/// </summary>
		protected ITestListener listener;

		#endregion

		#region Construction
		public ProxyTestRunner(TestRunner testRunner)
		{
			this.testRunner = testRunner;
			this.runnerID = testRunner.ID;
		}

		/// <summary>
		/// Protected constructor for runners that delay creation
		/// of their downstream runner.
		/// </summary>
		protected ProxyTestRunner( int runnerID )
		{
			this.runnerID = runnerID;
		}
		#endregion

		#region Properties
		public virtual int ID
		{
			get { return runnerID; }
		}

		public virtual bool Running
		{
			get { return testRunner != null && testRunner.Running; }
		}

		public virtual TestAssemblyInfo[] AssemblyInfo
		{
			get { return testRunner == null ? new TestAssemblyInfo[0] : testRunner.AssemblyInfo; }
		}

		public virtual ITest Test
		{
			get { return testRunner == null ? null : testRunner.Test; }
		}

		public virtual TestResult TestResult
		{
			get { return testRunner == null ? null : testRunner.TestResult; }
		}

		/// <summary>
		/// Protected property copies any settings to the downstream test runner
		/// when it is set. Derived runners overriding this should call the base
		/// or copy the settings themselves.
		/// </summary>
		protected virtual TestRunner TestRunner
		{
			get { return testRunner; }
			set { testRunner = value; }
		}
		#endregion

		#region Load and Unload Methods
		public virtual bool Load( TestPackage package )
		{
			return this.testRunner.Load( package );
		}

		public virtual void Unload()
		{
            if ( this.testRunner != null )
			    this.testRunner.Unload();
		}
		#endregion

		#region CountTestCases
		public virtual int CountTestCases( TestFilter filter )
		{
			return this.testRunner.CountTestCases( filter );
		}
		#endregion

		#region Methods for Running Tests
		public virtual TestResult Run(ITestListener listener)
		{
			// Save active listener for derived classes
			this.listener = listener;
			return this.testRunner.Run(listener);
		}

		public virtual TestResult Run(ITestListener listener, TestFilter filter)
		{
			// Save active listener for derived classes
			this.listener = listener;
			return this.testRunner.Run(listener, filter);
		}

		public virtual void BeginRun( ITestListener listener )
		{
			// Save active listener for derived classes
			this.listener = listener;
			this.testRunner.BeginRun( listener );
		}

		public virtual void BeginRun( ITestListener listener, TestFilter filter )
		{
			// Save active listener for derived classes
			this.listener = listener;
			this.testRunner.BeginRun( listener, filter );
		}

		public virtual TestResult EndRun()
		{
			return this.testRunner.EndRun();
		}

		public virtual void CancelRun()
		{
			this.testRunner.CancelRun();
		}

		public virtual void Wait()
		{
			this.testRunner.Wait();
		}
		#endregion

		#region InitializeLifetimeService Override
		public override object InitializeLifetimeService()
		{
			return null;
		}
		#endregion

	}
}
