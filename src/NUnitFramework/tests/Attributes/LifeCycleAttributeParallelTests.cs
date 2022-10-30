// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Attributes
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.All)]
    public class LifeCycleAttributeParallelTests
    {
        static ObjectIDGenerator generator = new ObjectIDGenerator();
        int constructorCount = 0;
        int setupCount = 0;

        public LifeCycleAttributeParallelTests()
        {
            OutputReferenceId("Ctor");
            constructorCount++;
        }

        [SetUp]
        public void SetUp()
        {
            OutputReferenceId("SetUp");
            setupCount++;
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance1()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance1");
            Assert.That(constructorCount, Is.EqualTo(1)); 
            Assert.That(setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance2()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance2");
            Assert.That(constructorCount, Is.EqualTo(1));
            Assert.That(setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance3()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance3");
            Assert.That(constructorCount, Is.EqualTo(1));
            Assert.That(setupCount, Is.EqualTo(1));
        }

        private void OutputReferenceId(string location)
        {
            long id = generator.GetId(this, out bool _);
            TestContext.WriteLine($"{location}: {id}>");
        }
    }
}
