using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// AllItemsConstraint applies another constraint to each
	/// item in a collection, succeeding if they all succeed.
	/// </summary>
	public class AllItemsConstraint : PrefixConstraint
	{
		/// <summary>
		/// Construct an AllItemsConstraint on top of an existing constraint
		/// </summary>
		/// <param name="itemConstraint"></param>
		public AllItemsConstraint(Constraint itemConstraint)
			: base( itemConstraint )
        {
            this.DisplayName = "all";
        }

		/// <summary>
		/// Apply the item constraint to each item in the collection,
		/// failing if any item fails.
		/// </summary>
		/// <param name="actual"></param>
		/// <returns></returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if ( !(actual is IEnumerable) )
				throw new ArgumentException( "The actual value must be an IEnumerable", "actual" );

			foreach(object item in (IEnumerable)actual)
				if (!baseConstraint.Matches(item))
					return false;

			return true;
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("all items");
			baseConstraint.WriteDescriptionTo(writer);
		}
	}
}