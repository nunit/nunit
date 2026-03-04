using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Class to configure how to compare fields
    /// </summary>
    public class FieldsComparerConfiguration
    {
        /// <summary>
        /// The global fallback configuration for comparing fields.
        /// </summary>
        internal static FieldsComparerConfiguration Default { get; } = new();

        /// <summary>
        /// Gets and sets the option to compare only readonly and init only fields.
        /// This ignores all fields that are not set during initialization of an object,
        /// which are often used for caching or other internal purposes and should not be relevant for equality of objects.
        /// </summary>
        internal bool OnlyCompareInitAndReadonlyFields { get; set; }

        /// <summary>
        /// Gets and sets the tolerance to apply for time values.
        /// </summary>
        internal Tolerance TimeSpanTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Gets and sets the tolerance to apply for numeric values.
        /// </summary>
        internal Tolerance FloatingPointTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Gets and sets the tolerance to apply for numeric values.
        /// </summary>
        internal Tolerance FixedPointTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Set the tolerance to apply based upon the type of the tolerance.
        /// </summary>
        /// <remarks>
        /// This method accepts a <see cref="TimeSpan"/>, a numeric value or a <see cref="Tolerance"/> instance.
        /// </remarks>
        /// <param name="amount"></param>
        protected void SetTolerance(object amount)
        {
            if (amount is not Tolerance instance)
                instance = new Tolerance(amount);

            if (instance.Amount is TimeSpan)
            {
                TimeSpanTolerance = instance;
            }
            else if (Numerics.IsFloatingPointNumeric(instance.Amount) || instance.Mode == ToleranceMode.Ulps)
            {
                FloatingPointTolerance = instance;
            }
            else
            {
                FixedPointTolerance = instance;
            }
        }

        /// <summary>
        /// Set the <see cref="FieldsComparerConfiguration.OnlyCompareInitAndReadonlyFields"/> property.
        /// </summary>
        /// <returns>Self.</returns>
        public FieldsComparerConfiguration CompareOnlyInitAndReadonlyFields()
        {
            OnlyCompareInitAndReadonlyFields = true;
            return this;
        }

        /// <summary>
        /// Specify a tolerance for all numeric comparisons.
        /// </summary>
        /// <param name="amount">The tolerance to apply.</param>
        /// <returns>Self.</returns>
        public FieldsComparerConfiguration Within(object amount)
        {
            SetTolerance(amount);
            return this;
        }
    }
}
