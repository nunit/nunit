// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test as using a pairwise join of any supplied argument data. Arguments will be 
    /// combined in such a way that all possible pairs of arguments are used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited=false)]
    public class PairwiseAttribute : CombiningStrategyAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PairwiseAttribute() : base(new PairwiseStrategy(), new ParameterDataSourceProvider()) { }
    }
}
