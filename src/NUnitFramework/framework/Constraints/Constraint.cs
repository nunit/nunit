// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

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
    public abstract class Constraint : ConstraintBase, IAsyncConstraint
    {
        #region Constructor

        /// <summary>
        /// Construct a constraint with optional arguments
        /// </summary>
        /// <param name="args">Arguments to be saved</param>
        protected Constraint(params object?[] args)
            : base(args)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        public ConstraintBuilder? Builder { get; set; }

        #endregion

        #region Abstract and Virtual Methods

        /// <inheritdoc/>
        public abstract ConstraintResult ApplyTo<TActual>(TActual actual);

        /// Applies the constraint to a Func that returns
        /// <inheritdoc/>
        /// <param name="del">A Func</param>
        public virtual ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            if (AsyncToSyncAdapter.IsAsyncOperation(del))
                return ApplyTo(AsyncToSyncAdapter.Await(TestExecutionContext.CurrentContext, () => del.Invoke()));

            return ApplyTo(GetTestObject(del));
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        [Obsolete("This was never implemented and will be removed.")]
        public virtual ConstraintResult ApplyTo<TActual>(ref TActual actual)
        {
            return ApplyTo(actual);
        }

        /// <inheritdoc/>
        public virtual async Task<ConstraintResult> ApplyToAsync<TActual>(Func<Task<TActual>> taskDel)
        {
            return ApplyTo(await taskDel());
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

        /// <inheritdoc/>
        public override string ToString()
        {
            string rep = GetStringRepresentation();

            return Builder is null ? rep : $"<unresolved {rep}>";
        }

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
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
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
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public ConstraintExpression With => And;

        /// <summary>
        /// Returns a ConstraintExpression by appending Or
        /// to the current constraint.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
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
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
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
