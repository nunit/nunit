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
        [DependsOnTest(nameof(Before))]
        public void After()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFailing
    {
        [Test]
        public void BeforeFailing()
        {
            FixtureDependencyEvents.Record();
            Assert.Fail("Intentional failure for method dependency behavior testing");
        }

        [Test]
        [DependsOnTest(nameof(BeforeFailing))]
        public void AfterFailing()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFork
    {
        [Test]
        public void Root()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(Root))]
        public void NodeA()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(Root))]
        public void NodeB()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyFailingAllowed
    {
        [Test]
        public void BeforeFailing()
        {
            FixtureDependencyEvents.Record();
            Assert.Fail("Intentional failure for method dependency behavior testing");
        }

        [Test]
        [DependsOnTest(nameof(BeforeFailing), AllowFailure = true)]
        public void AfterFailingAllowed()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyMissing
    {
        [Test]
        [DependsOnTest(nameof(NotATestMethod))]
        public void DependsOnMissingMethod()
        {
            FixtureDependencyEvents.Record();
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
        [DependsOnTest(nameof(B))]
        public void A()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(A))]
        public void B()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(A))]
        public void Referrer()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencySelfReferential
    {
        [Test]
        [DependsOnTest(nameof(Self))]
        public void Self()
        {
            FixtureDependencyEvents.Record();
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
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(Target))]
        public void Referrer()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOrderReferrer
    {
        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(Before))]
        [Order(1)]
        public void After()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOrderIndependent
    {
        [Test]
        public void Before()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(Before))]
        public void After()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [Order(1)]
        public void OrderedIndependent()
        {
            FixtureDependencyEvents.Record();
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
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        [DependsOnTest(nameof(BeforeSlow))]
        public void AfterParallelDependency()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        public void IndependentTest()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyBase
    {
        [Test]
        public virtual void BaseMethod()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependencyOnBaseClass : MethodDependencyBase
    {
        [Test]
        [DependsOnTest(nameof(BaseMethod))]
        public void Self()
        {
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependsOnParameterizedTest
    {
        [Test]
        [DependsOnTest(nameof(ParameterizedTestA))]
        public void A()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        public void ParameterizedTestA([Values(1, 2)] int x)
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodParameterizedTestDependsOnMethod
    {
        [Test]
        public void A()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        [DependsOnTest(nameof(A))]
        public void ParameterizedTestA([Values(1, 2)] int x)
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }
    }

    [TestFixture]
    public class MethodDependsOnParameterizedTestWithFailure
    {
        [Test]
        [DependsOnTest(nameof(ParameterizedTestA))]
        public void A()
        {
            FixtureDependencyEvents.Record();
            Assert.That(true, Is.True);
        }

        [Test]
        public void ParameterizedTestA([Values(1, 2)] int x)
        {
            FixtureDependencyEvents.Record();
            Assert.That(x, Is.Odd);
        }
    }

    [TestFixture]
    public class MethodDependenciesBetweenClasses
    {
        [TestFixture]
        public class FixtureA
        {
            [Test]
            public void A_Self()
            {
                Assert.That(true, Is.True);
            }
        }

        [TestFixture]
        public class FixtureB
        {
            [Test]
            [DependsOnTest("A_Self")]
            public void B_Self()
            {
                Assert.That(true, Is.True);
            }
        }
    }
    #endregion
}
