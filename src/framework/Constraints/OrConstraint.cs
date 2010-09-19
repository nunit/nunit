
namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// OrConstraint succeeds if either member succeeds
	/// </summary>
    public class OrConstraint : BinaryConstraint
    {
		/// <summary>
		/// Create an OrConstraint from two other constraints
		/// </summary>
		/// <param name="left">The first constraint</param>
		/// <param name="right">The second constraint</param>
		public OrConstraint(Constraint left, Constraint right) : base(left, right) { }

		/// <summary>
		/// Apply the member constraints to an actual value, succeeding 
		/// succeeding as soon as one of them succeeds.
		/// </summary>
		/// <param name="actual">The actual value</param>
		/// <returns>True if either constraint succeeded</returns>
		public override bool Matches(object actual)
        {
            this.actual = actual;
            return Left.Matches(actual) || Right.Matches(actual);
        }

		/// <summary>
		/// Write a description for this contraint to a MessageWriter
		/// </summary>
		/// <param name="writer">The MessageWriter to receive the description</param>
		public override void WriteDescriptionTo(MessageWriter writer)
        {
            Left.WriteDescriptionTo(writer);
            writer.WriteConnector("or");
            Right.WriteDescriptionTo(writer);
        }
    }
}