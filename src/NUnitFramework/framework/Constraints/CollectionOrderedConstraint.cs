// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionOrderedConstraint is used to test whether a collection is ordered.
    /// </summary>
    public class CollectionOrderedConstraint : CollectionConstraint
    {
        private readonly List<OrderingStep> _steps = new List<OrderingStep>();
        // The step we are currently building
        private OrderingStep _activeStep;

        enum OrderDirection
        {
            Unspecified,
            Ascending,
            Descending
        }

        /// <summary>
        /// Construct a CollectionOrderedConstraint
        /// </summary>
        public CollectionOrderedConstraint()
        {
            // Ensure there is an active specification
            CreateNextStep(null);
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "Ordered"; } }

        ///<summary>
        /// If used performs a default ascending comparison
        ///</summary>
        public CollectionOrderedConstraint Ascending
        {
            get
            {
                if (_activeStep.Direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _activeStep.Direction = OrderDirection.Ascending;
                return this;
            }
        }

        ///<summary>
        /// If used performs a reverse comparison
        ///</summary>
        public CollectionOrderedConstraint Descending
        {
            get
            {
                if (_activeStep.Direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _activeStep.Direction = OrderDirection.Descending;
                return this;
            }
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using(IComparer comparer)
        {
            if (_activeStep.ComparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _activeStep.Comparer = ComparisonAdapter.For(comparer);
            _activeStep.ComparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
        {
            if (_activeStep.ComparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _activeStep.Comparer = ComparisonAdapter.For(comparer);
            _activeStep.ComparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
        {
            if (_activeStep.ComparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _activeStep.Comparer = ComparisonAdapter.For(comparer);
            _activeStep.ComparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to test ordering by the value of
        /// a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint By(string propertyName)
        {
            if (_activeStep.PropertyName == null)
                _activeStep.PropertyName = propertyName;
            else
                CreateNextStep(propertyName);

            return this;
        }

        /// <summary>
        /// Then signals a break between two ordering steps
        /// </summary>
        public CollectionOrderedConstraint Then
        {
            get
            {
                CreateNextStep(null);
                return this;
            }
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                string description = "collection ordered";

                int index = 0;
                foreach (var step in _steps)
                {
                    if (index++ != 0) description += " then";

                    if (step.PropertyName != null)
                        description += " by " + MsgUtils.FormatValue(step.PropertyName);

                    if (step.Direction == OrderDirection.Descending)
                        description += ", descending";
                }

                return description;
            }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));

            int breakingIndex;
            object breakingValue;
            if (!Matches(enumerable, out breakingIndex, out breakingValue))
            {
                return new CollectionOrderedConstraintResult(this, enumerable, breakingIndex, breakingValue);
            }
            else
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Success);
            }
        }

        /// <summary>
        /// Test whether the collection is ordered
        /// </summary>
        private bool Matches(IEnumerable actual, out int index, out object value)
        {
            object previous = null;
            index = 0;
            foreach (object current in actual)
            {
                value = current;

                if (previous != null)
                {
                    if (_steps[0].PropertyName != null)
                    {
                        if (current == null)
                            throw new ArgumentNullException(nameof(actual), "Null value at index " + index.ToString());

                        foreach (var step in _steps)
                        {
                            string propertyName = step.PropertyName;

                            PropertyInfo previousProp = previous.GetType().GetProperty(propertyName);
                            if (previousProp == null)
                                throw new ArgumentException($"Property {propertyName} not found at index {index - 1}", nameof(actual));
                            var previousValue = previousProp.GetValue(previous, null);

                            PropertyInfo prop = current.GetType().GetProperty(propertyName);
                            if (prop == null)
                                throw new ArgumentException($"Property {propertyName} not found at index {index}", nameof(actual));
                            var currentValue = prop.GetValue(current, null);

                            int comparisonResult = step.Comparer.Compare(previousValue, currentValue);

                            if (comparisonResult < 0)
                                if (step.Direction == OrderDirection.Descending)
                                    return false;
                                else break;

                            if (comparisonResult > 0)
                                if (step.Direction != OrderDirection.Descending)
                                    return false;
                                else break;
                        }
                    }
                    else
                    {
                        int comparisonResult = _activeStep.Comparer.Compare(previous, current);

                        if (_activeStep.Direction == OrderDirection.Descending && comparisonResult < 0)
                            return false;
                        if (_activeStep.Direction != OrderDirection.Descending && comparisonResult > 0)
                            return false;
                    }
                }

                previous = current;
                index++;
            }

            value = null;
            return true;
        }

        /// <summary>
        /// Test whether the collection is ordered
        /// </summary>
        protected override bool Matches(IEnumerable collection)
        {
            int _index;
            object _value;
            return Matches(collection, out _index, out _value);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        protected override string GetStringRepresentation()
        {
            StringBuilder sb = new StringBuilder("<ordered");

            if (_steps.Count > 0) // Should always be true
            {
                // For now, just using the first step
                // TODO: Revise format and tests that depend on it
                var step = _steps[0];
                if (step.PropertyName != null)
                    sb.Append("by " + step.PropertyName);
                if (step.Direction == OrderDirection.Descending)
                    sb.Append(" descending");
                if (step.ComparerName != null)
                    sb.Append(" " + step.ComparerName);
            }

            sb.Append(">");

            return sb.ToString();
        }

        private void CreateNextStep(string propertyName)
        {
            _activeStep = new OrderingStep(propertyName);
            _steps.Add(_activeStep);
        }

        #region Internal OrderingStep Class

        /// <summary>
        /// An OrderingStep represents one stage of the sort
        /// </summary>
        private sealed class OrderingStep
        {
            public OrderingStep(string propertyName)
            {
                PropertyName = propertyName;
                Comparer = ComparisonAdapter.Default;
            }

            public string PropertyName { get; set; }

            public OrderDirection Direction { get; set; }

            public ComparisonAdapter Comparer { get; set; }

            public string ComparerName { get; set; }
        }

        #endregion

        #region Private CollectionOrderedConstraintResult Class

        private sealed class CollectionOrderedConstraintResult : ConstraintResult
        {
            private const int MaxDisplayedItems = 10;

            private readonly int _breakingIndex;
            private readonly object _breakingValue;

            public CollectionOrderedConstraintResult(IConstraint constraint, IEnumerable actualValue, int breakingIndex, object breakingValue)
                : base(constraint, actualValue, ConstraintStatus.Failure)
            {
                _breakingIndex = breakingIndex;
                _breakingValue = breakingValue;
            }

            public override void WriteActualValueTo(MessageWriter writer)
            {
                int startIndex = Math.Max(0, _breakingIndex - MaxDisplayedItems + 2);
                var actualValueMessage = MsgUtils.FormatCollection((IEnumerable)ActualValue, startIndex, MaxDisplayedItems);
                writer.Write(actualValueMessage);
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                var nonMatchingStr = $"  Ordering breaks at index [{_breakingIndex}]:  "
                    + MsgUtils.FormatValue(_breakingValue);
                writer.WriteLine(nonMatchingStr);
            }
        }

        #endregion Private CollectionOrderedConstraintResult Class
    }
}
