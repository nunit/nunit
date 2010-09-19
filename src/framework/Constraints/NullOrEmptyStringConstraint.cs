
using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NullEmptyStringConstraint tests whether a string is either null or empty.
    /// </summary>
    public class NullOrEmptyStringConstraint : Constraint
    {
        /// <summary>
        /// Constructs a new NullOrEmptyStringConstraint
        /// </summary>
        public NullOrEmptyStringConstraint()
        {
            this.DisplayName = "nullorempty";
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;

            if (actual == null)
                return true;

            if (!(actual is string))
                throw new ArgumentException("Actual value must be a string", "actual");

            return (string)actual == string.Empty;
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write("null or empty string");
        }
    } 
}