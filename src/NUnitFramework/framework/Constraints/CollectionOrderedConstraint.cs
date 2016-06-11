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
using System.Linq;

using NUnit.Framework.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionOrderedConstraint is used to test whether a collection is ordered.
    /// </summary>
    public class CollectionOrderedConstraint : CollectionConstraint
    {
        private ComparisonAdapter comparer = ComparisonAdapter.Default;
        private string comparerName;
        private Dictionary<string, bool> properties;
        private bool descending;

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

                if (this.properties != null && this.properties.Count != 0)
                    this.properties[this.properties.Keys.Max()] = true;

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
            if (this.properties != null && this.properties.Count != 0)
                throw new InvalidOperationException("Cannot use By on existing collection. Use ThenBy to add additional items.");

            this.properties = new Dictionary<string, bool>();
            this.properties.Add(propertyName, false);

            return this;
        }

        /// <summary>
        /// Modifies the constraint to test descending ordering by the value of
        /// a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint ByDescending(string propertyName)
        {
            if (this.properties != null && this.properties.Count != 0)
                throw new InvalidOperationException("Cannot use ByDescending on existing collection. Use ThenByDescending to add additional items.");

            this.properties = new Dictionary<string, bool>();
            this.properties.Add(propertyName, true);

            return this;
        }

        /// <summary>
        /// Modifies the constraint to test additional ordering by
        /// the value of a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint ThenBy(string propertyName)
        {
            if (this.properties == null || this.properties.Count < 1)
                throw new InvalidOperationException("Cannot use ThenBy on an empty collection.");

            this.properties.Add(propertyName, false);

            return this;
        }

        /// <summary>
        /// Modifies the constraint to test additional descending ordering by
        /// the value of a specified property and returns self.
        /// </summary>
        public CollectionOrderedConstraint ThenByDescending(string propertyName)
        {
            if (this.properties == null || this.properties.Count < 1)
                throw new InvalidOperationException("Cannot use ThenByDescending on an empty collection.");

            this.properties.Add(propertyName, true);

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
                string desc;

                if (this.properties == null || this.properties.Count == 0)
                {
                    desc = "collection ordered";

                    if (descending)
                        desc += ", descending";
                }
                else
                {
                    var first = this.properties.ElementAt(0);
                    desc = "collection ordered by " + MsgUtils.FormatValue(first.Key);

                    if (first.Value)
                        desc += ", descending";

                    for (int i = 1; i < this.properties.Count; i++)
                    {
                        var item = this.properties.ElementAt(i);
                        desc += ", then by " + MsgUtils.FormatValue(item.Key);

                        if (item.Value)
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
                            var item = this.properties.ElementAt(i);

                            var isDescending = item.Value;
                            var previousValue = previous.GetType().GetProperty(item.Key).GetValue(previous, null);
                            var currentValue = current.GetType().GetProperty(item.Key).GetValue(current, null);

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
                var first = this.properties.ElementAt(0);
                sb.Append("by " + first.Key);

                if (first.Value)
                    sb.Append(" descending");

                for (int i = 1; i < this.properties.Count; i++)
                {
                    var item = this.properties.ElementAt(i);
                    sb.Append(", then by " + item.Key);

                    if (item.Value)
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
    }
}