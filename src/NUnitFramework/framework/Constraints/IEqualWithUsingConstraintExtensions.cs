// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Allows specifying a custom comparer for the <see cref="EqualConstraint"/>.
    /// </summary>
    public static class IEqualWithUsingConstraintExtensions
    {
        #region TExpected Typed Comparers

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The boolean-returning delegate to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, Func<TExpected, TExpected, bool> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        [OverloadResolutionPriority(2)]
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, IEqualityComparer<TExpected> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        [OverloadResolutionPriority(1)]
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, IComparer<TExpected> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, Comparison<TExpected> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        #endregion

        #region TExpected vs TActual Typed Comparers

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparison function to use.</param>
        /// <typeparam name="TActual">The type of the actual value.</typeparam>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public static EqualUsingConstraint<TExpected> Using<TActual, TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, Func<TActual, TExpected, bool> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected,
                (object x, object y) => comparer.Invoke((TActual)x, (TExpected)y));
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <typeparam name="TActual">The type of the actual value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        [OverloadResolutionPriority(1)]
        public static EqualUsingConstraint<TExpected> Using<TActual, TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, IComparer<TActual> comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected,
                (object x, object y) => comparer.Compare((TActual)x, (TActual)y) == 0);
        }

        #endregion

        #region Non-Generic Comparers

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer object to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, IEqualityComparer comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="constraint">The constraint to add a user comparer to.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <typeparam name="TExpected">The type of the expected value.</typeparam>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public static EqualUsingConstraint<TExpected> Using<TExpected>(this IEqualWithUsingConstraint<TExpected> constraint, IComparer comparer)
        {
            return new EqualUsingConstraint<TExpected>(constraint.Expected, comparer);
        }

        #endregion
    }
}
