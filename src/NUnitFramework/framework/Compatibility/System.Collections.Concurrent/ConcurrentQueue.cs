// ConcurrentQueue.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Collections.Concurrent
{

    /// <summary>
    /// Represents a thread-safe first-in, first-out collection of objects.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    /// <remarks>
    /// All public  and protected members of <see cref="ConcurrentQueue{T}"/> are thread-safe and may be used
    /// concurrently from multiple threads.
    /// </remarks>
	[System.Diagnostics.DebuggerDisplay ("Count={Count}")]
	[System.Diagnostics.DebuggerTypeProxy (typeof (CollectionDebuggerView<>))]
	internal class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, ICollection,
	                                  IEnumerable
	{
		class Node
		{
			public T Value;
			public Node Next;
		}
		
		Node head = new Node ();
		Node tail;
		int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentQueue{T}"/> class.
        /// </summary>
		public ConcurrentQueue ()
		{
			tail = head;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentQueue{T}"/>
        /// class that contains elements copied from the specified collection
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new <see
        /// cref="ConcurrentQueue{T}"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="collection"/> argument is
        /// null.</exception>
        public ConcurrentQueue (IEnumerable<T> collection): this()
		{
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T item in collection)
				Enqueue (item);
		}

        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the end of the <see
        /// cref="ConcurrentQueue{T}"/>. The value can be a null reference
        /// (Nothing in Visual Basic) for reference types.
        /// </param>
        public void Enqueue (T item)
		{
			Node node = new Node ();
			node.Value = item;
			
			Node oldTail = null;
			Node oldNext = null;
			
			bool update = false;
			while (!update) {
				oldTail = tail;
				oldNext = oldTail.Next;
				
				// Did tail was already updated ?
				if (tail == oldTail) {
					if (oldNext == null) {
						// The place is for us
						update = Interlocked.CompareExchange (ref tail.Next, node, null) == null;
					} else {
						// another Thread already used the place so give him a hand by putting tail where it should be
						Interlocked.CompareExchange (ref tail, oldNext, oldTail);
					}
				}
			}
			// At this point we added correctly our node, now we have to update tail. If it fails then it will be done by another thread
			Interlocked.CompareExchange (ref tail, node, oldTail);
			Interlocked.Increment (ref count);
		}

        /// <summary>
        /// Attempts to add an object to the <see
        /// cref="T:System.Collections.Concurrent.IProducerConsumerCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see
        /// cref="T:System.Collections.Concurrent.IProducerConsumerCollection{T}"/>. The value can be a null
        /// reference (Nothing in Visual Basic) for reference types.
        /// </param>
        /// <returns>true if the object was added successfully; otherwise, false.</returns>
        /// <remarks>For <see cref="ConcurrentQueue{T}"/>, this operation will always add the object to the
        /// end of the <see cref="ConcurrentQueue{T}"/>
        /// and return true.</remarks>
        bool IProducerConsumerCollection<T>.TryAdd (T item)
		{
			Enqueue (item);
			return true;
		}

        /// <summary>
        /// Attempts to remove and return the object at the beginning of the <see
        /// cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, <paramref name="result"/> contains the
        /// object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>true if an element was removed and returned from the beginning of the <see
        /// cref="ConcurrentQueue{T}"/>
        /// successfully; otherwise, false.</returns>
		public bool TryDequeue (out T result)
		{
			result = default (T);
			Node oldNext = null;
			bool advanced = false;

			while (!advanced) {
				Node oldHead = head;
				Node oldTail = tail;
				oldNext = oldHead.Next;
				
				if (oldHead == head) {
					// Empty case ?
					if (oldHead == oldTail) {
						// This should be false then
						if (oldNext != null) {
							// If not then the linked list is mal formed, update tail
							Interlocked.CompareExchange (ref tail, oldNext, oldTail);
							continue;
						}
						result = default (T);
						return false;
					} else {
						result = oldNext.Value;
						advanced = Interlocked.CompareExchange (ref head, oldNext, oldHead) == oldHead;
					}
				}
			}

			oldNext.Value = default (T);

			Interlocked.Decrement (ref count);

			return true;
		}

        /// <summary>
        /// Attempts to return an object from the beginning of the <see cref="ConcurrentQueue{T}"/>
        /// without removing it.
        /// </summary>
        /// <param name="result">When this method returns, <paramref name="result"/> contains an object from
        /// the beginning of the <see cref="T:System.Collections.Concurrent.ConcurrentQueue{T}"/> or an
        /// unspecified value if the operation failed.</param>
        /// <returns>true if and object was returned successfully; otherwise, false.</returns>
        public bool TryPeek (out T result)
		{
			result = default (T);
			bool update = true;
			
			while (update)
			{
				Node oldHead = head;
				Node oldNext = oldHead.Next;

				if (oldNext == null) {
					result = default (T);
					return false;
				}

				result = oldNext.Value;
				
				//check if head has been updated
				update = head != oldHead;
			}
			return true;
		}
		
		internal void Clear ()
		{
			count = 0;
			tail = head = new Node ();
		}

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator ()
		{
			return (IEnumerator)InternalGetEnumerator ();
		}

        /// <summary>
        /// Returns an enumerator that iterates through the <see
        /// cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the <see
        /// cref="ConcurrentQueue{T}"/>.</returns>
        /// <remarks>
        /// The enumeration represents a moment-in-time snapshot of the contents
        /// of the queue.  It does not reflect any updates to the collection after 
        /// <see cref="GetEnumerator"/> was called.  The enumerator is safe to use
        /// concurrently with reads from and writes to the queue.
        /// </remarks>
        public IEnumerator<T> GetEnumerator ()
		{
			return InternalGetEnumerator ();
		}
		
		IEnumerator<T> InternalGetEnumerator ()
		{
			Node my_head = head;
			while ((my_head = my_head.Next) != null) {
				yield return my_head.Value;
			}
		}

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see
        /// cref="T:System.Array"/>, starting at a particular
        /// <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the
        /// destination of the elements copied from the
        /// <see cref="T:System.Collections.Concurrent.ConcurrentBag"/>. The <see
        /// cref="T:System.Array">Array</see> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying
        /// begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference (Nothing in
        /// Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// zero.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array"/> is multidimensional. -or-
        /// <paramref name="array"/> does not have zero-based indexing. -or-
        /// <paramref name="index"/> is equal to or greater than the length of the <paramref name="array"/>
        /// -or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is
        /// greater than the available space from <paramref name="index"/> to the end of the destination
        /// <paramref name="array"/>. -or- The type of the source <see
        /// cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the
        /// destination <paramref name="array"/>.
        /// </exception>
        void ICollection.CopyTo (Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException ("array");
			if (array.Rank > 1)
				throw new ArgumentException ("The array can't be multidimensional");
			if (array.GetLowerBound (0) != 0)
				throw new ArgumentException ("The array needs to be 0-based");

			T[] dest = array as T[];
			if (dest == null)
				throw new ArgumentException ("The array cannot be cast to the collection element type", "array");
			CopyTo (dest, index);
		}

        /// <summary>
        /// Copies the <see cref="ConcurrentQueue{T}"/> elements to an existing one-dimensional <see
        /// cref="T:System.Array">Array</see>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the
        /// destination of the elements copied from the
        /// <see cref="ConcurrentQueue{T}"/>. The <see cref="T:System.Array">Array</see> must have zero-based
        /// indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying
        /// begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference (Nothing in
        /// Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> is equal to or greater than the
        /// length of the <paramref name="array"/>
        /// -or- The number of elements in the source <see cref="ConcurrentQueue{T}"/> is greater than the
        /// available space from <paramref name="index"/> to the end of the destination <paramref
        /// name="array"/>.
        /// </exception>
        public void CopyTo (T[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException ("array");
			if (index < 0)
				throw new ArgumentOutOfRangeException ("index");
			if (index >= array.Length)
				throw new ArgumentException ("index is equals or greather than array length", "index");

			IEnumerator<T> e = InternalGetEnumerator ();
			int i = index;
			while (e.MoveNext ()) {
				if (i == array.Length - index)
					throw new ArgumentException ("The number of elements in the collection exceeds the capacity of array", "array");
				array[i++] = e.Current;
			}
		}

        /// <summary>
        /// Copies the elements stored in the <see cref="ConcurrentQueue{T}"/> to a new array.
        /// </summary>
        /// <returns>A new array containing a snapshot of elements copied from the <see
        /// cref="ConcurrentQueue{T}"/>.</returns>
        public T[] ToArray ()
		{
			return new List<T> (this).ToArray ();
		}

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is
        /// synchronized with the SyncRoot.
        /// </summary>
        /// <value>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized
        /// with the SyncRoot; otherwise, false. For <see cref="ConcurrentQueue{T}"/>, this property always
        /// returns false.</value>
        bool ICollection.IsSynchronized {
			get { return false; }
		}

        /// <summary>
        /// Attempts to remove and return an object from the <see
        /// cref="T:System.Collections.Concurrent.IProducerConsumerCollection{T}"/>.
        /// </summary>
        /// <param name="item">
        /// When this method returns, if the operation was successful, <paramref name="item"/> contains the
        /// object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>true if an element was removed and returned successfully; otherwise, false.</returns>
        /// <remarks>For <see cref="ConcurrentQueue{T}"/>, this operation will attempt to remove the object
        /// from the beginning of the <see cref="ConcurrentQueue{T}"/>.
        /// </remarks>
		bool IProducerConsumerCollection<T>.TryTake (out T item)
		{
			return TryDequeue (out item);
		}
		
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="T:System.Collections.ICollection"/>. This property is not supported.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The SyncRoot property is not supported.</exception>
		object ICollection.SyncRoot {
			get { throw new NotSupportedException (); }
		}

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ConcurrentQueue{T}"/>.</value>
        /// <remarks>
        /// For determining whether the collection contains any items, use of the <see cref="IsEmpty"/>
        /// property is recommended rather than retrieving the number of items from the <see cref="Count"/>
        /// property and comparing it to 0.
        /// </remarks>
        public int Count {
			get {
				return count;
			}
		}

        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentQueue{T}"/> is empty.
        /// </summary>
        /// <value>true if the <see cref="ConcurrentQueue{T}"/> is empty; otherwise, false.</value>
        /// <remarks>
        /// For determining whether the collection contains any items, use of this property is recommended
        /// rather than retrieving the number of items from the <see cref="Count"/> property and comparing it
        /// to 0.  However, as this collection is intended to be accessed concurrently, it may be the case
        /// that another thread will modify the collection after <see cref="IsEmpty"/> returns, thus invalidating
        /// the result.
        /// </remarks>
        public bool IsEmpty {
			get {
				return count == 0;
			}
		}
	}
}
