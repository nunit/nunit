// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Helper class with properties and methods that supply
    /// constraints that operate on exceptions.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class Throws
    {
        #region Exception

        /// <summary>
        /// Creates a constraint specifying an expected exception
        /// </summary>
        public static ResolvableConstraintExpression Exception => new ConstraintExpression().Append(new ThrowsOperator());

        #endregion

        #region InnerException

        /// <summary>
        /// Creates a constraint specifying an exception with a given InnerException
        /// </summary>
        public static ResolvableConstraintExpression InnerException => Exception.InnerException;

        #endregion

        #region TargetInvocationException

        /// <summary>
        /// Creates a constraint specifying an expected TargetInvocationException
        /// </summary>
        public static ExactTypeConstraint TargetInvocationException => TypeOf(typeof(System.Reflection.TargetInvocationException));

        #endregion

        #region ArgumentException

        /// <summary>
        /// Creates a constraint specifying an expected ArgumentException
        /// </summary>
        public static ExactTypeConstraint ArgumentException => TypeOf(typeof(System.ArgumentException));

        #endregion

        #region ArgumentNullException

        /// <summary>
        /// Creates a constraint specifying an expected ArgumentNullException
        /// </summary>
        public static ExactTypeConstraint ArgumentNullException => TypeOf(typeof(System.ArgumentNullException));

        #endregion

        #region InvalidOperationException

        /// <summary>
        /// Creates a constraint specifying an expected InvalidOperationException
        /// </summary>
        public static ExactTypeConstraint InvalidOperationException => TypeOf(typeof(System.InvalidOperationException));

        #endregion

        #region Nothing

        /// <summary>
        /// Creates a constraint specifying that no exception is thrown
        /// </summary>
        public static ThrowsNothingConstraint Nothing => new();

        #endregion

        #region TypeOf

        /// <summary>
        /// Creates a constraint specifying the exact type of exception expected
        /// </summary>
        public static ExactTypeConstraint TypeOf(Type expectedType)
        {
            return Exception.TypeOf(expectedType);
        }

        /// <summary>
        /// Creates a constraint specifying the exact type of exception expected
        /// </summary>
        public static ExactTypeConstraint TypeOf<TExpected>()
            where TExpected : Exception
        {
            return TypeOf(typeof(TExpected));
        }

        #endregion

        #region InstanceOf

        /// <summary>
        /// Creates a constraint specifying the type of exception expected
        /// </summary>
        public static InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return Exception.InstanceOf(expectedType);
        }

        /// <summary>
        /// Creates a constraint specifying the type of exception expected
        /// </summary>
        public static InstanceOfTypeConstraint InstanceOf<TExpected>()
            where TExpected : Exception
        {
            return InstanceOf(typeof(TExpected));
        }

        #endregion

    }
}
