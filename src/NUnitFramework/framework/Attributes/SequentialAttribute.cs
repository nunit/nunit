// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test to use a sequential join of any provided argument data.
    /// Arguments will be combined into test cases, taking the next value of
    /// each argument until all are used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited=false)]
    public class SequentialAttribute : CombiningStrategyAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SequentialAttribute() : base(new SequentialStrategy(), new ParameterDataSourceProvider()) { }
    }
}
