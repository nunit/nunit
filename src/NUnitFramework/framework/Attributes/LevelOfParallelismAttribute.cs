// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Sets the number of worker threads that may be allocated by the framework
    /// for running tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class LevelOfParallelismAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a LevelOfParallelismAttribute.
        /// </summary>
        /// <param name="level">The number of worker threads to be created by the framework.</param>
        public LevelOfParallelismAttribute(int level) : base(level) { }
    }
}
