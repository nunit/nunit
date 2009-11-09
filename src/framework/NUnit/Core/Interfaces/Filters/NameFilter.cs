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

namespace NUnit.Core.Filters
{
	/// <summary>
	/// Summary description for NameFilter.
	/// </summary>
	/// 
	[Serializable]
	public class NameFilter : TestFilter
	{
		private ArrayList testNames = new ArrayList();

		/// <summary>
		/// Construct an empty NameFilter
		/// </summary>
		public NameFilter() { }

		/// <summary>
		/// Construct a NameFilter for a single TestName
		/// </summary>
		/// <param name="testName"></param>
		public NameFilter( TestName testName )
		{
			testNames.Add( testName );
		}

		/// <summary>
		/// Add a TestName to a NameFilter
		/// </summary>
		/// <param name="testName"></param>
		public void Add( TestName testName )
		{
			testNames.Add( testName );
		}

		/// <summary>
		/// Check if a test matches the filter
		/// </summary>
		/// <param name="test">The test to match</param>
		/// <returns>True if it matches, false if not</returns>
		public override bool Match( ITest test )
		{
			foreach( TestName testName in testNames )
				if ( test.TestName == testName )
					return true;

			return false;
		}
	}
}
