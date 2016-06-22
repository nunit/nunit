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

using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionOrderedConstraint is used to test whether a collection is ordered.
    /// </summary>
    public class CollectionOrderedConstraint : CollectionConstraint
    {
        private List<OrderingStep> _steps;
        // The step we are currently building
        private OrderingStep _active;

        private ComparisonAdapter _comparer = ComparisonAdapter.Default;
        private string _comparerName;

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
            _steps = new List<OrderingStep>();

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
                if (_active.Direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _active.Direction = OrderDirection.Ascending;
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
                if (_active.Direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _active.Direction = OrderDirection.Descending;
                return this;
            }
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using(IComparer comparer)
        {
            if (_comparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _comparer = ComparisonAdapter.For(comparer);
            _comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
        {
            if (_comparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _comparer = ComparisonAdapter.For(comparer);
            _comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
        {
            if (_comparerName != null)
                throw new InvalidOperationException("Only one Using modifier may be used");
            _comparer = ComparisonAdapter.For(comparer);
            _comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to test ordering by the value of
        /// a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint By(string propertyName)
        {
            if (_active.PropertyName == null)
                _active.PropertyName = propertyName;
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
        /// Test whether the collection is ordered
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            object previous = null;
            int index = 0;
            foreach (object current in actual)
            {
                if (current == null)
                    throw new ArgumentNullException("actual", "Null value at index " + index.ToString());

                if (previous != null)
                {
                    if (_steps[0].PropertyName != null)
                    {
                        foreach (var step in _steps)
                        {
                            string propertyName = step.PropertyName;
                            PropertyInfo previousProp = previous.GetType().GetProperty(propertyName);
                            PropertyInfo prop = current.GetType().GetProperty(propertyName);
                            var previousValue = previousProp.GetValue(previous, null);
                            var currentValue = prop.GetValue(current, null);

                            if (currentValue == null)
                                throw new ArgumentException("actual", "Null property value at index " + index.ToString());

                            int comparisonResult = _comparer.Compare(previousValue, currentValue);

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
                        int comparisonResult = _comparer.Compare(previous, current);

                        if (_active.Direction == OrderDirection.Descending && comparisonResult < 0)
                            return false;
                        if (_active.Direction != OrderDirection.Descending && comparisonResult > 0)
                            return false;
                    }
                }

                previous = current;
                index++;
            }

            return true;
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
            }

            if (_comparerName != null)
                sb.Append(" " + _comparerName);

            sb.Append(">");

            return sb.ToString();
        }

        private void CreateNextStep(string propertyName)
        {
            _active = new OrderingStep(propertyName);
            _steps.Add(_active);
        }

        #region Internal Ordering Class

        /// <summary>
        /// An Ordering represents one stage of the sort
        /// </summary>
        private class OrderingStep
        {
            public OrderingStep(string propertyName)
            {
                PropertyName = propertyName;
            }

            public string PropertyName { get; set; }

            public OrderDirection Direction { get; set; }
        }

        #endregion
    }
}