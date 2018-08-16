using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Keeps track of an interval time which can be represented in
    /// Minutes, Seconds or Milliseconds
    /// </summary>
    public sealed class Interval
    {
        private readonly int _value;
        private IntervalUnit _mode;

        /// <summary>
        /// Constructs a interval given an value in milliseconds
        /// </summary>
        public Interval(int value)
        {
            _value = value;
            _mode = IntervalUnit.Millisecond;
            AsTimeSpan = TimeSpan.FromMilliseconds(value);
        }

        /// <summary>
        /// Gets Interval value represented as a TimeSpan object
        /// </summary>
        public TimeSpan AsTimeSpan { get; private set; }

        /// <summary>
        /// Returns the interval with the current value as a number of minutes.
        /// </summary>
        public Interval InMinutes
        {
            get
            {
                AsTimeSpan = TimeSpan.FromMinutes(_value);
                _mode = IntervalUnit.Minute;
                return this;
            }
        }

        /// <summary>
        /// Returns the interval with the current value as a number of seconds.
        /// </summary>
        public Interval InSeconds
        {
            get
            {
                AsTimeSpan = TimeSpan.FromSeconds(_value);
                _mode = IntervalUnit.Second;
                return this;
            }
        }

        /// <summary>
        /// Returns the interval with the current value as a number of milliseconds.
        /// </summary>
        public Interval InMilliseconds
        {
            get
            {
                AsTimeSpan = TimeSpan.FromMilliseconds(_value);
                _mode = IntervalUnit.Millisecond;
                return this;
            }
        }

        /// <summary>
        /// Is true for intervals created with a non-zero value
        /// </summary>
        public bool IsNotZero
        {
            get { return _value != 0; }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}{2}", _value, _mode.ToString().ToLower(), _value > 1 ? "s" : string.Empty);
        }

        /// <summary>
        /// IntervalUnit provides the semantics to the value stored in Interval class.
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
