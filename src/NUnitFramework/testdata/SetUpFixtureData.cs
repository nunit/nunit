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
using NUnit.Framework.Constraints;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// A static helper to Verify that Setup/Teardown 'events' occur, and that they are in the correct order...
    /// </summary>
    public static class SimpleEventRecorder
    {
        /// <summary>
        /// A helper class for matching test events.
        /// </summary>
        public class EventMatcher
        {
            private readonly List<string> _expectedEvents;

            public EventMatcher(IEnumerable<string> expectedEvents)
            {
                _expectedEvents = new List<string>(expectedEvents);
            }

            /// <summary>
            /// Matches a test event with one of the expected events.
            /// </summary>
            /// <param name="event">A string identifying the test event</param>
            /// <param name="item">The index of the recorded test event</param>
            /// <returns>true, if there are expected events left to match, otherwise false.</returns>
            public bool MatchEvent(string @event, int item)
            {
                Assert.Contains(@event, _expectedEvents, "Item {0}", item);
                _expectedEvents.Remove(@event);
                return _expectedEvents.Count > 0;
            }
        }

        /// <summary>
        /// Helper class for recording expected events.
        /// </summary>
        public class ExpectedEventsRecorder
        {
            private readonly Queue<string> _actualEvents;
            private readonly Queue<EventMatcher> _eventMatchers;

            public ExpectedEventsRecorder(Queue<string> actualEvents, params string[] expectedEvents)
            {
                _actualEvents = actualEvents;
                _eventMatchers = new Queue<EventMatcher>();
                AndThen(expectedEvents);
            }

            /// <summary>
            /// Adds the specified events as expected events.
            /// </summary>
            /// <param name="expectedEvents">An array of strings identifying the test events</param>
            /// <returns>Returns the ExpectedEventsRecorder for adding new expected events.</returns>
            public ExpectedEventsRecorder AndThen(params string[] expectedEvents)
            {
                _eventMatchers.Enqueue(new EventMatcher(expectedEvents));
                return this;
            }

            /// <summary>
            /// Verifies the recorded expected events with the actual recorded events.
            /// </summary>
            public void Verify()
            {
                EventMatcher eventMatcher = _eventMatchers.Dequeue();
                int item = 0;

                foreach (string actualEvent in _actualEvents)
                {
                    if (eventMatcher == null)
                    { 
                        Assert.Fail(
                            "More events than expected were recorded. Current event: {0} (Item {1})",
                            actualEvent,
                            item);
                    }

                    if (!eventMatcher.MatchEvent(actualEvent, item++))
                    {
                        if (_eventMatchers.Count > 0)
                            eventMatcher = _eventMatchers.Dequeue();
                        else
                            eventMatcher = null;
                    }
                }
            }
        }

        // Because it is static, this class can only be used by one fixture at a time.
        // Currently, only one fixture uses it, if more use it, they should not be run in parallel.
        // TODO: Create a utility that can be used by multiple fixtures

        private static Queue<string> _events;

        /// <summary>
        /// Initializes the <see cref="SimpleEventRecorder"/> 'static' class.
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
                int item = 0;
                string actual = _events.Count > 0 ? _events.Dequeue() : null;
                Assert.AreEqual( expected, actual, "Item {0}", item++ );
            }
        }

        /// <summary>
        /// Record the specified events as recorded expected events.
        /// </summary>
        /// <param name="expectedEvents">An array of strings identifying the test events</param>
        /// <returns>An ExpectedEventsRecorder so that further expected events can be recorded and verified</returns>
        public static ExpectedEventsRecorder ExpectEvents(params string[] expectedEvents)
        {
            return new ExpectedEventsRecorder(_events, expectedEvents);
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
        #region SomeFixture
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.Fixture.TearDown");
            }
        }
        #endregion SomeFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture1
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.OneTimeSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS1.OneTimeTearDown");
            }
        }
    }

    namespace Namespace2
    {

        #region Fixtures
        /// <summary>
        /// Summary description for SetUpFixtureTests.
        /// </summary>
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Fixture.TearDown");
            }
        }

        [TestFixture]
        public class AnotherFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.Fixture.TearDown");
            }
        }
        #endregion

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture2
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS2.OneTimeTearDown");
            }
        }
    }

    namespace Namespace3
    {
        namespace SubNamespace
        {


            #region SomeFixture
            [TestFixture]
            public class SomeFixture
            {
                [OneTimeSetUp]
                public void FixtureSetup()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Fixture.SetUp");
                }

                [SetUp]
                public void Setup()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test.SetUp");
                }

                [Test]
                public void Test()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test");
                }

                [TearDown]
                public void TearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test.TearDown");
                }

                [OneTimeTearDown]
                public void FixtureTearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Fixture.TearDown");
                }
            }
            #endregion SomeTestFixture

            [SetUpFixture]
            public class NUnitNamespaceSetUpFixture
            {
                [OneTimeSetUp]
                public void DoNamespaceSetUp()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.OneTimeSetUp");
                }

                [OneTimeTearDown]
                public void DoNamespaceTearDown()
                {
                    TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.OneTimeTearDown");
                }
            }

        }


        #region SomeFixture
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS3.OneTimeTearDown");
            }
        }
    }

    namespace Namespace4
    {
        #region SomeFixture
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.OneTimeSetUp1");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.OneTimeTearDown1");
            }
        }

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture2
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.OneTimeSetUp2");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS4.OneTimeTearDown2");
            }
        }
    }

    namespace Namespace5
    {
        #region SomeFixture
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.Test");
            }

            [TearDown]
            public void TearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public static void DoNamespaceTearDown()
            {
                TestUtilities.SimpleEventRecorder.RegisterEvent("NS5.OneTimeTearDown");
            }
        }
    }

    namespace Namespace6
    {
        [SetUpFixture]
        public class InvalidSetUpFixture
        {
            [SetUp]
            public void InvalidForOneTimeSetUp()
            {
            }
        }

        [TestFixture]
        public class SomeFixture
        {
            [Test]
            public void Test() { }
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
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("Assembly.OneTimeSetUp");
    }

    [OneTimeTearDown]
    public void DoNamespaceTearDown()
    {
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("Assembly.OneTimeTearDown");
    }
}

[TestFixture]
public class SomeFixture
{
    [Test]
    public void Test()
    {
        NUnit.TestUtilities.SimpleEventRecorder.RegisterEvent("NoNamespaceTest");
    }
}
#endregion
