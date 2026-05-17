// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies the maximum time (in milliseconds) for a test case to succeed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MaxTimeAttribute : PropertyAttribute, IWrapSetUpTearDown
    {
        private readonly int _milliseconds;

        /// <summary>
        /// Gets or sets an optional warning threshold in milliseconds.
        /// If the test takes longer than this time, but less than the maximum time, a warning is issued.
        /// </summary>
        public int WarningTime { get; set; }
        /// <summary>
        /// Construct a MaxTimeAttribute, given a time in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The maximum elapsed time in milliseconds</param>
        public MaxTimeAttribute(int milliseconds)
            : base(milliseconds)
        {
            _milliseconds = milliseconds;
        }

        #region ICommandWrapper Members

        TestCommand ICommandWrapper.Wrap(TestCommand command)
        {
            return new MaxTimeCommand(command, _milliseconds, WarningTime);
        }

        #endregion
    }
}
