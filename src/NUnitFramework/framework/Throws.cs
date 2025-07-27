// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
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
        public static ExactTypeConstraint TargetInvocationException => TypeOf<TargetInvocationException>();

        #endregion

        #region ArgumentException

        /// <summary>
        /// Creates a constraint specifying an expected ArgumentException
        /// </summary>
        public static ExactTypeConstraint<ArgumentException> ArgumentException => TypeOf<ArgumentException>();

        #endregion

        #region ArgumentNullException

        /// <summary>
        /// Creates a constraint specifying an expected ArgumentNullException
        /// </summary>
        public static ExactTypeConstraint<ArgumentNullException> ArgumentNullException => TypeOf<ArgumentNullException>();

        #endregion

        #region InvalidOperationException

        /// <summary>
        /// Creates a constraint specifying an expected InvalidOperationException
        /// </summary>
        public static ExactTypeConstraint<InvalidOperationException> InvalidOperationException => TypeOf<InvalidOperationException>();

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
        public static ExactTypeConstraint<TExpected> TypeOf<TExpected>()
            where TExpected : Exception
        {
            return Exception.TypeOf<TExpected>();
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
        public static InstanceOfTypeConstraint<TExpected> InstanceOf<TExpected>()
            where TExpected : Exception
        {
            return Exception.InstanceOf<TExpected>();
        }

        #endregion

    }
}
