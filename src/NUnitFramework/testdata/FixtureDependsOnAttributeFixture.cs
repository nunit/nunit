// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class FixtureDependencyEvents
    {
        private static readonly object SyncRoot = new();

        public static List<string> Events { get; } = [];

        public static void Reset()
        {
            lock (SyncRoot)
                Events.Clear();
        }

        public static void Record(string value)
        {
            lock (SyncRoot)
                Events.Add(value);
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
    [DependsOn(typeof(FixtureDependencyBeforeFailing), RequiresSuccess = false)]
    public class FixtureDependencyAfterFailingAllowed
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfterFailingAllowed) + ".OneTimeSetUp");
        }

        [Test]
        public void AfterFailingDependencyAllowedTest()
        {
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Events.Add(nameof(FixtureDependencyAfterFailingAllowed) + ".OneTimeTearDown");
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

    [TestFixture]
    public class FixtureDependencyOrderBefore
    {
        [Test]
        public void Before()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyOrderBefore))]
    [Order(1)]
    public class FixtureDependencyOrderAfter
    {
        [Test]
        public void After()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [Order(2)]
    public class FixtureDependencyOrderTarget
    {
        [Test]
        public void Target()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyOrderTarget))]
    public class FixtureDependencyOrderReferrer
    {
        [Test]
        public void Referrer()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FixtureDependencyParallelBeforeSlow
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Record(nameof(FixtureDependencyParallelBeforeSlow) + ".OneTimeSetUp");
        }

        [Test]
        public void BeforeSlowTest()
        {
            Thread.Sleep(150);
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            FixtureDependencyEvents.Record(nameof(FixtureDependencyParallelBeforeSlow) + ".OneTimeTearDown");
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [DependsOn(typeof(FixtureDependencyParallelBeforeSlow))]
    public class FixtureDependencyParallelAfter
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Record(nameof(FixtureDependencyParallelAfter) + ".OneTimeSetUp");
        }

        [Test]
        public void AfterParallelDependency()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FixtureDependencyParallelIndependent
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Record(nameof(FixtureDependencyParallelIndependent) + ".OneTimeSetUp");
        }

        [Test]
        public void IndependentTest()
        {
            Assert.Pass();
        }
    }
}
