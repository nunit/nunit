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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// A helper to Verify that Setup/Teardown 'events' occur, and that they are in the correct order...
    /// </summary>
    public class SimpleEventRecorder
    {
        private static Queue<string> _events;

        /// <summary>
        /// Initializes the <see cref="T:EventRegistrar"/> 'static' class.
        /// </summary>
        static SimpleEventRecorder()
        {
            _events = new Queue<string>();
        }

        /// <summary>
        /// Registers an event.
        /// </summary>
        /// <param name="evnt">The event to register.</param>
        public static void RegisterEvent(string evnt)
        {
            _events.Enqueue(evnt);
        }


        /// <summary>
        /// Verifies the specified expected events occurred and that they occurred in the specified order.
        /// </summary>
        /// <param name="expectedEvents">The expected events.</param>
        public static void Verify(params string[] expectedEvents)
        {
            foreach (string expected in expectedEvents)
            {
                string actual = _events.Count > 0 ? _events.Dequeue() as string : null;
                Assert.AreEqual( expected, actual );
            }
        }

        /// <summary>
        /// Clears any unverified events.
        /// </summary>
        public static void Clear()
        {
            _events.Clear();
        }
    }
}

namespace NUnit.TestData.SetupFixture
{
    namespace Namespace1
    {
        #region SomeTestFixture
        [TestFixture]
        public class SomeTestFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown");
            }
        }
    }

    namespace Namespace2
    {

        #region SomeTestFixture
        /// <summary>
        /// Summary description for SetUpFixtureTests.
        /// </summary>
        [TestFixture]
        public class SomeTestFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture

        #region SomeTestFixture2
        [TestFixture]
        public class SomeTestFixture2
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture2

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown");
            }
        }
    }

    namespace Namespace3
    {
        namespace SubNamespace
        {


            #region SomeTestFixture
            [TestFixture]
            public class SomeTestFixture
            {
                [OneTimeSetUp]
                public void FixtureSetup()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
                }

                [SetUp]
                public void Setup()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
                }

                [Test]
                public void Test()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
                }

                [TearDown]
                public void TearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
                }

                [OneTimeTearDown]
                public void FixtureTearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
                }
            }
            #endregion SomeTestFixture

            [SetUpFixture]
            public class NUnitNamespaceSetUpFixture
            {
                [OneTimeSetUp]
                public void DoNamespaceSetUp()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("SubNamespaceSetup");
                }

                [OneTimeTearDown]
                public void DoNamespaceTearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("SubNamespaceTearDown");
                }
            }

        }


        #region SomeTestFixture
        [TestFixture]
        public class SomeTestFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown");
            }
        }
    }

    namespace Namespace4
    {
        #region SomeTestFixture
        [TestFixture]
        public class SomeTestFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown");
            }
        }

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture2
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup2");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown2");
            }
        }
    }

#if !NETCF
    namespace Namespace5
    {
        [SetUpFixture]
        public class CurrentDirectoryRecordingSetUpFixture
        {
            [OneTimeSetUp]
            public void DoSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("SetUp:" + Environment.CurrentDirectory);
            }

            [OneTimeTearDown]
            public void DoTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown:" + Environment.CurrentDirectory);
            }
        }

        [TestFixture]
        public class SomeFixture
        {
            [Test]
            public void SomeMethod() { }				
        }
    }
#endif

    namespace Namespace5
    {
        #region SomeTestFixture
        [TestFixture]
        public class SomeTestFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureSetup");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Setup");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("FixtureTearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceSetup");
            }

            [OneTimeTearDown]
            public static void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NamespaceTearDown");
            }
        }
    }
}
#region NoNamespaceSetupFixture
[SetUpFixture]
public class NoNamespaceSetupFixture
{
    [OneTimeSetUp]
    public void DoNamespaceSetUp()
    {
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("RootNamespaceSetup");
    }

    [OneTimeTearDown]
    public void DoNamespaceTearDown()
    {
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("RootNamespaceTearDown");
    }
}

[TestFixture]
public class SomeTestFixture
{
    [Test]
    public void Test()
    {
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("Test");
    }
}
#endregion NoNamespaceSetupFixture
