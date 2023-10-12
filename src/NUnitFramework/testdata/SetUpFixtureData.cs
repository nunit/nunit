// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.TestUtilities
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
                Assert.That(_expectedEvents, Does.Contain(@event), $"Item {item}");
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
                    if (eventMatcher is null)
                    {
                        Assert.Fail($"More events than expected were recorded. Current event: {actualEvent} (Item {item})");
                    }

                    if (!eventMatcher.MatchEvent(actualEvent, item++))
                    {
                        eventMatcher = _eventMatchers.Count > 0 ? _eventMatchers.Dequeue() : null;
                    }
                }
            }
        }

        // Because it is static, this class can only be used by one fixture at a time.
        // Currently, only one fixture uses it, if more use it, they should not be run in parallel.
        // TODO: Create a utility that can be used by multiple fixtures

        private static readonly Queue<string> Events = new();

        /// <summary>
        /// Registers an event.
        /// </summary>
        /// <param name="evnt">The event to register.</param>
        public static void RegisterEvent(string evnt)
        {
            Events.Enqueue(evnt);
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
                string actual = Events.Count > 0 ? Events.Dequeue() : null;
                Assert.That(actual, Is.EqualTo(expected), $"Item {++item}");
            }
        }

        /// <summary>
        /// Record the specified events as recorded expected events.
        /// </summary>
        /// <param name="expectedEvents">An array of strings identifying the test events</param>
        /// <returns>An ExpectedEventsRecorder so that further expected events can be recorded and verified</returns>
        public static ExpectedEventsRecorder ExpectEvents(params string[] expectedEvents)
        {
            return new ExpectedEventsRecorder(Events, expectedEvents);
        }

        /// <summary>
        /// Clears any unverified events.
        /// </summary>
        public static void Clear()
        {
            Events.Clear();
        }
    }
}

namespace NUnit.TestData.SetupFixture
{
    namespace StaticFixture
    {
        #region SomeFixture
        [TestFixture]
        public class TestSetupFixtureStuff
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.Fixture.TearDown");
            }
        }
        #endregion SomeFixture

        [SetUpFixture]
        public static class StaticSetupTeardown
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public static void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("StaticFixture.OneTimeTearDown");
            }
        }
    }

    namespace Namespace1
    {
        #region SomeFixture
        [TestFixture]
        public class SomeFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                SimpleEventRecorder.RegisterEvent("NS1.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS1.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS1.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS1.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS1.Fixture.TearDown");
            }
        }
        #endregion SomeFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture1
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS1.OneTimeSetup");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS1.OneTimeTearDown");
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
                SimpleEventRecorder.RegisterEvent("NS2.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Fixture.TearDown");
            }
        }

        [TestFixture]
        public class AnotherFixture
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS2.Fixture.TearDown");
            }
        }
        #endregion

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture2
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS2.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS2.OneTimeTearDown");
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
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Fixture.SetUp");
                }

                [SetUp]
                public void Setup()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test.SetUp");
                }

                [Test]
                public void Test()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test");
                }

                [TearDown]
                public void TearDown()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Test.TearDown");
                }

                [OneTimeTearDown]
                public void FixtureTearDown()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.Fixture.TearDown");
                }
            }
            #endregion SomeTestFixture

            [SetUpFixture]
            public class NUnitNamespaceSetUpFixture
            {
                [OneTimeSetUp]
                public void DoNamespaceSetUp()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.OneTimeSetUp");
                }

                [OneTimeTearDown]
                public void DoNamespaceTearDown()
                {
                    SimpleEventRecorder.RegisterEvent("NS3.SubNamespace.OneTimeTearDown");
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
                SimpleEventRecorder.RegisterEvent("NS3.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS3.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS3.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS3.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS3.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS3.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS3.OneTimeTearDown");
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
                SimpleEventRecorder.RegisterEvent("NS4.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS4.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS4.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS4.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS4.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS4.OneTimeSetUp1");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS4.OneTimeTearDown1");
            }
        }

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture2
        {
            [OneTimeSetUp]
            public void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS4.OneTimeSetUp2");
            }

            [OneTimeTearDown]
            public void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS4.OneTimeTearDown2");
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
                SimpleEventRecorder.RegisterEvent("NS5.Fixture.SetUp");
            }

            [SetUp]
            public void Setup()
            {
                SimpleEventRecorder.RegisterEvent("NS5.Test.SetUp");
            }

            [Test]
            public void Test()
            {
                SimpleEventRecorder.RegisterEvent("NS5.Test");
            }

            [TearDown]
            public void TearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS5.Test.TearDown");
            }

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS5.Fixture.TearDown");
            }
        }
        #endregion SomeTestFixture

        [SetUpFixture]
        public class NUnitNamespaceSetUpFixture
        {
            [OneTimeSetUp]
            public static void DoNamespaceSetUp()
            {
                SimpleEventRecorder.RegisterEvent("NS5.OneTimeSetUp");
            }

            [OneTimeTearDown]
            public static void DoNamespaceTearDown()
            {
                SimpleEventRecorder.RegisterEvent("NS5.OneTimeTearDown");
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
            public void Test()
            {
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
        SimpleEventRecorder.RegisterEvent("Assembly.OneTimeSetUp");
    }

    [OneTimeTearDown]
    public void DoNamespaceTearDown()
    {
        SimpleEventRecorder.RegisterEvent("Assembly.OneTimeTearDown");
    }
}

[TestFixture]
public class SomeFixture
{
    [Test]
    public void Test()
    {
        SimpleEventRecorder.RegisterEvent("NoNamespaceTest");
    }
}
#endregion
