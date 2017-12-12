// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if PARALLEL
using System.Collections.Generic;
using System.Linq;

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
#if APARTMENT_STATE
                    "ParallelSTAQueue",
                    "NonParallelSTAQueue"
#endif
                }));
        }

        [Test]
        public void ConstructorCreatesShifts()
        {
            var shifts = new List<WorkShift>(_dispatcher.Shifts);

            Assert.That(shifts, Is.Unique);

#if APARTMENT_STATE
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
#else
            Assert.That(shifts.Count, Is.EqualTo(2));

            // Parallel Shift
            Assert.That(shifts[0].Name, Is.EqualTo("Parallel"));
            Assert.That(shifts[0].Queues.Count, Is.EqualTo(1));
            Assert.That(shifts[0].Queues[0].Name, Is.EqualTo("ParallelQueue"));
            Assert.That(shifts[0].Workers.Count, Is.EqualTo(LEVEL_OF_PARALLELISM));

            // NonParallel Shift
            Assert.That(shifts[1].Name, Is.EqualTo("NonParallel"));
            Assert.That(shifts[1].Queues.Count, Is.EqualTo(1));
            Assert.That(shifts[1].Queues[0].Name, Is.EqualTo("NonParallelQueue"));
            Assert.That(shifts[1].Workers.Count, Is.EqualTo(1));
#endif
        }
    }
}
#endif
