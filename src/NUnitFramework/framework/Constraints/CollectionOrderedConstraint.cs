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
        private ComparisonAdapter _comparer = ComparisonAdapter.Default;
        private string _comparerName;
        private string _propertyName;
        private OrderDirection _direction = OrderDirection.Unspecified;

        enum OrderDirection
        {
            Unspecified,
            Ascending,
            Descending
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
                if (_direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _direction = OrderDirection.Ascending;
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
                if (_direction != OrderDirection.Unspecified)
                    throw new InvalidOperationException("Only one directional modifier may be used");
                _direction = OrderDirection.Descending;
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
            _propertyName = propertyName;
            return this;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get 
            { 
                string desc = _propertyName == null
                    ? "collection ordered"
                    : "collection ordered by "+ MsgUtils.FormatValue(_propertyName);

                if (_direction == OrderDirection.Descending)
                    desc += ", descending";

                return desc;
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
            foreach (object obj in actual)
            {
                object objToCompare = obj;
                if (obj == null)
                    throw new ArgumentNullException("actual", "Null value at index " + index.ToString());

                if (_propertyName != null)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(_propertyName);
                    objToCompare = prop.GetValue(obj, null);
                    if (objToCompare == null)
                        throw new ArgumentNullException("actual", "Null property value at index " + index.ToString());
                }

                if (previous != null)
                {
                    //int comparisonResult = comparer.Compare(al[i], al[i + 1]);
                    int comparisonResult = _comparer.Compare(previous, objToCompare);

                    if (_direction == OrderDirection.Descending && comparisonResult < 0)
                        return false;
                    if (_direction != OrderDirection.Descending && comparisonResult > 0)
                        return false;
                }

                previous = objToCompare;
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

            if (_propertyName != null)
                sb.Append("by " + _propertyName);
            if (_direction == OrderDirection.Descending)
                sb.Append(" descending");
            if (_comparerName != null)
                sb.Append(" " + _comparerName);

            sb.Append(">");

            return sb.ToString();
        }
    }
}