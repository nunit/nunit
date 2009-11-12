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
using System.IO;
using NUnit.Framework;
using NUnit.Core;
using NUnit.TestData.LegacySuiteData;
using NUnit.TestUtilities;

namespace NUnit.Core.Tests
{
	[TestFixture]
	public class SuiteBuilderTests
	{
		private string testsDll = "nunit.framework.tests.dll";
		private string testData = "test-assembly.dll";
		private string tempFile = "x.dll";
		private TestSuiteBuilder builder;

        [TestFixtureSetUp]
        public void InitializeBuilders()
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();
        }

		[SetUp]
		public void CreateBuilder()
		{
			builder = new TestSuiteBuilder();
		}
		[TearDown]
		public void TearDown()
		{
			FileInfo info = new FileInfo(tempFile);
			if(info.Exists) info.Delete();
		}

		[Test]
		public void LoadAssembly() 
		{
			Test suite = builder.Build( new TestPackage( testsDll ) );
			Assert.IsNotNull(suite, "Unable to build suite" );
			Assert.AreEqual( 1, suite.Tests.Count );
			Assert.AreEqual( "NUnit", ((ITest)suite.Tests[0]).TestName.Name );
		}

		[Test]
		public void LoadAssemblyWithoutNamespaces()
		{
			TestPackage package = new TestPackage( testsDll );
			package.Settings["AutoNamespaceSuites"] = false;
			Test suite = builder.Build( package );
			Assert.IsNotNull(suite, "Unable to build suite" );
			Assert.Greater( suite.Tests.Count, 1 );
			Assert.AreEqual( "NUnitTestFixture", ((ITest)suite.Tests[0]).TestType );
		}

		[Test]
		public void LoadFixture()
		{
			TestPackage package = new TestPackage( testsDll );
			package.TestName = this.GetType().FullName;
			Test suite= builder.Build( package );
			Assert.IsNotNull(suite, "Unable to build suite");
		}

		[Test]
		public void LoadSuite()
		{
			TestPackage package = new TestPackage( testData );
			package.TestName = typeof(LegacySuiteReturningFixtures).FullName;
			Test suite= builder.Build( package );
			Assert.IsNotNull(suite, "Unable to build suite");
            Assert.AreEqual(3, suite.Tests.Count);
            Assert.AreEqual(5, suite.TestCount);
        }

		[Test]
		public void LoadNamespaceAsSuite()
		{
			TestPackage package = new TestPackage( testsDll );
			package.TestName = "NUnit.Core.Tests";
			Test suite= builder.Build( package );
			Assert.IsNotNull( suite );
			Assert.AreEqual( testsDll, suite.TestName.Name );
			Assert.AreEqual( "NUnit", ((Test)suite.Tests[0]).TestName.Name );
		}

		[Test]
		public void DiscoverSuite()
		{
			TestPackage package = new TestPackage( testData );
			package.TestName = typeof(NUnit.TestData.LegacySuiteData.EmptySuite).FullName;
			Test suite = builder.Build( package );
			Assert.IsNotNull(suite, "Could not discover suite attribute");
		}

		[Test]
		public void WrongReturnTypeSuite()
		{
			TestPackage package = new TestPackage( testData );
			package.TestName = typeof(LegacySuiteWithInvalidPropertyType).FullName;
			Test suite = builder.Build( package );
			Assert.AreEqual(RunState.NotRunnable, suite.RunState);
		}

		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void FileNotFound()
		{
			builder.Build( new TestPackage( "xxxx.dll" ) );
		}

		// Gives FileNotFoundException on Mono
		[Test, ExpectedException(typeof(BadImageFormatException))]
		public void InvalidAssembly()
		{
			FileInfo file = new FileInfo( tempFile );

			StreamWriter sw = file.AppendText();

			sw.WriteLine("This is a new entry to add to the file");
			sw.WriteLine("This is yet another line to add...");
			sw.Flush();
			sw.Close();

			builder.Build( new TestPackage( tempFile ) );
		}

		[Test]
		public void FixtureNotFound()
		{
			TestPackage package = new TestPackage( testsDll );
			package.TestName = "NUnit.Tests.Junk";
			Assert.IsNull( builder.Build( package ) );
		}
	}
}
