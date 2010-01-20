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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

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

		public void TestStarted(ITest test)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestStarted( test );
		}

		public void TestFinished(ITestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestFinished( result );
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
