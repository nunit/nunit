
namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator that requires at least one of it's arguments to succeed
    /// </summary>
    public class OrOperator : BinaryOperator
    {
        /// <summary>
        /// Construct an OrOperator
        /// </summary>
        public OrOperator()
        {
            this.left_precedence = this.right_precedence = 3;
        }

        /// <summary>
        /// Apply the operator to produce an OrConstraint
        /// </summary>
        public override Constraint ApplyOperator(Constraint left, Constraint right)
        {
            return new OrConstraint(left, right);
        }
    }
}