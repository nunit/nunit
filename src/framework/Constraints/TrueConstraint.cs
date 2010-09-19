
namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// TrueConstraint tests that the actual value is true
    /// </summary>
    public class TrueConstraint : BasicConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrueConstraint"/> class.
        /// </summary>
        public TrueConstraint() : base(true, "True") { }
    }
}