// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics;

namespace NUnitLite
{
    public class TimeStamp
    {
        public TimeStamp()
        {
            DateTime = DateTime.UtcNow;
            Ticks = Stopwatch.GetTimestamp();
        }

        public DateTime DateTime { get; }
        public long Ticks { get; }

        public static double TicksToSeconds(long ticks)
        {
            return (double)ticks / Stopwatch.Frequency;
        }
    }
}
