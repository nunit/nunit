// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Threading;
using System.Globalization;
using NUnit.Framework;
#if !NETCF
using System.Security.Principal;
#endif
#if !NUNITLITE
using NUnit.TestData.TestContextData;
using NUnit.TestUtilities;
#endif

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Summary description for TestExecutionContextTests.
	/// </summary>
	[TestFixture]
	public class TestExecutionContextTests
	{
		string currentDirectory;
		CultureInfo currentCulture;
        CultureInfo currentUICulture;
#if !NETCF
        IPrincipal currentPrincipal;
#endif

		/// <summary>
		/// Since we are testing the mechanism that saves and
		/// restores contexts, we save manually here
		/// </summary>
		[SetUp]
		public void SaveContext()
		{
			currentCulture = CultureInfo.CurrentCulture;
            currentUICulture = CultureInfo.CurrentUICulture;
#if !NETCF
			currentDirectory = Environment.CurrentDirectory;
            currentPrincipal = Thread.CurrentPrincipal;
#endif
		}

		[TearDown]
		public void RestoreContext()
		{
#if !NETCF
			Environment.CurrentDirectory = currentDirectory;
			Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;
            Thread.CurrentPrincipal = currentPrincipal;
#endif
		}

        [Test]
        public void TestCanAccessItsOwnName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Name, Is.EqualTo("TestCanAccessItsOwnName"));
        }

        [Test]
        public void TestCanAccessItsOwnFullName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.TestCanAccessItsOwnFullName"));
        }

        [Test]
        public void TestCanAccessItsOwnId()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Id, Is.GreaterThan(0));
        }

        [Test]
        [Property("Answer", 42)]
        public void TestCanAccessItsOwnProperties()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
        }

#if !NUNITLITE
		[Test]
		public void SetAndRestoreCurrentDirectory()
		{
			Assert.AreEqual( currentDirectory, TestExecutionContext.CurrentContext.CurrentDirectory, "Directory not in initial context" );
			
			TestExecutionContext.Save();

            try
            {
                string otherDirectory = System.IO.Path.GetTempPath();
                if (otherDirectory[otherDirectory.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    otherDirectory = otherDirectory.Substring(0, otherDirectory.Length - 1);
                TestExecutionContext.CurrentContext.CurrentDirectory = otherDirectory;
                Assert.AreEqual(otherDirectory, Environment.CurrentDirectory, "Directory was not set");
                Assert.AreEqual(otherDirectory, TestExecutionContext.CurrentContext.CurrentDirectory, "Directory not in new context");
            }
            finally
            {
                TestExecutionContext.Restore();
            }

			Assert.AreEqual( currentDirectory, Environment.CurrentDirectory, "Directory was not restored" );
			Assert.AreEqual( currentDirectory, TestExecutionContext.CurrentContext.CurrentDirectory, "Directory not in final context" );
		}

		[Test]
		public void SetAndRestoreCurrentCulture()
		{
			Assert.AreEqual( currentCulture, TestExecutionContext.CurrentContext.CurrentCulture, "Culture not in initial context" );

            TestExecutionContext.Save();

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(currentCulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                TestExecutionContext.CurrentContext.CurrentCulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentCulture, "Culture was not set");
                Assert.AreEqual(otherCulture, TestExecutionContext.CurrentContext.CurrentCulture, "Culture not in new context");
            }
            finally
            {
                TestExecutionContext.Restore();
            }
            
            Assert.AreEqual(currentCulture, CultureInfo.CurrentCulture, "Culture was not restored");
            Assert.AreEqual(currentCulture, TestExecutionContext.CurrentContext.CurrentCulture, "Culture not in final context");
		}

        [Test]
        public void SetAndRestoreCurrentUICulture()
        {
            Assert.AreEqual(currentUICulture, TestExecutionContext.CurrentContext.CurrentUICulture, "UICulture not in initial context");

            TestExecutionContext.Save();

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(currentUICulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                TestExecutionContext.CurrentContext.CurrentUICulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentUICulture, "UICulture was not set");
                Assert.AreEqual(otherCulture, TestExecutionContext.CurrentContext.CurrentUICulture, "UICulture not in new context");
            }
            finally
            {
                TestExecutionContext.Restore();
            }

            Assert.AreEqual(currentUICulture, CultureInfo.CurrentUICulture, "UICulture was not restored");
            Assert.AreEqual(currentUICulture, TestExecutionContext.CurrentContext.CurrentUICulture, "UICulture not in final context");
        }

        [Test]
        public void SetAndRestoreCurrentPrincipal()
        {
            Assert.AreEqual(currentPrincipal, TestExecutionContext.CurrentContext.CurrentPrincipal, "Principal not in initial context");

            TestExecutionContext.Save();

            try
            {
                GenericIdentity identity = new GenericIdentity("foo");
                TestExecutionContext.CurrentContext.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);
                Assert.AreEqual("foo", Thread.CurrentPrincipal.Identity.Name, "Principal was not set");
                Assert.AreEqual("foo", TestExecutionContext.CurrentContext.CurrentPrincipal.Identity.Name, "Principal not in new context");
            }
            finally
            {
                TestExecutionContext.Restore();
            }

            Assert.AreEqual(currentPrincipal, Thread.CurrentPrincipal, "Principal was not restored");
            Assert.AreEqual(currentPrincipal, TestExecutionContext.CurrentContext.CurrentPrincipal, "Principal not in final context");
        }
#endif
    }
}
