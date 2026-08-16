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

    public class FixtureDependencyBase
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            FixtureDependencyEvents.Record($"{testName}.OneTimeSetUp");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            FixtureDependencyEvents.Record($"{testName}.OneTimeTearDown");
        }
    }

    #region Basic Functionality
    [TestFixture]
    public class FixtureDependencyBefore
    {
        [Test]
        public void BeforeTest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBefore))]
    public class FixtureDependencyAfter
    {
        [Test]
        public void AfterTest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class FixtureDependencyBeforeFailing : FixtureDependencyBase
    {
        [Test]
        public void BeforeFailingTest()
        {
            Assert.Fail("Intentional failure for dependency behavior testing");
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBeforeFailing))]
    public class FixtureDependencyAfterFailing : FixtureDependencyBase
    {
        [Test]
        public void AfterFailingDependencyTest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBeforeFailing), AllowFailure = true)]
    public class FixtureDependencyAfterFailingAllowed : FixtureDependencyBase
    {
        [Test]
        public void AfterFailingDependencyAllowedTest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class ForkingDependencyRoot : FixtureDependencyBase
    {
        [Test]
        public void RootTest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(ForkingDependencyRoot))]
    public class ForkingDependencyNodeA : FixtureDependencyBase
    {
        [Test]
        public void NodeATest()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(ForkingDependencyRoot))]
    public class ForkingDependencyNodeB : FixtureDependencyBase
    {
        [Test]
        public void NodeBTest()
        {
            Assert.That(true, Is.True);
        }
    }

    #endregion

    #region Referential Integrity
    [TestFixture]
    [DependsOn(typeof(FixtureDependencyCycleB))]
    public class FixtureDependencyCycleA
    {
        [Test]
        public void A()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyCycleA))]
    public class FixtureDependencyCycleB
    {
        [Test]
        public void B()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyCycleA))]
    public class FixtureDependencyCycleReferrer : FixtureDependencyBase
    {
        [Test]
        public void C()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencySelfReferential))]
    public class FixtureDependencySelfReferential
    {
        [Test]
        public void B()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyBase))]
    public class FixtureDependencySelfReferentialBase : FixtureDependencyBase
    {
        [Test]
        public void B()
        {
            Assert.That(true, Is.True);
        }
    }
    #endregion

    #region Feature Compatibility
    [TestFixture]
    public class FixtureDependencyOrderBefore
    {
        [Test]
        public void Before()
        {
            Assert.That(true, Is.True);
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
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [Order(2)]
    public class FixtureDependencyOrderTarget
    {
        [Test]
        public void Target()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(typeof(FixtureDependencyOrderTarget))]
    public class FixtureDependencyOrderReferrer
    {
        [Test]
        public void Referrer()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FixtureDependencyParallelBeforeSlow
    {
        [Test]
        public void BeforeSlowTest()
        {
            Thread.Sleep(150);
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [DependsOn(typeof(FixtureDependencyParallelBeforeSlow))]
    public class FixtureDependencyParallelAfter
    {
        [Test]
        public void AfterParallelDependency()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FixtureDependencyParallelIndependent
    {
        [Test]
        public void IndependentTest()
        {
            Assert.That(true, Is.True);
        }
    }
    #endregion
}
