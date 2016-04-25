//
// IOrderedEnumerable.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
//

using System.Collections.Generic;

namespace System.Linq {

    /// <summary>
    /// Represents a sorted sequence
    /// </summary>
    /// <typeparam name="TElement">The type of the elements of the sequence</typeparam>
	public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
	{
        /// <summary>
        /// Performs a subsequent ordering on the elements of an <see cref="T:System.Linq.IOrderedEnumerable`1" /> according to a key
        /// </summary>
        /// <typeparam name="TKey">The type of the key produced by <paramref name="keySelector" /></typeparam>
        /// <param name="keySelector">The <see cref="T:System.Func`2" /> used to extract the key for each element</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> used to compare keys for placement in the returned sequence</param>
        /// <param name="descending">true to sort the elements in descending order; false to sort the elements in ascending order</param>
        /// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key</returns>
		IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey> (Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
	}
}
