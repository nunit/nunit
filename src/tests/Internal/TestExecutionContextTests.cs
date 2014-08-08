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
    [TestFixture][Property("Question", "Why?")]
    public class TestExecutionContextTests
    {
        TestExecutionContext fixtureContext;
        TestExecutionContext setupContext;

#if !NETCF && !SILVERLIGHT
        string originalDirectory;
        IPrincipal originalPrincipal;
#endif

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            fixtureContext = TestExecutionContext.CurrentContext;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // TODO: We put some tests in one time teardown to verify that
            // the context is still valid. It would be better if these tests
            // were placed in a second-level test, invoked from this test class.
            TestExecutionContext ec = TestExecutionContext.CurrentContext;
            Assert.That(ec.CurrentTest.Name, Is.EqualTo("TestExecutionContextTests"));
            Assert.That(ec.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests"));
            Assert.That(fixtureContext.CurrentTest.Id, Is.GreaterThan(0));
            Assert.That(fixtureContext.CurrentTest.Properties.Get("Question"), Is.EqualTo("Why?"));
        }

        /// <summary>
        /// Since we are testing the mechanism that saves and
        /// restores contexts, we save manually here
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            setupContext = new TestExecutionContext(TestExecutionContext.CurrentContext);
#if !NETCF
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            originalDirectory = Environment.CurrentDirectory;
            originalPrincipal = Thread.CurrentPrincipal;
#endif
        }

        [TearDown]
        public void Cleanup()
        {
#if !NETCF
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            Environment.CurrentDirectory = originalDirectory;
            Thread.CurrentPrincipal = originalPrincipal;
#endif

            Assert.That(
                TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo(setupContext.CurrentTest.FullName),
                "Context at TearDown failed to match that saved from SetUp");
        }

        #region CurrentTest

        [Test]
        public void FixtureSetUpCanAccessFixtureName()
        {
            Assert.That(fixtureContext.CurrentTest.Name, Is.EqualTo("TestExecutionContextTests"));
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureFullName()
        {
            Assert.That(fixtureContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests"));
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureId()
        {
            Assert.That(fixtureContext.CurrentTest.Id, Is.GreaterThan(0));
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureProperties()
        {
            Assert.That(fixtureContext.CurrentTest.Properties.Get("Question"), Is.EqualTo("Why?"));
        }

        [Test]
        public void SetUpCanAccessTestName()
        {
            Assert.That(setupContext.CurrentTest.Name, Is.EqualTo("SetUpCanAccessTestName"));
        }

        [Test]
        public void SetUpCanAccessTestFullName()
        {
            Assert.That(setupContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.SetUpCanAccessTestFullName"));
        }

        [Test]
        public void SetUpCanAccessTestId()
        {
            Assert.That(setupContext.CurrentTest.Id, Is.GreaterThan(0));
        }

        [Test]
        [Property("Answer", 42)]
        public void SetUpCanAccessTestProperties()
        {
            Assert.That(setupContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
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

        #endregion

        #region CurrentCulture and CurrentUICulture

#if !NETCF
        CultureInfo originalCulture;
        CultureInfo originalUICulture;

        [Test]
        public void FixtureSetUpontextReflectsCurrentCulture()
        {
            Assert.That(fixtureContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test]
        public void FixtureSetUpContextReflectsCurrentUICulture()
        {
            Assert.That(fixtureContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
        }

        [Test]
        public void SetUpContextReflectsCurrentCulture()
        {
            Assert.That(setupContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test]
        public void SetUpContextReflectsCurrentUICulture()
        {
            Assert.That(setupContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));

        }

        [Test]
        public void TestContextReflectsCurrentCulture()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test]
        public void TestContextReflectsCurrentUICulture()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
        }

        [Test]
        public void SetAndRestoreCurrentCulture()
        {
            var context = new TestExecutionContext(setupContext);

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(originalCulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                context.CurrentCulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentCulture, "Culture was not set");
                Assert.AreEqual(otherCulture, context.CurrentCulture, "Culture not in new context");
                Assert.AreEqual(setupContext.CurrentCulture, originalCulture, "Original context should not change");
            }
            finally
            {
                setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(CultureInfo.CurrentCulture, originalCulture, "Culture was not restored");
            Assert.AreEqual(setupContext.CurrentCulture, originalCulture, "Culture not in final context");
        }

        [Test]
        public void SetAndRestoreCurrentUICulture()
        {
            var context = new TestExecutionContext(setupContext);

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(originalUICulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                context.CurrentUICulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentUICulture, "UICulture was not set");
                Assert.AreEqual(otherCulture, context.CurrentUICulture, "UICulture not in new context");
                Assert.AreEqual(setupContext.CurrentUICulture, originalUICulture, "Original context should not change");
            }
            finally
            {
                setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(CultureInfo.CurrentUICulture, originalUICulture, "UICulture was not restored");
            Assert.AreEqual(setupContext.CurrentUICulture, originalUICulture, "UICulture not in final context");
        }
#endif

        #endregion

        #region CurrentDirectory

#if !NETCF && !SILVERLIGHT
        [Test]
        public void FixtureSetUpContextReflectsCurrentDirectory()
        {
            Assert.That(fixtureContext.CurrentDirectory, Is.EqualTo(Environment.CurrentDirectory));
        }

        [Test]
        public void SetUpContextReflectsCurrentDirectory()
        {
            Assert.That(setupContext.CurrentDirectory, Is.EqualTo(Environment.CurrentDirectory));
        }

        [Test]
        public void TestContextReflectsCurrentDirectory()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentDirectory, Is.EqualTo(Environment.CurrentDirectory));
        }

        [Test]
        public void SetAndRestoreCurrentDirectory()
        {
            var context = new TestExecutionContext(setupContext);

            try
            {
                string otherDirectory = System.IO.Path.GetTempPath();
                if (otherDirectory[otherDirectory.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    otherDirectory = otherDirectory.Substring(0, otherDirectory.Length - 1);
                context.CurrentDirectory = otherDirectory;
                Assert.AreEqual(otherDirectory, Environment.CurrentDirectory, "Directory was not set");
                Assert.AreEqual(otherDirectory, context.CurrentDirectory, "Directory not in new context");
                Assert.AreEqual(setupContext.CurrentDirectory, originalDirectory, "Original context should not change");
            }
            finally
            {
                setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(Environment.CurrentDirectory, originalDirectory, "Directory was not restored");
            Assert.AreEqual(setupContext.CurrentDirectory, originalDirectory, "Directory not in final context");
        }
#endif

        #endregion

        #region CurrentPrincipal

#if !NETCF && !SILVERLIGHT
        [Test]
        public void FixtureSetUpContextReflectsCurrentPrincipal()
        {
            Assert.That(fixtureContext.CurrentPrincipal, Is.EqualTo(Thread.CurrentPrincipal));
        }

        [Test]
        public void SetUpContextReflectsCurrentPrincipal()
        {
            Assert.That(setupContext.CurrentPrincipal, Is.EqualTo(Thread.CurrentPrincipal));
        }

        [Test]
        public void TestContextReflectsCurrentPrincipal()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentPrincipal, Is.EqualTo(Thread.CurrentPrincipal));
        }

        [Test]
        public void SetAndRestoreCurrentPrincipal()
        {
            var context = new TestExecutionContext(setupContext);

            try
            {
                GenericIdentity identity = new GenericIdentity("foo");
                context.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);
                Assert.AreEqual("foo", Thread.CurrentPrincipal.Identity.Name, "Principal was not set");
                Assert.AreEqual("foo", context.CurrentPrincipal.Identity.Name, "Principal not in new context");
                Assert.AreEqual(setupContext.CurrentPrincipal, originalPrincipal, "Original context should not change");
            }
            finally
            {
                setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(Thread.CurrentPrincipal, originalPrincipal, "Principal was not restored");
            Assert.AreEqual(setupContext.CurrentPrincipal, originalPrincipal, "Principal not in final context");
        }
#endif

        #endregion

        #region ExecutionStatus

        [Test]
        public void ExecutionStatusIsPushedToHigherContext()
        {
            var topContext = new TestExecutionContext();
            var bottomContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            bottomContext.ExecutionStatus = TestExecutionStatus.StopRequested;

            Assert.That(topContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
        }

        [Test]
        public void ExecutionStatusIsPulledFromHigherContext()
        {
            var topContext = new TestExecutionContext();
            var bottomContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            topContext.ExecutionStatus = TestExecutionStatus.AbortRequested;

            Assert.That(bottomContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.AbortRequested));
        }

        [Test]
        public void ExecutionStatusIsPromulgatedAcrossBranches()
        {
            var topContext = new TestExecutionContext();
            var leftContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));
            var rightContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            leftContext.ExecutionStatus = TestExecutionStatus.StopRequested;

            Assert.That(rightContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
        }

        #endregion
    }
}
