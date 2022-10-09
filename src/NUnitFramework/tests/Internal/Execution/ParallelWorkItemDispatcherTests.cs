// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    public class ParallelWorkItemDispatcherTests
    {
        private const int LEVEL_OF_PARALLELISM = 4;
        private ParallelWorkItemDispatcher _dispatcher;

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

            Assert.That(shifts.Count, Is.EqualTo(3));

            // Parallel Shift
            Assert.That(shifts[0].Name, Is.EqualTo("Parallel"));
            Assert.That(shifts[0].Queues.Count, Is.EqualTo(2));
            Assert.That(shifts[0].Queues[0].Name, Is.EqualTo("ParallelQueue"));
            Assert.That(shifts[0].Queues[1].Name, Is.EqualTo("ParallelSTAQueue"));
            Assert.That(shifts[0].Workers.Count, Is.EqualTo(LEVEL_OF_PARALLELISM + 1));

            // NonParallel Shift
            Assert.That(shifts[1].Name, Is.EqualTo("NonParallel"));
            Assert.That(shifts[1].Queues.Count, Is.EqualTo(1));
            Assert.That(shifts[1].Queues[0].Name, Is.EqualTo("NonParallelQueue"));
            Assert.That(shifts[1].Workers.Count, Is.EqualTo(1));

            // NonParallelSTAShift
            Assert.That(shifts[2].Name, Is.EqualTo("NonParallelSTA"));
            Assert.That(shifts[2].Queues.Count, Is.EqualTo(1));
            Assert.That(shifts[2].Queues[0].Name, Is.EqualTo("NonParallelSTAQueue"));
            Assert.That(shifts[2].Workers.Count, Is.EqualTo(1));
        }
    }
}
