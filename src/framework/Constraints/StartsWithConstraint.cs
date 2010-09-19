
namespace NUnit.Framework.Constraints
{
    /// <summary>
	/// StartsWithConstraint can test whether a string starts
	/// with an expected substring.
	/// </summary>
    public class StartsWithConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StartsWithConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected string</param>
        public StartsWithConstraint(string expected) : base(expected) { }

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
                return ((string)actual).ToLower().StartsWith(expected.ToLower());
            else
                return ((string)actual).StartsWith(expected);
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String starting with");
            writer.WriteExpectedValue( MsgUtils.ClipString(expected, writer.MaxLineLength - 40, 0) );
			if ( this.caseInsensitive )
				writer.WriteModifier( "ignoring case" );
		}
    } 
}