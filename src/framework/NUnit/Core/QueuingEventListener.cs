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

namespace NUnit.Core
{
	/// <summary>
	/// QueuingEventListener uses an EventQueue to store any
	/// events received on its EventListener interface.
	/// </summary>
	public class QueuingEventListener : ITestListener
	{
		private EventQueue events = new EventQueue();

		/// <summary>
		/// The EvenQueue created and filled by this listener
		/// </summary>
		public EventQueue Events
		{
			get { return events; }
		}

		#region EventListener Methods
		/// <summary>
		/// Run is starting
		/// </summary>
        /// <param name="testName">The TestName of the test being started</param>
        /// <param name="testCount">The number of test cases to be executed</param>
        public void RunStarted(TestName testName, int testCount)
		{
			events.Enqueue( new RunStartedEvent( testName, testCount ) );
		}

		/// <summary>
		/// Run finished successfully
		/// </summary>
		/// <param name="results">Array of test results</param>
		public void RunFinished( TestResult result )
		{
			events.Enqueue( new RunFinishedEvent( result ) );
		}

		/// <summary>
		/// Run was terminated due to an exception
		/// </summary>
		/// <param name="exception">Exception that was thrown</param>
		public void RunFinished( Exception exception )
		{
			events.Enqueue( new RunFinishedEvent( exception ) );
		}

		/// <summary>
		/// A single test case is starting
		/// </summary>
		/// <param name="testCase">The test case</param>
		public void TestStarted(TestName testName)
		{
			events.Enqueue( new TestStartedEvent( testName ) );
		}

		/// <summary>
		/// A test case finished
		/// </summary>
		/// <param name="result">Result of the test case</param>
		public void TestFinished(TestResult result)
		{
			events.Enqueue( new TestFinishedEvent( result ) );
		}

		/// <summary>
		/// A suite is starting
		/// </summary>
		/// <param name="suite">The suite that is starting</param>
		public void SuiteStarted(TestName testName)
		{
			events.Enqueue( new SuiteStartedEvent( testName ) );
		}

		/// <summary>
		/// A suite finished
		/// </summary>
		/// <param name="result">Result of the suite</param>
		public void SuiteFinished(TestResult result)
		{
			events.Enqueue( new SuiteFinishedEvent( result ) );
		}

		/// <summary>
		/// An unhandled exception occured while running a test,
		/// but the test was not terminated.
		/// </summary>
		/// <param name="exception"></param>
		public void UnhandledException( Exception exception )
		{
			events.Enqueue( new UnhandledExceptionEvent( exception ) );
		}

		/// <summary>
		/// A message has been output to the console.
		/// </summary>
		/// <param name="testOutput">A console message</param>
		public void TestOutput( TestOutput output )
		{
			events.Enqueue( new OutputEvent( output ) );
		}
		#endregion
	}
}
