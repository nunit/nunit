// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Tests.Attributes
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.All)]
    public class LifeCycleAttributeParallelTests
    {
        private static readonly Dictionary<object, long> ObjectIds = new();

        private readonly int _constructorCount = 0;
        private int _setupCount = 0;

        public LifeCycleAttributeParallelTests()
        {
            OutputReferenceId("Ctor");
            _constructorCount++;
        }

        [SetUp]
        public void SetUp()
        {
            OutputReferenceId("SetUp");
            _setupCount++;
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance1()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance1");
            Assert.That(_constructorCount, Is.EqualTo(1));
            Assert.That(_setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance2()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance2");
            Assert.That(_constructorCount, Is.EqualTo(1));
            Assert.That(_setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance3()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance3");
            Assert.That(_constructorCount, Is.EqualTo(1));
            Assert.That(_setupCount, Is.EqualTo(1));
        }

        private void OutputReferenceId(string location)
        {
            if (!ObjectIds.TryGetValue(this, out long id))
            {
                ObjectIds[this] = id = ObjectIds.Count + 1;
            }
            TestContext.WriteLine($"{location}: {id}>");
        }
    }
}
