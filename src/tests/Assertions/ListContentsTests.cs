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
using NUnit.Framework;

namespace NUnit.Framework.Assertions
{
	/// <summary>
	/// Summary description for ListContentsTests.
	/// </summary>
	[TestFixture]
	public class ListContentsTests : MessageChecker
	{
		private static readonly object[] testArray = { "abc", 123, "xyz" };

		[Test]
		public void ArraySucceeds()
		{
			Assert.Contains( "abc", testArray );
			Assert.Contains( 123, testArray );
			Assert.Contains( "xyz", testArray );
		}

		[Test,ExpectedException(typeof(AssertionException))]
		public void ArrayFails()
		{
			expectedMessage =
				"  Expected: collection containing \"def\"" + Environment.NewLine + 
				"  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;	
			Assert.Contains("def", testArray);
		}

		[Test,ExpectedException(typeof(AssertionException))]
		public void EmptyArrayFails()
		{
			expectedMessage =
				"  Expected: collection containing \"def\"" + Environment.NewLine + 
				"  But was:  <empty>" + Environment.NewLine;	
			Assert.Contains( "def", new object[0] );
		}

		[Test,ExpectedException(typeof(ArgumentException))]
		public void NullArrayIsError()
		{
			Assert.Contains( "def", null );
		}

		[Test]
		public void ArrayListSucceeds()
		{
			ArrayList list = new ArrayList( testArray );

			Assert.Contains( "abc", list );
			Assert.Contains( 123, list );
			Assert.Contains( "xyz", list );
		}

		[Test,ExpectedException(typeof(AssertionException))]
		public void ArrayListFails()
		{
			expectedMessage =
				"  Expected: collection containing \"def\"" + Environment.NewLine + 
				"  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
			Assert.Contains( "def", new ArrayList( testArray ) );
		}

		[Test]
		public void DifferentTypesMayBeEqual()
		{
			// TODO: Better message for this case
			expectedMessage =
				"  Expected: collection containing 123.0d" + Environment.NewLine + 
				"  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
			Assert.Contains( 123.0, new ArrayList( testArray ) );
		}
	}
}
#endif
