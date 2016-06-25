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
        private readonly List<OrderedPropertyInfo> properties = new List<OrderedPropertyInfo>();

        private ComparisonAdapter comparer = ComparisonAdapter.Default;
        private string comparerName;
        private bool descending;
        private bool allowBy = true;

        /// <summary>
        /// Construct a CollectionOrderedConstraint
        /// </summary>
        public CollectionOrderedConstraint()
        {
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "Ordered"; } }

        ///<summary>
        /// If used performs a reverse comparison
        ///</summary>
        public CollectionOrderedConstraint Descending
        {
            get
            {
                descending = true;

                if (!this.allowBy && this.properties != null && this.properties.Count != 0)
                {
                    this.properties[this.properties.Count - 1].SortDescending = true;
                    descending = false;
                }

                return this;
            }
        }

        /// <summary>
        /// Returns self.
        /// </summary>
        public CollectionOrderedConstraint Then
        {
            get
            {
                this.allowBy = true;
                return this;
            }
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using(IComparer comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            this.comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            this.comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            this.comparerName = comparer.GetType().FullName;
            return this;
        }

        /// <summary>
        /// Modifies the constraint to test ordering by the value of
        /// a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint By(string propertyName)
        {
            if (!this.allowBy) 
                throw new InvalidOperationException("The use of 'By' is not allowed.");

            this.properties.Add(new OrderedPropertyInfo(propertyName, this.descending));
            this.allowBy = false;

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
                string desc = string.Empty;

                if (this.properties == null || this.properties.Count == 0)
                {
                    desc = "collection ordered";

                    if (descending)
                        desc += ", descending";
                }
                else
                {
                    var first = this.properties[0];
                    desc = "collection ordered by " + MsgUtils.FormatValue(first.PropertyName);

                    if (first.SortDescending)
                        desc += ", descending";

                    for (int i = 1; i < this.properties.Count; i++)
                    {
                        var item = this.properties[i];
                        desc += ", then by " + MsgUtils.FormatValue(item.PropertyName);

                        if (item.SortDescending)
                            desc += ", descending";
                    }
                }

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
            foreach (object current in actual)
            {
                if (current == null)
                    throw new ArgumentNullException("actual", "Null value at index " + index.ToString());

                if (this.properties != null && this.properties.Count > 0)
                {
                    if (index > 0 && previous != null)
                    {
                        for (int i = 0; i < this.properties.Count; i++)
                        {
                            var item = this.properties[i];

                            var isDescending = item.SortDescending;
                            var previousValue = previous.GetType().GetProperty(item.PropertyName).GetValue(previous, null);
                            var currentValue = current.GetType().GetProperty(item.PropertyName).GetValue(current, null);

                            if (currentValue == null)
                                throw new ArgumentNullException("actual", "Null property value at index " + index.ToString());

                            int comparisonResult = comparer.Compare(previousValue, currentValue);

                            if (isDescending && comparisonResult < 0)
                                return false;
                            if (!isDescending && comparisonResult > 0)
                                return false;
                        }
                    }
                }
                else
                {
                    if (previous != null)
                    {
                        int comparisonResult = comparer.Compare(previous, current);

                        if (descending && comparisonResult < 0)
                            return false;
                        if (!descending && comparisonResult > 0)
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

            if (this.properties != null && this.properties.Count > 0)
            {
                var first = this.properties[0];
                sb.Append("by " + first.PropertyName);

                if (first.SortDescending)
                    sb.Append(" descending");

                for (int i = 1; i < this.properties.Count; i++)
                {
                    var item = this.properties[i];
                    sb.Append(", then by " + item.PropertyName);

                    if (item.SortDescending)
                        sb.Append(" descending");
                }
            }
            else
            {
                if (descending)
                    sb.Append(" descending");
            }

            if (comparerName != null)
                sb.Append(" " + comparerName);

            sb.Append(">");

            return sb.ToString();
        }

        private class OrderedPropertyInfo
        {
            public OrderedPropertyInfo(string propertyName)
            {
                PropertyName = propertyName;
            }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public OrderedPropertyInfo(string propertyName, bool sortDescending)
            {
                PropertyName = propertyName;
                SortDescending = sortDescending;
            }

            public string PropertyName { get; private set; }

            public bool SortDescending { get; set; }
        }
    }
}