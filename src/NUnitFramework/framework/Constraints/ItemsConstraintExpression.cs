// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// An extension of ResolvableConstraintExpression that adds a no-op Items property for readability.
    /// </summary>
    public sealed class ItemsConstraintExpression : ConstraintExpression
    {
        /// <summary>
        /// Create a new instance of ItemsConstraintExpression
        /// </summary>
        public ItemsConstraintExpression() { }

        /// <summary>
        /// Create a new instance of ResolvableConstraintExpression,
        /// passing in a pre-populated ConstraintBuilder.
        /// </summary>
        /// <param name="builder"></param>
        public ItemsConstraintExpression(ConstraintBuilder builder)
            : base(builder) { }

        /// <summary>
        /// No-op property for readability.
        /// </summary>
        public ResolvableConstraintExpression Items => new ResolvableConstraintExpression(builder);
    }
}
