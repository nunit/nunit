using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Keeps track of an interval time which can be represented in
    /// Minutes, Seconds or Milliseconds
    /// </summary>
    internal class Interval
    {
        private readonly int _amount;
        private IntervalUnit _mode;

        /// <summary>
        /// Constructs a interval given an amount in milliseconds
        /// </summary>
        public Interval(int amount)
        {
            _amount = amount;
            _mode = IntervalUnit.Millisecond;
            AsTimeSpan = TimeSpan.FromMilliseconds(amount);
        }

        /// <summary>
        /// Gets Interval value represented as a TimeSpan object
        /// </summary>
        public TimeSpan AsTimeSpan { get; private set; }

        /// <summary>
        /// Returns the interval with the current amount as a number of minutes.
        /// </summary>
        public Interval InMinutes
        {
            get
            {
                AsTimeSpan = TimeSpan.FromMinutes(_amount);
                _mode = IntervalUnit.Minute;
                return this;
            }
        }

        /// <summary>
        /// Returns the interval with the current amount as a number of seconds.
        /// </summary>
        public Interval InSeconds
        {
            get
            {
                AsTimeSpan = TimeSpan.FromSeconds(_amount);
                _mode = IntervalUnit.Second;
                return this;
            }
        }

        /// <summary>
        /// Returns the interval with the current amount as a number of milliseconds.
        /// </summary>
        public Interval InMilliseconds
        {
            get
            {
                AsTimeSpan = TimeSpan.FromMilliseconds(_amount);
                _mode = IntervalUnit.Millisecond;
                return this;
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
            return string.Format("{0} {1}{2}", _amount, _mode.ToString().ToLower(), _amount > 1 ? "s" : string.Empty);
        }

        /// <summary>
        /// IntervalUnit provides the semantics to the amount stored in Interval class.
        /// </summary>
        internal enum IntervalUnit
        {
            /// <summary>
            /// Unit representing an Interval in minutes
            /// </summary>
            Minute,

            /// <summary>
            /// Unit representing an Interval in seconds
            /// </summary>
            Second,

            /// <summary>
            /// Unit representing an Interval in milliseconds
            /// </summary>
            Millisecond
        }
    }
}