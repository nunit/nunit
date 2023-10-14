// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks tests that should NOT be run in parallel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NonParallelizableAttribute : ParallelizableAttribute
    {
        /// <summary>
        /// Construct a NonParallelizableAttribute.
        /// </summary>
        public NonParallelizableAttribute() : base(ParallelScope.None)
        {
        }
    }
}
