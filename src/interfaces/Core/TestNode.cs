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

namespace NUnit.Core
{
	/// <summary>
	/// TestNode represents a single test or suite in the test hierarchy.
	/// TestNode holds common info needed about a test and represents a
	/// single node - either a test or a suite - in the hierarchy of tests.
	/// 
	/// TestNode extends TestInfo, which holds all the information with
	/// the exception of the list of child classes. When constructed from
	/// a Test, TestNodes are always fully populated with child TestNodes.
	/// 
	/// Like TestInfo, TestNode is purely a data class, and is not able
	/// to execute tests.
	/// 
	/// </summary>
	[Serializable]
	public class TestNode : TestInfo
	{
		#region Instance Variables
		private ITest parent;

		/// <summary>
		/// For a test suite, the child tests or suites
		/// Null if this is not a test suite
		/// </summary>
		private ArrayList tests;
		#endregion

		#region Constructors
		/// <summary>
		/// Construct from an ITest
		/// </summary>
		/// <param name="test">Test from which a TestNode is to be constructed</param>
		public TestNode ( ITest test ) : base( test )
		{
			if ( test.IsSuite )
			{
				this.tests = new ArrayList();
				
				foreach( ITest child in test.Tests )
				{
					TestNode node = new TestNode( child );
					this.Tests.Add( node );
					node.parent = this;
				}
			}
		}

        /// <summary>
        /// Construct a TestNode given a TestName and an
        /// array of child tests.
        /// </summary>
        /// <param name="testName">The TestName of the new test</param>
        /// <param name="tests">An array of tests to be added as children of the new test</param>
	    public TestNode ( TestName testName, ITest[] tests ) : base( testName, tests )
		{
			this.tests = new ArrayList();
			this.tests.AddRange( tests );
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the parent test of the current test
        /// </summary>
		public override ITest Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Array of child tests, null if this is a test case.
		/// </summary>
		public override IList Tests 
		{
			get { return tests; }
		}
		#endregion
	}
}
