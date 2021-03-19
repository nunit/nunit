// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// FileExistsConstraint is used to determine if a file exists
    /// </summary>
    [Obsolete("The FileExistsConstraint class has been deprecated and will be removed in a future release. "
        + "Please use " + nameof(FileOrDirectoryExistsConstraint) + " instead.")]
    public class FileExistsConstraint : FileOrDirectoryExistsConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExistsConstraint"/> class.
        /// </summary>
        public FileExistsConstraint() : base(true)
        {
        }

        #region Overrides of Constraint

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "file exists"; }
        }

        #endregion
    }
}
