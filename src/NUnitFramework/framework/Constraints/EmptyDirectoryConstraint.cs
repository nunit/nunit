// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EmptyDirectoryConstraint is used to test that a directory is empty
    /// </summary>
    public class EmptyDirectoryConstraint : Constraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "an empty directory"; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var dirInfo = ConstraintUtils.RequireActual<DirectoryInfo>(actual, nameof(actual));
            bool hasSucceeded = !dirInfo.EnumerateFileSystemInfos().Any();
            return new ConstraintResult(this, actual, hasSucceeded);
        }

        // TODO: Decide if we need a special result for this
        ///// <summary>
        ///// Write the actual value for a failing constraint test to a
        ///// MessageWriter. The default implementation simply writes
        ///// the raw value of actual, leaving it to the writer to
        ///// perform any formatting.
        ///// </summary>
        ///// <param name="writer">The writer on which the actual value is displayed</param>
        //public override void WriteActualValueTo(MessageWriter writer)
        //{
        //    DirectoryInfo dir = actual as DirectoryInfo;
        //    if (dir == null)
        //        base.WriteActualValueTo(writer);
        //    else
        //    {
        //        writer.WriteActualValue(dir);
        //        writer.Write(" with {0} files and {1} directories", files, subdirs);
        //    }
        //}
    }
}
