// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

using System;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Internal
{
    public static class SingleThreadedTestSynchronizationContextTests
    {
        [Test]
        public static void RecursivelyPostedWorkIsStillExecutedIfStartedWithinTimeout()
        {
            using (var context = new SingleThreadedTestSynchronizationContext())
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = false;

                context.Post(state =>
                {
                    context.Post(_ => Thread.Sleep(TimeSpan.FromSeconds(0.5)), null);
                    context.Post(_ => wasExecuted = true, null);

                    context.ShutDown();
                }, null);

                context.Run();
                Assert.That(wasExecuted);
            }
        }

        [Test]
        public static void RecursivelyPostedWorkIsStillExecutedWithinTimeout()
        {
            using (var context = new SingleThreadedTestSynchronizationContext())
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                context.Post(_ =>
                {
                    ScheduleWorkRecursively(Stopwatch.StartNew(), until: TimeSpan.FromSeconds(0.5));

                    context.ShutDown();
                }, null);

                context.Run();
            }
        }

        private static void ScheduleWorkRecursively(Stopwatch stopwatch, TimeSpan until)
        {
            if (stopwatch.Elapsed >= until) return;

            SynchronizationContext.Current.Post(_ => ScheduleWorkRecursively(stopwatch, until), null);
        }
    }
}
