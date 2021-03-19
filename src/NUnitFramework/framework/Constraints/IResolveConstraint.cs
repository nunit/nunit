// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The IResolveConstraint interface is implemented by all
    /// complete and resolvable constraints and expressions.
    /// </summary>
    public interface IResolveConstraint
    {
        /// <summary>
        /// Return the top-level constraint for this expression
        /// </summary>
        /// <returns></returns>
        IConstraint Resolve();
    }
}
