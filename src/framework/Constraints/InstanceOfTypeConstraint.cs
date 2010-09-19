
using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// InstanceOfTypeConstraint is used to test that an object
    /// is of the same type provided or derived from it.
    /// </summary>
    public class InstanceOfTypeConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an InstanceOfTypeConstraint for the type provided
        /// </summary>
        /// <param name="type">The expected Type</param>
        public InstanceOfTypeConstraint(Type type)
            : base(type)
        {
            this.DisplayName = "instanceof";
        }

        /// <summary>
        /// Test whether an object is of the specified type or a derived type
        /// </summary>
        /// <param name="actual">The object to be tested</param>
        /// <returns>True if the object is of the provided type or derives from it, otherwise false.</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;
            return actual != null && expectedType.IsInstanceOfType(actual);
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer">The MessageWriter to use</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("instance of");
            writer.WriteExpectedValue(expectedType);
        }
    }
}