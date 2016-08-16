// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="Throws.cs">
//   
// </copyright>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    using NUnit.Framework.Constraints;

    /// <summary>
    /// Helper class with properties and methods that supply
    /// constraints that operate on exceptions.
    /// </summary>
    public class Throws
    {
        #region Exception

        /// <summary>
        /// Gets a constraint specifying an expected exception
        /// </summary>
        public static ResolvableConstraintExpression Exception
        {
            get { return new ConstraintExpression().Append(new ThrowsOperator()); }
        }

        #endregion

        #region InnerException

        /// <summary>
        /// Gets a constraint specifying an exception with a given InnerException
        /// </summary>
        public static ResolvableConstraintExpression InnerException
        {
            get { return Exception.InnerException; }
        }

        #endregion

        #region TargetInvocationException

        /// <summary>
        /// Gets a constraint specifying an expected TargetInvocationException
        /// </summary>
        public static ExactTypeConstraint TargetInvocationException
        {
            get { return TypeOf(typeof(TargetInvocationException)); }
        }

        #endregion

        #region ArgumentException

        /// <summary>
        /// Gets a constraint specifying an expected ArgumentException
        /// </summary>
        public static ExactTypeConstraint ArgumentException
        {
            get { return TypeOf(typeof(ArgumentException)); }
        }

        #endregion

        #region ArgumentNullException

        /// <summary>
        /// Gets a constraint specifying an expected ArgumentNullException
        /// </summary>
        public static ExactTypeConstraint ArgumentNullException
        {
            get
            {
                return TypeOf(typeof(ArgumentNullException));
            }
        }

        #endregion

        #region InvalidOperationException

        /// <summary>
        /// Gets a constraint specifying an expected InvalidOperationException
        /// </summary>
        public static ExactTypeConstraint InvalidOperationException
        {
            get { return TypeOf(typeof(InvalidOperationException)); }
        }

        #endregion

        #region Nothing

        /// <summary>
        /// Gets a constraint specifying that no exception is thrown
        /// </summary>
        public static ThrowsNothingConstraint Nothing
        {
            get { return new ThrowsNothingConstraint(); }
        }

        #endregion

        #region TypeOf

        /// <summary>
        /// Creates a constraint specifying the exact type of exception expected
        /// </summary>
        /// <param name="expectedType">
        /// The expected Type.
        /// </param>
        /// <returns>
        /// The <see cref="ExactTypeConstraint"/>.
        /// </returns>
        public static ExactTypeConstraint TypeOf(Type expectedType)
        {
            return Exception.TypeOf(expectedType);
        }

        /// <summary>
        /// Creates a constraint specifying the exact type of exception expected
        /// </summary>
        /// <typeparam name="TExpected">The expected exception type</typeparam>
        /// <returns>
        /// The <see cref="ExactTypeConstraint"/>.
        /// </returns>
        public static ExactTypeConstraint TypeOf<TExpected>()
        {
            return TypeOf(typeof(TExpected));
        }

        #endregion

        #region InstanceOf

        /// <summary>
        /// Creates a constraint specifying the type of exception expected
        /// </summary>
        /// <param name="expectedType">
        /// The expected Type.
        /// </param>
        /// <returns>
        /// The <see cref="InstanceOfTypeConstraint"/>.
        /// </returns>
        public static InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return Exception.InstanceOf(expectedType);
        }

        /// <summary>
        /// Creates a constraint specifying the type of exception expected
        /// </summary>
        /// <typeparam name="TExpected">The expected exception type</typeparam>
        /// <returns>
        /// The <see cref="InstanceOfTypeConstraint"/>.
        /// </returns>
        public static InstanceOfTypeConstraint InstanceOf<TExpected>()
        {
            return InstanceOf(typeof(TExpected));
        }

        #endregion
    }
}
