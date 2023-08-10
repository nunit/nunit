// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework
{
    /// <summary>
    /// The IgnoredTestCaseData class represents a ignored TestCaseData. It adds
    /// the ability to set a date until which the test will be ignored.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class IgnoredTestCaseData : TestCaseData
    {
        #region Instance Fields

        /// <summary>
        /// The previous RunState
        /// </summary>
        private readonly RunState _prevRunState;

        #endregion

        #region Constructors

        internal IgnoredTestCaseData(TestCaseData data, RunState prevRunState)
        {
            Arguments = data.Arguments;
            ArgDisplayNames = data.ArgDisplayNames;
            ExpectedResult = data.ExpectedResult;
            HasExpectedResult = data.HasExpectedResult;
            OriginalArguments = data.OriginalArguments;
            Properties = data.Properties;
            RunState = data.RunState;
            TestName = data.TestName;
            _prevRunState = prevRunState;
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Set the date that the test is being ignored until
        /// </summary>
        /// <param name="datetime">The date that the test is being ignored until</param>
        /// <returns>A modified TestCaseData.</returns>
        public TestCaseData Until(DateTimeOffset datetime)
        {
            if (_prevRunState != RunState.NotRunnable)
            {
                if (datetime > DateTimeOffset.UtcNow)
                {
                    RunState = RunState.Ignored;
                    string? reason = (string?)Properties.Get(PropertyNames.SkipReason);
                    Properties.AddIgnoreUntilReason(datetime, reason);
                }
                else
                {
                    RunState = _prevRunState;
                }
                Properties.Set(PropertyNames.IgnoreUntilDate, datetime.ToString("u"));
            }
            return this;
        }

        #endregion

    }
}
