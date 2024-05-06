// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Text;

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
        private readonly Lazy<string> _displayName;

        #region Constructor

        /// <summary>
        /// Construct a constraint with optional arguments
        /// </summary>
        /// <param name="args">Arguments to be saved</param>
        protected Constraint(params object?[] args)
        {
            Arguments = args;

            _displayName = new Lazy<string>(() =>
            {
                var type = GetType();
                var displayName = type.Name;
                if (type.IsGenericType)
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
        public virtual string DisplayName => _displayName.Value;

        /// <inheritdoc/>
        public abstract string Description { get; }

        /// <summary>
        /// Arguments provided to this Constraint, for use in
        /// formatting the description.
        /// </summary>
        public object?[] Arguments { get; }

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        public ConstraintBuilder? Builder { get; set; }

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
            if (AsyncToSyncAdapter.IsAsyncOperation(del))
                return ApplyTo(AsyncToSyncAdapter.Await(() => del.Invoke()));

            return ApplyTo(GetTestObject(del));
        }

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

        /// <summary>
        /// Retrieves the value to be tested from an ActualValueDelegate.
        /// The default implementation simply evaluates the delegate but derived
        /// classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>Delegate evaluation result</returns>
        protected virtual object? GetTestObject<TActual>(ActualValueDelegate<TActual> del)
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

            return Builder is null ? rep : $"<unresolved {rep}>";
        }

        /// <summary>
        /// Returns the string representation of this constraint and the passed in arguments
        /// </summary>
        protected string GetStringRepresentation(IEnumerable arguments)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<");
            sb.Append(DisplayName.ToLower());

            foreach (object? arg in arguments)
            {
                sb.Append(" ");
                sb.Append(Displayable(arg));
            }

            sb.Append(">");

            return sb.ToString();

            static string Displayable(object? o)
            {
                if (o is null)
                    return "null";
                else if (o is string s)
                    return $"\"{s}\"";
                else
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", o);
            }
        }

        /// <summary>
        /// Returns the string representation of this constraint
        /// </summary>
        protected virtual string GetStringRepresentation()
            => GetStringRepresentation(Arguments);

        #endregion

        #region Operator Overloads

        /// <summary>
        /// This operator creates a constraint that is satisfied only if both
        /// argument constraints are satisfied.
        /// </summary>
        public static Constraint operator &(Constraint left, Constraint right)
        {
            var l = (IResolveConstraint)left;
            var r = (IResolveConstraint)right;
            return new AndConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if either
        /// of the argument constraints is satisfied.
        /// </summary>
        public static Constraint operator |(Constraint left, Constraint right)
        {
            var l = (IResolveConstraint)left;
            var r = (IResolveConstraint)right;
            return new OrConstraint(l.Resolve(), r.Resolve());
        }

        /// <summary>
        /// This operator creates a constraint that is satisfied if the
        /// argument constraint is not satisfied.
        /// </summary>
        public static Constraint operator !(Constraint constraint)
        {
            var r = (IResolveConstraint)constraint;
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
                ConstraintBuilder? builder = Builder;
                if (builder is null)
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
        public ConstraintExpression With => And;

        /// <summary>
        /// Returns a ConstraintExpression by appending Or
        /// to the current constraint.
        /// </summary>
        public ConstraintExpression Or
        {
            get
            {
                ConstraintBuilder? builder = Builder;
                if (builder is null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new OrOperator());

                return new ConstraintExpression(builder);
            }
        }

        /// <summary>
        /// Returns a ConstraintExpression by appending Instead
        /// to the current constraint.
        /// </summary>
        internal ConstraintExpression Instead
        {
            get
            {
                ConstraintBuilder? builder = Builder;
                if (builder is null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }

                builder.Append(new InsteadOperator());

                return new ConstraintExpression(builder);
            }
        }

        #endregion

        #region After Modifier

        /// <summary>
        /// Returns a DelayedConstraint.WithRawDelayInterval with the specified delay time.
        /// </summary>
        /// <param name="delay">The delay, which defaults to milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint.WithRawDelayInterval After(int delay)
        {
            return new DelayedConstraint.WithRawDelayInterval(new DelayedConstraint(
                Builder is null ? this : Builder.Resolve(),
                delay));
        }

        /// <summary>
        /// Returns a DelayedConstraint with the specified delay time
        /// and polling interval.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <param name="pollingInterval">The interval at which to test the constraint, in milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint After(int delayInMilliseconds, int pollingInterval)
        {
            return new DelayedConstraint(
                Builder is null ? this : Builder.Resolve(),
                delayInMilliseconds,
                pollingInterval);
        }

        #endregion

        #region IResolveConstraint Members

        /// <summary>
        /// Resolves any pending operators and returns the resolved constraint.
        /// </summary>
        IConstraint IResolveConstraint.Resolve()
        {
            return Builder is null ? this : Builder.Resolve();
        }

        #endregion
    }
}
