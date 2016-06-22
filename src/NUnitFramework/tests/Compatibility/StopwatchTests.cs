// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if SILVERLIGHT
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Tests.Compatibility
{
    [TestFixture]
    public class StopwatchTests
    {
        private const int DELAY = 100;
        private const int TOLERANCE = 50;

        [Test]
        public void TestStartNewIsRunning()
        {
            var watch = Stopwatch.StartNew();
            Delay(DELAY);
            Assert.That(watch.IsRunning, Is.True);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(DELAY).Within(TOLERANCE).Percent);
        }

        [Test]
        public void TestConstructNewIsNotRunning()
        {
            var watch = new Stopwatch();
            Delay(DELAY);
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
        }

        [Test]
        public void TestGetTimestamp()
        {
            var before = Stopwatch.GetTimestamp();
            Delay(DELAY);
            var after = Stopwatch.GetTimestamp();
            Assert.That(before, Is.LessThan(after));
        }

        [Test]
        public void TestReset()
        {
            var watch = Stopwatch.StartNew();
            Delay(DELAY);
            watch.Reset();
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
        }

        [Test]
        public void TestStart()
        {
            var watch = new Stopwatch();
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
            watch.Start();
            Delay(DELAY);
            Assert.That(watch.IsRunning, Is.True);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(DELAY).Within(TOLERANCE).Percent);
        }

        [Test]
        public void TestStop()
        {
            var watch = Stopwatch.StartNew();
            Assert.That(watch.IsRunning, Is.True);
            Delay(DELAY);
            watch.Stop();
            Assert.That(watch.IsRunning, Is.False);
            var saved = watch.ElapsedMilliseconds;
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(DELAY).Within(TOLERANCE).Percent);
            Delay(DELAY);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(saved));
        }        

        [Test]
        public void TestFrequency()
        {
            Assert.That(Stopwatch.Frequency, Is.GreaterThan(0));
        }
        
        [Test]
        public void TestIsHighResolution()
        {
            Assert.That(Stopwatch.IsHighResolution, Is.False);
        }    

        private static AutoResetEvent waitEvent = new AutoResetEvent(false);

        private static void Delay(int delay)
        {
            waitEvent.WaitOne(delay);
        }
    }
}
#endif
