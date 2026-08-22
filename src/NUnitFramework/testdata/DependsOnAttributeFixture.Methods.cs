// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    #region Method Dependencies
    [TestFixture]
    public class MethodDependencyOrdered
    {
        [Test]
        [DependsOn(nameof(Before))]
        public void After()
        {
            FixtureDependencyEvents.Record(nameof(After));
            Assert.That(true, Is.True);
        }

        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record(nameof(Before));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFailing
    {
        [Test]
        public void BeforeFailing()
        {
            FixtureDependencyEvents.Record(nameof(BeforeFailing));
            Assert.Fail("Intentional failure for method dependency behavior testing");
        }

        [Test]
        [DependsOn(nameof(BeforeFailing))]
        public void AfterFailing()
        {
            FixtureDependencyEvents.Record(nameof(AfterFailing));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFork
    {
        [Test]
        public void Root()
        {
            FixtureDependencyEvents.Record(nameof(Root));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(Root))]
        public void NodeA()
        {
            FixtureDependencyEvents.Record(nameof(NodeA));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(Root))]
        public void NodeB()
        {
            FixtureDependencyEvents.Record(nameof(NodeB));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFailingAllowed
    {
        [Test]
        public void BeforeFailing()
        {
            FixtureDependencyEvents.Record(nameof(BeforeFailing));
            Assert.Fail("Intentional failure for method dependency behavior testing");
        }

        [Test]
        [DependsOn(nameof(BeforeFailing), AllowFailure = true)]
        public void AfterFailingAllowed()
        {
            FixtureDependencyEvents.Record(nameof(AfterFailingAllowed));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyMissing
    {
        [Test]
        [DependsOn(nameof(NotATestMethod))]
        public void DependsOnMissingMethod()
        {
            FixtureDependencyEvents.Record(nameof(DependsOnMissingMethod));
            Assert.That(true, Is.True);
        }

        private void NotATestMethod()
        {
        }
    }

    [TestFixture]
    public class MethodDependencyCycle
    {
        [Test]
        [DependsOn(nameof(B))]
        public void A()
        {
            FixtureDependencyEvents.Record(nameof(A));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(A))]
        public void B()
        {
            FixtureDependencyEvents.Record(nameof(B));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(A))]
        public void Referrer()
        {
            FixtureDependencyEvents.Record(nameof(Referrer));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencySelfReferential
    {
        [Test]
        [DependsOn(nameof(Self))]
        public void Self()
        {
            FixtureDependencyEvents.Record(nameof(Self));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOrderTarget
    {
        [Test]
        [Order(1)]
        public void Target()
        {
            FixtureDependencyEvents.Record(nameof(Target));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(Target))]
        public void Referrer()
        {
            FixtureDependencyEvents.Record(nameof(Referrer));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOrderReferrer
    {
        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record(nameof(Before));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(Before))]
        [Order(1)]
        public void After()
        {
            FixtureDependencyEvents.Record(nameof(After));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOrderIndependent
    {
        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record(nameof(Before));
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(nameof(Before))]
        public void After()
        {
            FixtureDependencyEvents.Record(nameof(After));
            Assert.That(true, Is.True);
        }

        [Test]
        [Order(1)]
        public void OrderedIndependent()
        {
            FixtureDependencyEvents.Record(nameof(OrderedIndependent));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyParallel
    {
        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void BeforeSlow()
        {
            Thread.Sleep(150);
            FixtureDependencyEvents.Record(nameof(BeforeSlow));
            Assert.That(true, Is.True);
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        [DependsOn(nameof(BeforeSlow))]
        public void AfterParallelDependency()
        {
            FixtureDependencyEvents.Record(nameof(AfterParallelDependency));
            Assert.That(true, Is.True);
        }

        [Test]
        public void IndependentTest()
        {
            FixtureDependencyEvents.Record(nameof(IndependentTest));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    [DependsOn(nameof(FixtureMethod))]
    public class MethodDependencyInvalidStringOnFixture
    {
        [Test]
        public void FixtureMethod()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyInvalidTypeOnMethod
    {
        [Test]
        public void DependencyTarget()
        {
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOn(typeof(MethodDependencyInvalidTypeOnMethod))]
        public void DependantMethod()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyBase
    {
        [Test]
        public virtual void BaseMethod()
        {
            FixtureDependencyEvents.Record(nameof(BaseMethod));
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOnBaseClass : MethodDependencyBase
    {
        [Test]
        [DependsOn(nameof(BaseMethod))]
        public void Self()
        {
            FixtureDependencyEvents.Record(nameof(Self));
            Assert.That(true, Is.True);
        }
    }
    #endregion
}
