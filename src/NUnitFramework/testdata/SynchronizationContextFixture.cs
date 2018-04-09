// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Threading;
using NUnit.Framework;

#if ASYNC
using System.Threading.Tasks;
#endif

namespace NUnit.TestData
{
    public sealed class SynchronizationContextFixture : IDisposable
    {
        private readonly SynchronizationContext _synchronizationContext;
        private IDisposable installation;

        public SynchronizationContextFixture(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        [SetUp]
        public void SetUp()
        {
            installation = TemporarySynchronizationContext(_synchronizationContext);
        }

        public void Dispose()
        {
            // Make sure this gets cleaned up even if the tests fail!
            installation.Dispose();
        }

#if ASYNC
        [Test]
        public async Task YieldAndAssertSameThread()
        {
            var originalThread = Thread.CurrentThread;
#if NET40
            await TaskEx.Yield();
#else
            await Task.Yield();
#endif
            Assert.That(Thread.CurrentThread, Is.SameAs(originalThread));
        }
#endif

        private static IDisposable TemporarySynchronizationContext(SynchronizationContext synchronizationContext)
        {
            var originalContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            return On.Dispose(() => SynchronizationContext.SetSynchronizationContext(originalContext));
        }
    }
}
