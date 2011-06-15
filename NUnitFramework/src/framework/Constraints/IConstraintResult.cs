// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.Framework.Constraints
{

    /// <summary>
    /// Contain the result of <see cref="Constraint.Matches(object)"/> executed against an actual value.
    /// </summary>
    public interface IConstraintResult
    {
        /// <summary>
        /// True if actual value meets the Constraint criteria otherwise false.
        /// </summary>
        bool HasSucceeded { get; }

        /// <summary>
        /// The actual value that were passed to the <see cref="Constraint.Matches(object)"/> method.
        /// </summary>
        object Actual { get; }

        /// <summary>
        /// The expected criteria the Constraint matches the actual value against.
        /// </summary>
        object Expected { get; }

        #region Constraint Information

        /// <summary>
        /// Display friendly name of the constraint.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the constraint may be affected by the state the constraint hade 
        /// when <see cref="Constraint.Matches(object)"/> was performed against the actual value.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Predicate that were used when <see cref="Constraint.Matches(object)"/> was performed, 
        /// set to empty string if no predicate were used.
        /// </summary>
        string Predicate { get; }

        /// <summary>
        /// Modifier that were used when <see cref="Constraint.Matches(object)"/> was performed, 
        /// set to empty string if no modifier were used.
        /// </summary>
        string Modifier { get; }

        #endregion

        #region Write Methods

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        void WriteDescriptionTo(MessageWriter writer);

        /// <summary>
        /// Write the failure message to the MessageWriter provided
        /// as an argument. The default implementation simply passes
        /// the constraint and the actual value to the writer, which
        /// then displays the constraint description and the value.
        /// 
        /// Constraints that need to provide additional details,
        /// such as where the error occured can override this.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display the message</param>
        void WriteMessageTo(MessageWriter writer);

        /// <summary>
        /// Write the actual value for a failing constraint test to a
        /// MessageWriter. The default implementation simply writes
        /// the raw value of actual, leaving it to the writer to
        /// perform any formatting.
        /// </summary>
        /// <param name="writer">The writer on which the actual value is displayed</param>
        void WriteActualValueTo(MessageWriter writer);

        #endregion
    }
}
