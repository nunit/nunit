// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    using System.Linq;

    /// <summary><see cref="CollectionTally"/> counts (tallies) the number of occurrences 
    /// of each object in one or more enumerations.</summary>
    public class CollectionTally
    {
        /// <summary>The result of a <see cref="CollectionTally"/>.</summary>
        public class CollectionTallyResult
        {
            /// <summary>Items that were not in the expected collection.</summary>
            public List<object> ExtraItems { get; set; }

            /// <summary>Items that were not accounted for in the expected collection.</summary>
            public List<object> MissingItems { get; set; }

            /// <summary>Constructs an empty <see cref="CollectionTallyResult"/>.</summary>
            public CollectionTallyResult()
            {
                ExtraItems = new List<object>();
                MissingItems = new List<object>();
            }
        }

        /// <summary>The result of the comparision between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                return new CollectionTallyResult()
                {
                    MissingItems = new List<object>(_missingItems.OrderBy(i => i.Value).Select(i => i.Key)),
                    ExtraItems = new List<object>(_extraItems)
                };
            }
        }

        private readonly Dictionary<object, int> _missingItems;

        private readonly List<object> _extraItems = new List<object>();

        /// <summary>Construct a CollectionTally object from a comparer and a collection.</summary>
        /// <param name="comparer">The comparer to use for equality.</param>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(NUnitEqualityComparer comparer, IEnumerable c)
        {
            _missingItems = new Dictionary<object, int>(new TallyEqualityComparer(comparer));

            var index = 0;
            foreach (object o in c)
                _missingItems.Add(o, index++);
        }

        /// <summary>Try to remove an object from the tally.</summary>
        /// <param name="o">The object to remove.</param>
        public void TryRemove(object o)
        {
            if (_missingItems.Remove(o)) return;
            _extraItems.Add(o);
        }

        /// <summary>Try to remove a set of objects from the tally.</summary>
        /// <param name="c">The objects to remove.</param>
        public void TryRemove(IEnumerable c)
        {
            foreach (object o in c)
                TryRemove(o);
        }

        private class TallyEqualityComparer : EqualityComparer<object>
        {
            private readonly NUnitEqualityComparer _comparer;
            private Tolerance _tolerance = Tolerance.Default;

            public TallyEqualityComparer(NUnitEqualityComparer comparer)
            {
                _comparer = comparer;
            }

            public override bool Equals(object x, object y)
            {
                return _comparer.AreEqual(x, y, ref _tolerance);
            }

            public override int GetHashCode(object obj)
            {
                return Default.GetHashCode(obj);
            }
        }
    }
}
