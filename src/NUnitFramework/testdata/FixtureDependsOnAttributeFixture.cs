// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class FixtureDependencyEvents
    {
        public static List<string> Events { get; } = new();

        public static void Reset()
        {
            Events.Clear();
        }
    }

    [TestFixture]
    public class FixtureDependencyBefore
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyBefore) + ".OneTimeSetUp");
        }

        [Test]
        public void BeforeTest()
        {
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyBefore) + ".OneTimeTearDown");
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBefore))]
    public class FixtureDependencyAfter
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfter) + ".OneTimeSetUp");
        }

        [Test]
        public void AfterTest()
        {
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfter) + ".OneTimeTearDown");
        }
    }

    [TestFixture]
    public class FixtureDependencyBeforeFailing
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyBeforeFailing) + ".OneTimeSetUp");
        }

        [Test]
        public void BeforeFailingTest()
        {
            Assert.Fail("Intentional failure for dependency behavior testing");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyBeforeFailing) + ".OneTimeTearDown");
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBeforeFailing))]
    public class FixtureDependencyAfterFailing
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfterFailing) + ".OneTimeSetUp");
        }

        [Test]
        public void AfterFailingDependencyTest()
        {
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfterFailing) + ".OneTimeTearDown");
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyCycleB))]
    public class FixtureDependencyCycleA
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyCycleA) + ".OneTimeSetUp");
        }

        [Test]
        public void A()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyCycleA))]
    public class FixtureDependencyCycleB
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyCycleB) + ".OneTimeSetUp");
        }

        [Test]
        public void B()
        {
            Assert.Pass();
        }
    }
}
