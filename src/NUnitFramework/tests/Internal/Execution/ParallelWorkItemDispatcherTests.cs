// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Tests.Internal.Execution
{
    public class ParallelWorkItemDispatcherTests
    {
        private const int LEVEL_OF_PARALLELISM = 4;
        private ParallelWorkItemDispatcher? _dispatcher;

        [SetUp]
        public void CreateDispatcher()
        {
            _dispatcher = new ParallelWorkItemDispatcher(LEVEL_OF_PARALLELISM);
        }

        [Test]
        public void ConstructorCreatesQueues()
        {
            Assert.That(_dispatcher.Queues.Select(q => q.Name),
                Is.EquivalentTo(new[]
                {
                    "ParallelQueue",
                    "NonParallelQueue",
                    "ParallelSTAQueue",
                    "NonParallelSTAQueue"
                }));
        }

        [Test]
        public void ConstructorCreatesShifts()
        {
            var shifts = new List<WorkShift>(_dispatcher.Shifts);

            Assert.That(shifts, Is.Unique);

            Assert.That(shifts, Has.Count.EqualTo(3));

            // Parallel Shift
            Assert.That(shifts[0].Name, Is.EqualTo("Parallel"));
            Assert.That(shifts[0].Queues, Has.Count.EqualTo(2));
            Assert.That(shifts[0].Queues[0].Name, Is.EqualTo("ParallelQueue"));
            Assert.That(shifts[0].Queues[1].Name, Is.EqualTo("ParallelSTAQueue"));
            Assert.That(shifts[0].Workers, Has.Count.EqualTo(LEVEL_OF_PARALLELISM + 1));

            // NonParallel Shift
            Assert.That(shifts[1].Name, Is.EqualTo("NonParallel"));
            Assert.That(shifts[1].Queues, Has.Count.EqualTo(1));
            Assert.That(shifts[1].Queues[0].Name, Is.EqualTo("NonParallelQueue"));
            Assert.That(shifts[1].Workers, Has.Count.EqualTo(1));

            // NonParallelSTAShift
            Assert.That(shifts[2].Name, Is.EqualTo("NonParallelSTA"));
            Assert.That(shifts[2].Queues, Has.Count.EqualTo(1));
            Assert.That(shifts[2].Queues[0].Name, Is.EqualTo("NonParallelSTAQueue"));
            Assert.That(shifts[2].Workers, Has.Count.EqualTo(1));
        }
    }
}
