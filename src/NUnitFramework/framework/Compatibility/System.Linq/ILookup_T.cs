//
// ILookup<T>.cs
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
    /// Defines an indexer, size property, and Boolean search method for data structures that map keys to <see cref="T:System.Collections.Generic.IEnumerable`1" /> sequences of values
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <see cref="T:System.Linq.ILookup`2" /></typeparam>
    /// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> sequences that make up the values in the <see cref="T:System.Linq.ILookup`2" /></typeparam>
	public interface ILookup<TKey, TElement>: IEnumerable<IGrouping <TKey, TElement>> {

        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="T:System.Linq.ILookup`2" />
        /// </summary>
		int Count { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEnumerable`1" /> sequence of values indexed by a specified key
        /// </summary>
        /// <param name="key">The key of the desired sequence of values</param>
        /// <returns>The <see cref="T:System.Collections.Generic.IEnumerable`1" /> sequence of values indexed by the specified key</returns>
		IEnumerable<TElement> this [TKey key] { get; }

        /// <summary>
        /// Determines whether a specified key exists in the <see cref="T:System.Linq.ILookup`2" />
        /// </summary>
        /// <param name="key">The key to search for in the <see cref="T:System.Linq.ILookup`2" /></param>
        /// <returns>true if <paramref name="key" /> is in the <see cref="T:System.Linq.ILookup`2" />; otherwise, false</returns>
		bool Contains (TKey key);
	}
}
