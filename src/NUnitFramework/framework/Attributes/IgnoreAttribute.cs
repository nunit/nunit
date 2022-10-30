// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as being ignored. Ignored tests result in a warning message when the tests are run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class|AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public class IgnoreAttribute : NUnitAttribute, IApplyToTest
    {
        private readonly string _reason;
        private DateTime? _untilDate;
        private string? _until;

        /// <summary>
        /// Constructs the attribute giving a reason for ignoring the test
        /// </summary>
        /// <param name="reason">The reason for ignoring the test</param>
        public IgnoreAttribute(string reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// The date in the future to stop ignoring the test as a string in UTC time. 
        /// For example for a date and time, "2014-12-25 08:10:00Z" or for just a date,
        /// "2014-12-25". If just a date is given, the Ignore will expire at midnight UTC.
        /// </summary>
        /// <remarks>
        /// Once the ignore until date has passed, the test will be marked
        /// as runnable. Tests with an ignore until date will have an IgnoreUntilDate
        /// property set which will appear in the test results.
        /// </remarks>
        /// <exception cref="FormatException">The string does not contain a valid string representation of a date and time.</exception> 
        public string? Until
        {
            get { return _until; }
            set
            {
                _until = value;
                _untilDate = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }
        }

        #region IApplyToTest members

        /// <summary>
        /// Modifies a test by marking it as Ignored.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable)
            {
                if (_untilDate.HasValue)
                {
                    if (_untilDate.Value > DateTime.Now)
                    {
                        test.RunState = RunState.Ignored;
                        test.Properties.AddIgnoreUntilReason(_untilDate.Value, _reason);
                    }
                    test.Properties.Set(PropertyNames.IgnoreUntilDate, _untilDate.Value.ToString("u") );

                    return;
                }
                test.RunState = RunState.Ignored;
                test.Properties.Set(PropertyNames.SkipReason, _reason);
            }
        }

        #endregion
    }
}
