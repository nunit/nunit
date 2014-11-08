// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if !SILVERLIGHT && !PORTABLE
using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EmptyDirectoryConstraint is used to test that a directory is empty
    /// </summary>
    public class EmptyDirectoryConstraint : Constraint
    {
        private int files = 0;
        private int subdirs = 0;

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
            DirectoryInfo dirInfo = actual as DirectoryInfo;
            if (dirInfo == null)
                throw new ArgumentException("The actual value must be a DirectoryInfo", "actual");
            files = dirInfo.GetFiles().Length;
            subdirs = dirInfo.GetDirectories().Length;
            bool hasSucceeded = files == 0 && subdirs == 0;

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
#endif