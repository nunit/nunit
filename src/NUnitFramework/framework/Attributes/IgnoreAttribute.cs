// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Attribute used to mark a test that is to be ignored.
    /// Ignored tests result in a warning message when the
    /// tests are run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class|AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public class IgnoreAttribute : NUnitAttribute, IApplyToTest
    {
        private string _reason;
        private DateTime? _untilDate;
        private string _until;

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
        public string Until
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
                        string reason = string.Format("Ignoring until {0}. {1}", _untilDate.Value.ToString("u"), _reason);
                        test.Properties.Set(PropertyNames.SkipReason, reason);
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
