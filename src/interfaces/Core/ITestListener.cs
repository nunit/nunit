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

	/// <summary>
	/// The EventListener interface is used within the NUnit core to receive 
	/// notifications of significant events while a test is being run. These
	/// events are propogated to any client, which may choose to convert them
	/// to .NET events or to use them directly.
	/// </summary>
	public interface ITestListener
	{
		/// <summary>
		/// Called when a test run is starting
		/// </summary>
        /// <param name="testName">The TestName of the test being started</param>
        /// <param name="testCount">The number of test cases to be executed</param>
        void RunStarted(TestName testName, int testCount);

		/// <summary>
		/// Called when a run finishes normally
		/// </summary>
		/// <param name="result">The result of the test</param>
		void RunFinished( TestResult result );

		/// <summary>
		/// Called when a run is terminated due to an exception
		/// </summary>
		/// <param name="exception">Exception that was thrown</param>
		void RunFinished( Exception exception );

		/// <summary>
		/// Called when a test case is starting
		/// </summary>
		/// <param name="testName">The name of the test case</param>
		void TestStarted(TestName testName);
			
		/// <summary>
		/// Called when a test case has finished
		/// </summary>
		/// <param name="result">The result of the test</param>
		void TestFinished(TestResult result);

		/// <summary>
		/// Called when a suite is starting
		/// </summary>
		/// <param name="testName">The name of the suite</param>
		void SuiteStarted(TestName testName);

		/// <summary>
		/// Called when a suite has finished
		/// </summary>
		/// <param name="result">The result of the suite</param>
		void SuiteFinished(TestResult result);

		/// <summary>
		/// Called when an unhandled exception is detected during
		/// the execution of a test run.
		/// </summary>
		/// <param name="exception">The exception thta was detected</param>
		void UnhandledException( Exception exception );

		/// <summary>
		/// Called when the test direts output to the console.
		/// </summary>
		/// <param name="testOutput">A console message</param>
		void TestOutput(TestOutput testOutput);
	}
}
