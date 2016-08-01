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
using System;

namespace System.Diagnostics
{
    /// <summary>
    /// This class is a System.Diagnostics.Stopwatch on operating systems that support it. On those that don't,
    /// it replicates the functionality at the resolution supported.
    /// </summary>
    public class Stopwatch
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;

        private long _elapsedMilliseconds;
        private bool _isRunning;
        private long _startMilliseconds = GetTimestamp();

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                return (_elapsedMilliseconds + GetTimeSinceLastStart())/TicksPerMillisecond;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Stopwatch timer is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }

        /// <summary>
        /// Gets the current number of ticks in the timer mechanism.
        /// </summary>
        /// <remarks>
        /// If the Stopwatch class uses a high-resolution performance counter, GetTimestamp returns the current
        /// value of that counter. If the Stopwatch class uses the system timer, GetTimestamp returns the current 
        /// DateTime.Ticks property of the DateTime.Now instance.
        /// </remarks>
        /// <returns>A long integer representing the tick counter value of the underlying timer mechanism.</returns>
        public static long GetTimestamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero.
        /// </summary>
        public void Reset()
        {
            _elapsedMilliseconds = 0;
            _isRunning = false;
        }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _startMilliseconds = GetTimestamp();
            }
        }

        /// <summary>
        /// Initializes a new Stopwatch instance, sets the elapsed time property to zero, and starts measuring elapsed time.
        /// </summary>
        /// <returns>A Stopwatch that has just begun measuring elapsed time.</returns>
        public static Stopwatch StartNew()
        {
            var watch = new Stopwatch();
            watch.Start();
            return watch;
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                _elapsedMilliseconds += GetTimeSinceLastStart();
                _isRunning = false;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Gets the frequency of the timer as the number of ticks per second.
        /// </summary>
        public static long Frequency
        {
            get
            {
                return TicksPerSecond;
            }
        }

        /// <summary>
        /// Indicates whether the timer is based on a high-resolution performance counter.
        /// </summary>
        public static bool IsHighResolution
        {
            get
            {
                return false;
            }
        }

        private long GetTimeSinceLastStart()
        {
            if (_isRunning)
            {
                var now = GetTimestamp();
                return now - _startMilliseconds;
            }
            return 0;
        }
    }
}
#endif
