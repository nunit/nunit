// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using NUnit.Framework.Internal;
using NUnit.Compatibility;
using System.Collections;
using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Delegate used to delay evaluation of the actual value
    /// to be used in evaluating a constraint
    /// </summary>
    public delegate TActual ActualValueDelegate<TActual>();

    /// <summary>
    /// The Constraint class is the base of all built-in constraints
    /// within NUnit. It provides the operator overloads used to combine 
    /// constraints.
    /// </summary>
    public abstract class Constraint : IConstraint
    {
        Lazy<string> _displayName;

        #region Constructor

        /// <summary>
        /// Construct a constraint with optional arguments
        /// </summary>
        /// <param name="args">Arguments to be saved</param>
        protected Constraint(params object[] args)
        {
            Arguments = args;

            _displayName = new Lazy<string>(() =>
            {
                var type = this.GetType();
                var displayName = type.Name;
                if (type.GetTypeInfo().IsGenericType)
                    displayName = displayName.Substring(0, displayName.Length - 2);
                if (displayName.EndsWith("Constraint", StringComparison.Ordinal))
                    displayName = displayName.Substring(0, displayName.Length - 10);
                return displayName;
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public virtual string DisplayName { get { return _displayName.Value; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public virtual string Description { get; protected set; }

        /// <summary>
        /// Arguments provided to this Constraint, for use in
        /// formatting the description.
        /// </summary>
        public object[] Arguments { get; private set; }

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        public ConstraintBuilder Builder { get; set; }

        #endregion

        #region Abstract and Virtual Methods

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public abstract ConstraintResult ApplyTo<TActual>(TActual actual);

        /// <summary>
        /// Applies the constraint to an ActualValueDelegate that returns 
        /// the value to be tested. The default implementation simply evaluates 
        /// the delegate but derived classes may override it to provide for 
        /// delayed processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>A ConstraintResult</returns>
        public virtual ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
#if NET_4_0 || NET_4_5 || PORTABLE
            if (AsyncInvocationRegion.IsAsyncOperation(del))
                using (var region = AsyncInvocationRegion.Create(del))
                    return ApplyTo(region.WaitForPendingOperationsToComplete(del()));
#endif
            return ApplyTo(GetTestObject(del));
        }

#pragma warning disable 3006
        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public virtual ConstraintResult ApplyTo<TActual>(ref TActual actual)
        {
            return ApplyTo(actual);
        }
#pragma warning restore 3006

        /// <summary>
        /// Retrieves the value to be tested from an ActualValueDelegate.
        /// The default implementation simply evaluates the delegate but derived
        /// classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>Delegate evaluation result</returns>
        protected virtual object GetTestObject<TActual>(ActualValueDelegate<TActual> del)
        {
            return del();
        }

        #endregion

        #region ToString Override

        /// <summary>
        /// Default override of ToString returns the constraint DisplayName
        /// followed by any arguments within angle brackets.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string rep = GetStringRepresentation();

            return this.Builder == null ? rep : string.Format("<unresolved {0}>", rep);
        }

        /// <summary>
        /// Returns the string representation of this constraint
        /// </summary>
        protected virtual string GetStringRepresentation()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<");
            sb.Append(DisplayName.ToLower());

            foreach (object arg in Arguments)
            {
                sb.Append(" ");
                sb.Append(_displayable(arg));
            }

            sb.Append(">");

            return sb.ToString();
        }

        private static string _displayable(object o)
        {
            if (o == null) return "null";

            string fmt = o is string ? "\"{0}\"" : "{0}";
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, fmt, o);
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// This operator creates a constraint that is satisfied only if both 
        /// argument constraints are satisfied.
        /// </summary>
        public static Constraint operator &(Constraint left, Constraint right)
        {
            IResolveConstraint l = (IResolveConstraint)left;
            IResolveConstraint r = (IResolveConstraint)right;
            return new AndConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if either 
        /// of the argument constraints is satisfied.
        /// </summary>
        public static Constraint operator |(Constraint left, Constraint right)
        {
            IResolveConstraint l = (IResolveConstraint)left;
            IResolveConstraint r = (IResolveConstraint)right;
            return new OrConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if the 
        /// argument constraint is not satisfied.
        /// </summary>
        public static Constraint operator !(Constraint constraint)
        {
            IResolveConstraint r = (IResolveConstraint)constraint;
            return new NotConstraint(r.Resolve());
        }

        #endregion

        #region Binary Operators

        /// <summary>
        /// Returns a ConstraintExpression by appending And
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression And
        {
            get
            {
                ConstraintBuilder builder = this.Builder;
                if (builder == null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new AndOperator());

                return new ConstraintExpression(builder);
            }
        }

        /// <summary>
        /// Returns a ConstraintExpression by appending And
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression With
        {
            get { return this.And; }
        }

        /// <summary>
        /// Returns a ConstraintExpression by appending Or
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression Or
        {
            get
            {
                ConstraintBuilder builder = this.Builder;
                if (builder == null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new OrOperator());

                return new ConstraintExpression(builder);
            }
        }

        #endregion

        #region After Modifier

#if !PORTABLE
        /// <summary>
        /// Returns a DelayedConstraint with the specified delay time.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint After(int delayInMilliseconds)
        {
            return new DelayedConstraint(
                Builder == null ? this : Builder.Resolve(),
                delayInMilliseconds);
        }

        /// <summary>
        /// Returns a DelayedConstraint with the specified delay time
        /// and polling interval.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <param name="pollingInterval">The interval at which to test the constraint.</param>
        /// <returns></returns>
        public DelayedConstraint After(int delayInMilliseconds, int pollingInterval)
        {
            return new DelayedConstraint(
                Builder == null ? this : Builder.Resolve(),
                delayInMilliseconds,
                pollingInterval);
        }
#endif

        #endregion

        #region IResolveConstraint Members

        /// <summary>
        /// Resolves any pending operators and returns the resolved constraint.
        /// </summary>
        IConstraint IResolveConstraint.Resolve()
        {
            return Builder == null ? this : Builder.Resolve();
        }

        #endregion
    }
}