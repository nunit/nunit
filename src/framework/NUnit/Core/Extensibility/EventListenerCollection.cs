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
using System.Collections;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// EventListenerCollection holds multiple event listeners
	/// and relays all event calls to each of them.
	/// </summary>
	public class EventListenerCollection : ExtensionPoint, ITestListener
	{
		#region Constructor
		public EventListenerCollection( IExtensionHost host )
			: base( "EventListeners", host ) { }
		#endregion

		#region EventListener Members
		public void RunStarted(TestName testName, int testCount)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunStarted( testName, testCount );
		}

		public void RunFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunFinished( result );
		}

		public void RunFinished(Exception exception)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunFinished( exception );
		}

		public void SuiteStarted(TestName testName)
		{
			foreach( ITestListener listener in Extensions )
				listener.SuiteStarted( testName );
		}

		public void SuiteFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.SuiteFinished( result );
		}

		public void TestStarted(TestName testName)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestStarted( testName );
		}

		public void TestFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestFinished( result );
		}

		public void UnhandledException(Exception exception)
		{
			foreach( ITestListener listener in Extensions )
				listener.UnhandledException( exception );
		}

		public void TestOutput(TestOutput testOutput)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestOutput( testOutput );
		}

		#endregion

		#region ExtensionPoint Overrides
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestListener; 
		}
		#endregion
	}
}
