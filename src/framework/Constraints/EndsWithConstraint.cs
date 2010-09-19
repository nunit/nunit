
namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EndsWithConstraint can test whether a string ends
    /// with an expected substring.
    /// </summary>
    public class EndsWithConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EndsWithConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected string</param>
        public EndsWithConstraint(string expected) : base(expected) { }

        /// <summary>
        /// Test whether the constraint is matched by the actual value.
        /// This is a template method, which calls the IsMatch method
        /// of the derived class.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;

            if (!(actual is string))
                return false;

            if ( this.caseInsensitive )
                return ((string)actual).ToLower().EndsWith(expected.ToLower());
            else
                return ((string)actual).EndsWith(expected);
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String ending with");
            writer.WriteExpectedValue(expected);
			if ( this.caseInsensitive )
				writer.WriteModifier( "ignoring case" );
		}
    }
}