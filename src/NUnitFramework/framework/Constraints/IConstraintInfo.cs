// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Information about the constraint for display purposes.
    /// </summary>
    public interface IConstraintInfo
    {
        #region Properties

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Arguments provided to this Constraint, for use in
        /// formatting the description.
        /// </summary>
        object?[] Arguments { get; }

        #endregion
    }
}
