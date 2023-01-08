// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Helper class with properties and methods that supply
    /// a number of constraints used in Asserts.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class Has
    {
        #region No

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public static ConstraintExpression No => new ConstraintExpression().Not;

        #endregion

        #region All

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them succeed.
        /// </summary>
        public static ConstraintExpression All => new ConstraintExpression().All;

        #endregion

        #region Some

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if at least one of them succeeds.
        /// </summary>
        public static ConstraintExpression Some => new ConstraintExpression().Some;

        #endregion

        #region None

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them fail.
        /// </summary>
        public static ConstraintExpression None => new ConstraintExpression().None;

        #endregion

        #region Exactly(n)
 
        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding only if a specified number of them succeed.
        /// </summary>
        public static ItemsConstraintExpression Exactly(int expectedCount)
        {
            return new ConstraintExpression().Exactly(expectedCount);
        }

        #endregion

        #region One

        /// <summary>
        /// Returns a <see cref="ItemsConstraintExpression"/> which will apply
        /// the following constraint to only one member of the collection,
        /// and fail if none or more than one match occurs.
        /// </summary>
        public static ItemsConstraintExpression One => new ConstraintExpression().Exactly(1);

        #endregion

        #region Property

        /// <summary>
        /// Returns a new PropertyConstraintExpression, which will either
        /// test for the existence of the named property on the object
        /// being tested or apply any following constraint to that property.
        /// </summary>
        public static ResolvableConstraintExpression Property(string name)
        {
            return new ConstraintExpression().Property(name);
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Length property of the object being tested.
        /// </summary>
        public static ResolvableConstraintExpression Length => Property("Length");

        #endregion

        #region Count

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Count property of the object being tested.
        /// </summary>
        public static ResolvableConstraintExpression Count => Property("Count");

        #endregion

        #region Message

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Message property of the object being tested.
        /// </summary>
        public static ResolvableConstraintExpression Message => Property("Message");

        #endregion

        #region InnerException

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the InnerException property of the object being tested.
        /// </summary>
        public static ResolvableConstraintExpression InnerException => Property("InnerException");

        #endregion

        #region Attribute
        
        /// <summary>
        /// Returns a new AttributeConstraint checking for the
        /// presence of a particular attribute on an object.
        /// </summary>
        public static ResolvableConstraintExpression Attribute(Type expectedType)
        {
            return new ConstraintExpression().Attribute(expectedType);
        }

        /// <summary>
        /// Returns a new AttributeConstraint checking for the
        /// presence of a particular attribute on an object.
        /// </summary>
        public static ResolvableConstraintExpression Attribute<T>()
        {
            return Attribute(typeof(T));
        }

        #endregion

        #region Member

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public static SomeItemsConstraint Member(object expected)
        {
            return new SomeItemsConstraint(new EqualConstraint(expected));
        }

        #endregion

        #region ItemAt

        /// <summary>
        /// Returns a new IndexerConstraintExpression, which will
        /// apply any following constraint to that indexer value.
        /// </summary>
        /// <param name="indexArgs">Index accessor values.</param>
        public static ConstraintExpression ItemAt(params object[] indexArgs)
        {
            return new ConstraintExpression().ItemAt(indexArgs);
        }

        #endregion
    }
}
