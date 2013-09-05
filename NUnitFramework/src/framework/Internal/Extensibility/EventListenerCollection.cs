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

#if !NUNITLITE
using System;
using System.Collections;
using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Internal.Extensibility
{
	/// <summary>
	/// EventListenerCollection holds multiple event listeners
	/// and relays all event calls to each of them.
	/// </summary>
	public class EventListenerCollection : ExtensionPoint, ITestListener
	{
		#region 

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListenerCollection"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
		public EventListenerCollection( IExtensionHost host )
			: base( "EventListeners", host ) { }
		#endregion

		#region EventListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
		public void TestStarted(ITest test)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestStarted( test );
		}

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
		public void TestFinished(ITestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestFinished( result );
		}

        /// <summary>
        /// Called when the test creates text output.
        /// </summary>
        /// <param name="testOutput">A console message</param>
		public void TestOutput(TestOutput testOutput)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestOutput( testOutput );
		}

		#endregion

		#region ExtensionPoint Overrides

        /// <summary>
        /// Determines whether [is valid extension] [the specified extension].
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid extension] [the specified extension]; otherwise, <c>false</c>.
        /// </returns>
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestListener; 
		}
		#endregion
	}
}
#endif
