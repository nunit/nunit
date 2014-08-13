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

#region Using Directives

using System.Threading;
using NUnit.Framework.Compatibility;

#endregion

namespace NUnit.Framework.Tests.Compatibility
{
    [TestFixture]
    public class StopwatchTests
    {
        private const int SLEEP = 100;
        private const int MIN = (int)(SLEEP * 0.9);
        private const int MAX = (int)(SLEEP * 1.5);

        [Test]
        public void TestStartNewIsRunning()
        {
            var watch = Stopwatch.StartNew();
            Thread.Sleep(SLEEP);
            Assert.That(watch.IsRunning, Is.True);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(watch.ElapsedMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void TestConstructNewIsNotRunning()
        {
            var watch = new Stopwatch();
            Thread.Sleep(SLEEP);
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
        }

        [Test]
        public void TestGetTimestamp()
        {
            var before = Stopwatch.GetTimestamp();
            Thread.Sleep(SLEEP);
            var after = Stopwatch.GetTimestamp();
            Assert.That(before, Is.LessThan(after));
        }

        [Test]
        public void TestReset()
        {
            var watch = Stopwatch.StartNew();
            Thread.Sleep(SLEEP);
            watch.Reset();
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
        }

        [Test]
        public void TestRestart()
        {
            var watch = Stopwatch.StartNew();
            Thread.Sleep(SLEEP);
            watch.Restart();
            Thread.Sleep(SLEEP);
            Assert.That(watch.IsRunning, Is.True);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(watch.ElapsedMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void TestStart()
        {
            var watch = new Stopwatch();
            Assert.That(watch.IsRunning, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(0));
            watch.Start();
            Thread.Sleep(SLEEP);
            Assert.That(watch.IsRunning, Is.True);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(watch.ElapsedMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void TestStop()
        {
            var watch = Stopwatch.StartNew();
            Assert.That(watch.IsRunning, Is.True);
            Thread.Sleep(SLEEP);
            watch.Stop();
            Assert.That(watch.IsRunning, Is.False);
            var saved = watch.ElapsedMilliseconds;
            Assert.That(saved, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(watch.ElapsedMilliseconds, Is.LessThan(MAX));
            Thread.Sleep(SLEEP);
            Assert.That(watch.ElapsedMilliseconds, Is.EqualTo(saved));
        }        

        [Test]
        public void TestFrequency()
        {
            Assert.That(Stopwatch.Frequency, Is.GreaterThan(0));
        }
        
#if NETCF || SILVERLIGHT
        [Test]
        public void TestIsHighResolution()
        {
            Assert.That(Stopwatch.IsHighResolution, Is.False);
        }    
#endif
    }
}