

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// FalseConstraint tests that the actual value is false
    /// </summary>
    public class FalseConstraint : BasicConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FalseConstraint"/> class.
        /// </summary>
        public FalseConstraint() : base(false, "False") { }
    }
}