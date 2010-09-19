
namespace NUnit.Framework.Constraints
{
    /// <summary>
	/// SubstringConstraint can test whether a string contains
	/// the expected substring.
	/// </summary>
    public class SubstringConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SubstringConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected.</param>
        public SubstringConstraint(string expected) : base(expected) { }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;
            
            if ( !(actual is string) )
                return false;

            if (this.caseInsensitive)
                return ((string)actual).ToLower().IndexOf(expected.ToLower()) >= 0;
            else
                return ((string)actual).IndexOf(expected) >= 0;
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String containing");
            writer.WriteExpectedValue(expected);
			if ( this.caseInsensitive )
				writer.WriteModifier( "ignoring case" );
		}
    } 
}