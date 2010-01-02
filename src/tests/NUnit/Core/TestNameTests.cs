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
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Tests
{
	[TestFixture]
	public class TestNameTests
	{
		private TestName weakName1;
		private TestName weakName2;
		private TestName strongName1;
		private TestName strongName2;

		[SetUp]
		public void CreateTestNames()
		{
			weakName1 = new TestName();
			weakName2 = new TestName();
			weakName1.FullName = weakName2.FullName = "Name.Of.This.Test";

			strongName1 = new TestName();
			strongName2 = new TestName();
			strongName1.FullName = strongName2.FullName = "Name.Of.This.Test";
			strongName1.TestID = new TestID();
			strongName2.TestID = new TestID();
		}

		[Test]
		public void CanCompareWeakTestNames()
		{
			Assert.AreEqual( weakName1, weakName2 );
			Assert.IsTrue( weakName1 == weakName2, "operator ==");
			Assert.IsFalse( weakName1 != weakName2, "operator !=");

			weakName2.FullName = "A.Different.Name";
			Assert.AreNotEqual( weakName1, weakName2 );
			Assert.IsFalse( weakName1 == weakName2, "operator ==");
			Assert.IsTrue( weakName1 != weakName2, "operator !=");
		}

		[Test]
		public void CanCompareStrongTestNames()
		{
			Assert.AreNotEqual( strongName1, strongName2 );
			Assert.IsFalse( strongName1 == strongName2, "operator ==" );
			Assert.IsTrue( strongName1 != strongName2, "operator !=" );

			strongName2.TestID = strongName1.TestID;
			Assert.AreEqual( strongName1, strongName2 );
			Assert.IsTrue( strongName1 == strongName2, "operator ==" );
			Assert.IsFalse( strongName1 != strongName2, "operator !=" );

			strongName2.FullName = "A.Different.Name";
			Assert.AreNotEqual( strongName1, strongName2 );
			Assert.IsFalse( strongName1 == strongName2, "operator ==" );
			Assert.IsTrue( strongName1 != strongName2, "operator !=" );
		}

		[Test]
		public void CanCompareWeakAndStrongTestNames()
		{
			Assert.AreNotEqual( weakName1, strongName1 );
			Assert.IsFalse( weakName1 == strongName1, "operator ==" );
			Assert.IsTrue( weakName1 != strongName1, "operator !=" );
		}

		[Test]
		public void TestNamesWithDifferentRunnerIDsAreNotEqual()
		{
			weakName2.RunnerID = 7;
			Assert.AreEqual( 0, weakName1.RunnerID );
			Assert.AreNotEqual( weakName1, weakName2 );
			Assert.IsFalse( weakName1 == weakName2, "operator ==" );
			Assert.IsTrue( weakName1 != weakName2, "operator !=" );

			strongName1.RunnerID = 3;
			strongName2.RunnerID = 5;
			strongName2.TestID = strongName1.TestID;
			Assert.AreNotEqual( strongName1, strongName2 );
			Assert.IsFalse( strongName1 == strongName2, "operator ==" );
			Assert.IsTrue( strongName1 != strongName2, "operator !=" );
		}

		[Test]
		public void ClonedTestNamesAreEqual()
		{
			TestName clonedName = (TestName)weakName1.Clone();
			Assert.AreEqual( weakName1, clonedName );
			Assert.IsTrue( weakName1 == clonedName, "operator ==" );
			Assert.IsFalse( weakName1 != clonedName, "operator !=" );

			clonedName = (TestName)strongName1.Clone();
			Assert.AreEqual( strongName1, clonedName );
			Assert.IsTrue( strongName1 == clonedName, "operator ==" );
			Assert.IsFalse( strongName1 != clonedName, "operator !=" );
		}

		[Test]
		public void CanDisplayUniqueNames()
		{
			Assert.AreEqual( "[0]Name.Of.This.Test", weakName1.UniqueName );
			Assert.AreEqual( "[0-" + strongName1.TestID.ToString() + "]Name.Of.This.Test", strongName1.UniqueName );
		}

		[Test]
		public void CanParseSimpleTestNames()
		{
			TestName tn = TestName.Parse( "Name.Of.This.Test" );
			Assert.AreEqual( "Name.Of.This.Test", tn.FullName );
		}

		[Test]
		public void CanParseWeakTestNames()
		{
			TestName testName = TestName.Parse( weakName1.UniqueName );
			Assert.AreEqual( weakName1, testName );

			weakName1.RunnerID = 7;
			testName = TestName.Parse( weakName1.UniqueName );
			Assert.AreEqual( weakName1, testName );
		}

		[Test]
		public void CanParseStrongTestNames()
		{
			TestName testName = TestName.Parse( strongName1.UniqueName );
			Assert.AreEqual( strongName1, testName );

			strongName1.RunnerID = 7;
			testName = TestName.Parse( strongName1.UniqueName );
			Assert.AreEqual( strongName1, testName );
		}
	}
}
