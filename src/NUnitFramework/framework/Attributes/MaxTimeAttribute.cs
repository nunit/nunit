// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies the maximum time (in milliseconds) for a test case to succeed.
    /// </summary>
    [AttributeUsage( AttributeTargets.Method, AllowMultiple=false, Inherited=false )]
    public sealed class MaxTimeAttribute : PropertyAttribute, IWrapSetUpTearDown
    {
        private readonly int _milliseconds;
        /// <summary>
        /// Construct a MaxTimeAttribute, given a time in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The maximum elapsed time in milliseconds</param>
        public MaxTimeAttribute( int milliseconds )
            : base( milliseconds )
        {
            _milliseconds = milliseconds;
        }

        #region ICommandWrapper Members

        TestCommand ICommandWrapper.Wrap(TestCommand command)
        {
            return new MaxTimeCommand(command, _milliseconds);
        }

        #endregion
    }
}
