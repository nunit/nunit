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

using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// TestListener provides an implementation of ITestListener that
    /// does nothing.
	/// </summary>
	/// 
	[Serializable]
	public class TestListener : ITestListener
	{
		public void RunStarted( TestName testName, int testCount ){ }

		public void RunFinished( TestResult result ) { }

		public void RunFinished( Exception exception ) { }

		public void TestStarted(TestName testName){}
			
		public void TestFinished(TestResult result){}

		public void SuiteStarted(TestName testName){}

		public void SuiteFinished(TestResult result){}

		public void UnhandledException( Exception exception ) {}

		public void TestOutput(TestOutput testOutput) {}

        private TestListener() { }

		public static ITestListener NULL
		{
			get { return new TestListener();}
		}
	}
}
