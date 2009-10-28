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

namespace NUnit.Core
{
	using System;
	using System.Threading;
	using System.Collections.Specialized;

	/// <summary>
	/// ThreadedTestRunner overrides the Run and BeginRun methods 
	/// so that they are always run on a separate thread. The actual
	/// </summary>
	public class ThreadedTestRunner : ProxyTestRunner
	{
        static Logger log = InternalTrace.GetLogger(typeof(ThreadedTestRunner));

		#region Instance Variables
		private TestRunnerThread testRunnerThread;
		#endregion

		#region Constructors
		public ThreadedTestRunner( TestRunner testRunner ) : base ( testRunner ) { }
		#endregion

		#region Overrides
		public override TestResult Run( ITestListener listener )
		{
			BeginRun( listener );
			return EndRun();
		}

		public override TestResult Run( ITestListener listener, TestFilter filter )
		{
			BeginRun( listener, filter );
			return EndRun();
		}

		public override void BeginRun( ITestListener listener )
		{
            log.Info("BeginRun");
   			testRunnerThread = new TestRunnerThread( this.TestRunner );
            testRunnerThread.StartRun( listener );
		}

		public override void BeginRun( ITestListener listener, TestFilter filter )
		{
            log.Info("BeginRun");
            testRunnerThread = new TestRunnerThread(this.TestRunner);
			testRunnerThread.StartRun( listener, filter );
		}

		public override TestResult EndRun()
		{
            log.Info("EndRun");
            this.Wait();
			return this.TestRunner.TestResult;
		}


		public override void Wait()
		{
			if ( testRunnerThread != null )
				testRunnerThread.Wait();
		}

		public override void CancelRun()
		{
			if ( testRunnerThread != null )
				testRunnerThread.Cancel();
		}

		#endregion
	}
}
