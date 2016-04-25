//
// HashSet.cs
//
// Authors:
//  Jb Evain  <jbevain@novell.com>
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;

// HashSet is basically implemented as a reduction of Dictionary<K, V>

namespace System.Collections.Generic {

    /// <summary>
    /// Implementation notes:
    /// This uses an array-based implementation similar to Dictionary&lt;T&gt;, using a buckets array
    /// to map hash values to the Slots array. Items in the Slots array that hash to the same value
    /// are chained together through the "next" indices. 
    /// 
    /// The capacity is always prime; so during resizing, the capacity is chosen as the next prime
    /// greater than double the last capacity. 
    /// 
    /// The underlying data structures are lazily initialized. Because of the observation that, 
    /// in practice, hashtables tend to contain only a few elements, the initial capacity is
    /// set very small (3 elements) unless the ctor with a collection is used.
    /// 
    /// The +/- 1 modifications in methods that add, check for containment, etc allow us to 
    /// distinguish a hash code of 0 from an uninitialized bucket. This saves us from having to 
    /// reset each bucket to -1 when resizing. See Contains, for example.
    /// 
    /// Set methods such as UnionWith, IntersectWith, ExceptWith, and SymmetricExceptWith modify
    /// this set.
    /// 
    /// Some operations can perform faster if we can assume "other" contains unique elements
    /// according to this equality comparer. The only times this is efficient to check is if
    /// other is a hashset. Note that checking that it's a hashset alone doesn't suffice; we
    /// also have to check that the hashset is using the same equality comparer. If other 
    /// has a different equality comparer, it will have unique elements according to its own
    /// equality comparer, but not necessarily according to ours. Therefore, to go these 
    /// optimized routes we check that other is a hashset using the same equality comparer.
    /// 
    /// A HashSet with no elements has the properties of the empty set. (See IsSubset, etc. for 
    /// special empty set checks.)
    /// 
    /// A couple of methods have a special case if other is this (e.g. SymmetricExceptWith). 
    /// If we didn't have these checks, we could be iterating over the set and modifying at
    /// the same time. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[Serializable]
	[DebuggerDisplay ("Count={Count}")]
	[DebuggerTypeProxy (typeof (CollectionDebuggerView<,>))]
	public class HashSet<T> : ICollection<T>, ISerializable, IDeserializationCallback
#if NET_4_0
							, ISet<T>
#endif
	{
		const int INITIAL_SIZE = 10;
		const float DEFAULT_LOAD_FACTOR = (90f / 100);
		const int NO_SLOT = -1;
		const int HASH_FLAG = -2147483648;

		struct Link {
			public int HashCode;
			public int Next;
		}

		// The hash table contains indices into the "links" array
		int [] table;

		Link [] links;
		T [] slots;

		// The number of slots in "links" and "slots" that
		// are in use (i.e. filled with data) or have been used and marked as
		// "empty" later on.
		int touched;

		// The index of the first slot in the "empty slots chain".
		// "Remove ()" prepends the cleared slots to the empty chain.
		// "Add ()" fills the first slot in the empty slots chain with the
		// added item (or increases "touched" if the chain itself is empty).
		int empty_slot;

		// The number of items in this set.
		int count;

		// The number of items the set can hold without
		// resizing the hash table and the slots arrays.
		int threshold;

		IEqualityComparer<T> comparer;
		SerializationInfo si;

		// The number of changes made to this set. Used by enumerators
		// to detect changes and invalidate themselves.
		int generation;

        /// <summary>
        /// Number of elements in this hashset
        /// </summary>
		public int Count {
			get { return count; }
		}

        /// <summary>
        /// </summary>
		public HashSet ()
		{
			Init (INITIAL_SIZE, null);
		}

        /// <summary>
        /// </summary>
        /// <param name="comparer"></param>
		public HashSet (IEqualityComparer<T> comparer)
		{
			Init (INITIAL_SIZE, comparer);
		}

        /// <summary>
        /// Implementation Notes:
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize 
        /// the need to resize by setting the initial capacity based on size of collection. 
        /// </summary>
        /// <param name="collection"></param>
		public HashSet (IEnumerable<T> collection) : this (collection, null)
		{
		}

        /// <summary>
        /// Implementation Notes:
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize 
        /// the need to resize by setting the initial capacity based on size of collection. 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
		public HashSet (IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			if (collection == null)
				throw new ArgumentNullException ("collection");

			int capacity = 0;
			var col = collection as ICollection<T>;
			if (col != null)
				capacity = col.Count;

			Init (capacity, comparer);
			foreach (var item in collection)
				Add (item);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
		protected HashSet (SerializationInfo info, StreamingContext context)
		{
			si = info;
		}

		void Init (int capacity, IEqualityComparer<T> comparer)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException ("capacity");

			this.comparer = comparer ?? EqualityComparer<T>.Default;
			if (capacity == 0)
				capacity = INITIAL_SIZE;

			/* Modify capacity so 'capacity' elements can be added without resizing */
			capacity = (int) (capacity / DEFAULT_LOAD_FACTOR) + 1;

			InitArrays (capacity);
			generation = 0;
		}

		void InitArrays (int size)
		{
			table = new int [size];

			links = new Link [size];
			empty_slot = NO_SLOT;

			slots = new T [size];
			touched = 0;

			threshold = (int) (table.Length * DEFAULT_LOAD_FACTOR);
			if (threshold == 0 && table.Length > 0)
				threshold = 1;
		}

		bool SlotsContainsAt (int index, int hash, T item)
		{
			int current = table [index] - 1;
			while (current != NO_SLOT) {
				Link link = links [current];
				if (link.HashCode == hash && ((hash == HASH_FLAG && (item == null || null == slots [current])) ? (item == null && null == slots [current]) : comparer.Equals (item, slots [current])))
					return true;

				current = link.Next;
			}

			return false;
		}

        /// <summary>
        /// Copy items in this hashset to array
        /// </summary>
        /// <param name="array">array to add items to</param>
		public void CopyTo (T [] array)
		{
			CopyTo (array, 0, count);
		}

        /// <summary>
        /// Copy items in this hashset to array, starting at arrayIndex
        /// </summary>
        /// <param name="array">array to add items to</param>
        /// <param name="arrayIndex">index to start at</param>
        public void CopyTo (T [] array, int arrayIndex)
		{
			CopyTo (array, arrayIndex, count);
		}

        /// <summary>
        /// Copy items in this hashset to array, starting at arrayIndex
        /// </summary>
        /// <param name="array">array to add items to</param>
        /// <param name="arrayIndex">index to start at</param>
        /// <param name="count">number of items to coppy</param>
		public void CopyTo (T [] array, int arrayIndex, int count)
		{
			if (array == null)
				throw new ArgumentNullException ("array");
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException ("arrayIndex");
			if (arrayIndex > array.Length)
				throw new ArgumentException ("index larger than largest valid index of array");
			if (array.Length - arrayIndex < count)
				throw new ArgumentException ("Destination array cannot hold the requested elements!");

			for (int i = 0, items = 0; i < touched && items < count; i++) {
				if (GetLinkHashCode (i) != 0)
					array [arrayIndex++] = slots [i];
			}
		}

		void Resize (int size)
		{
			int newSize = HashPrimeNumbers.ToPrime (size);

			// allocate new hash table and link slots array
			var newTable = new int [newSize];
			var newLinks = new Link [newSize];

			for (int i = 0; i < table.Length; i++) {
				int current = table [i] - 1;
				while (current != NO_SLOT) {
					int hashCode = newLinks [current].HashCode = GetItemHashCode (slots [current]);
					int index = (hashCode & int.MaxValue) % newSize;
					newLinks [current].Next = newTable [index] - 1;
					newTable [index] = current + 1;
					current = links [current].Next;
				}
			}

			table = newTable;
			links = newLinks;

			// allocate new data slots, copy data
			var newSlots = new T [newSize];
			Array.Copy (slots, 0, newSlots, 0, touched);
			slots = newSlots;

			threshold = (int) (newSize * DEFAULT_LOAD_FACTOR);
		}

		int GetLinkHashCode (int index)
		{
			return links [index].HashCode & HASH_FLAG;
		}

		int GetItemHashCode (T item)
		{
			if (item == null)
				return HASH_FLAG;
			return comparer.GetHashCode (item) | HASH_FLAG;
		}

        /// <summary>
        /// Add item to this HashSet. Returns bool indicating whether item was added (won't be 
        /// added if already present)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if added, false if already present</returns>
		public bool Add (T item)
		{
			int hashCode = GetItemHashCode (item);
			int index = (hashCode & int.MaxValue) % table.Length;

			if (SlotsContainsAt (index, hashCode, item))
				return false;

			if (++count > threshold) {
				Resize ((table.Length << 1) | 1);
				index = (hashCode & int.MaxValue) % table.Length;
			}

			// find an empty slot
			int current = empty_slot;
			if (current == NO_SLOT)
				current = touched++;
			else
				empty_slot = links [current].Next;

			// store the hash code of the added item,
			// prepend the added item to its linked list,
			// update the hash table
			links [current].HashCode = hashCode;
			links [current].Next = table [index] - 1;
			table [index] = current + 1;

			// store item
			slots [current] = item;

			generation++;

			return true;
		}

        /// <summary>
        /// Gets the IEqualityComparer that is used to determine equality of keys for 
        /// the HashSet.
        /// </summary>
		public IEqualityComparer<T> Comparer {
			get { return comparer; }
		}

        /// <summary>
        /// Remove all items from this set. This clears the elements but not the underlying 
        /// buckets and slots array. Follow this call by TrimExcess to release these.
        /// </summary>
		public void Clear ()
		{
			count = 0;

			Array.Clear (table, 0, table.Length);
			Array.Clear (slots, 0, slots.Length);
			Array.Clear (links, 0, links.Length);

			// empty the "empty slots chain"
			empty_slot = NO_SLOT;

			touched = 0;
			generation++;
		}

        /// <summary>
        /// Checks if this hashset contains the item
        /// </summary>
        /// <param name="item">item to check for containment</param>
        /// <returns>true if item contained; false if not</returns>
		public bool Contains (T item)
		{
			int hashCode = GetItemHashCode (item);
			int index = (hashCode & int.MaxValue) % table.Length;

			return SlotsContainsAt (index, hashCode, item);
		}

        /// <summary>
        /// Remove item from this hashset
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if removed; false if not (i.e. if the item wasn't in the HashSet)</returns>
		public bool Remove (T item)
		{
			// get first item of linked list corresponding to given key
			int hashCode = GetItemHashCode (item);
			int index = (hashCode & int.MaxValue) % table.Length;
			int current = table [index] - 1;

			// if there is no linked list, return false
			if (current == NO_SLOT)
				return false;

			// walk linked list until right slot (and its predecessor) is
			// found or end is reached
			int prev = NO_SLOT;
			do {
				Link link = links [current];
				if (link.HashCode == hashCode && ((hashCode == HASH_FLAG && (item == null || null == slots [current])) ? (item == null && null == slots [current]) : comparer.Equals (slots [current], item)))
					break;

				prev = current;
				current = link.Next;
			} while (current != NO_SLOT);

			// if we reached the end of the chain, return false
			if (current == NO_SLOT)
				return false;

			count--;

			// remove slot from linked list
			// is slot at beginning of linked list?
			if (prev == NO_SLOT)
				table [index] = links [current].Next + 1;
			else
				links [prev].Next = links [current].Next;

			// mark slot as empty and prepend it to "empty slots chain"
			links [current].Next = empty_slot;
			empty_slot = current;

			// clear slot
			links [current].HashCode = 0;
			slots [current] = default (T);

			generation++;

			return true;
		}

        /// <summary>
        /// Remove elements that match specified predicate. Returns the number of elements removed
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
		public int RemoveWhere (Predicate<T> match)
		{
			if (match == null)
				throw new ArgumentNullException ("match");

			var candidates = new List<T> ();

			foreach (var item in this)
				if (match (item)) 
					candidates.Add (item);

			foreach (var item in candidates)
				Remove (item);

			return candidates.Count;
		}

        /// <summary>
        /// Sets the capacity of this list to the size of the list (rounded up to nearest prime),
        /// unless count is 0, in which case we release references.
        /// 
        /// This method can be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and release all 
        /// memory referenced by the list, execute the following statements:
        /// 
        /// list.Clear();
        /// list.TrimExcess(); 
        /// </summary>
		public void TrimExcess ()
		{
			Resize (count);
		}

        // set operations

        /// <summary>
        /// Takes the intersection of this set with other. Modifies this set.
        /// 
        /// Implementation Notes: 
        /// We get better perf if other is a hashset using same equality comparer, because we 
        /// get constant contains check in other. Resulting cost is O(n1) to iterate over this.
        /// 
        /// If we can't go above route, iterate over the other and mark intersection by checking
        /// contains in this. Then loop over and delete any unmarked elements. Total cost is n2+n1. 
        /// 
        /// Attempts to return early based on counts alone, using the property that the 
        /// intersection of anything with the empty set is the empty set.
        /// </summary>
        /// <param name="other">enumerable with items to add </param>
        public void IntersectWith (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			var other_set = ToSet (other);

			RemoveWhere (item => !other_set.Contains (item));
		}

        /// <summary>
        /// Remove items in other from this set. Modifies this set.
        /// </summary>
        /// <param name="other">enumerable with items to remove</param>
		public void ExceptWith (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in other)
				Remove (item);
		}

        /// <summary>
        /// Checks if this set overlaps other (i.e. they share at least one item)
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if these have at least one common element; false if disjoint</returns>
		public bool Overlaps (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in other)
				if (Contains (item))
					return true;

			return false;
		}

        /// <summary>
        /// Checks if this and other contain the same elements. This is set equality: 
        /// duplicates and order are ignored
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
		public bool SetEquals (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			var other_set = ToSet (other);

			if (count != other_set.Count)
				return false;

			foreach (var item in this)
				if (!other_set.Contains (item))
					return false;

			return true;
		}

        /// <summary>
        /// Takes symmetric difference (XOR) with other and this set. Modifies this set.
        /// </summary>
        /// <param name="other">enumerable with items to XOR</param>
		public void SymmetricExceptWith (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in ToSet (other))
				if (!Add (item))
					Remove (item);
		}

		HashSet<T> ToSet (IEnumerable<T> enumerable)
		{
			var set = enumerable as HashSet<T>;
			if (set == null || !Comparer.Equals (set.Comparer))
				set = new HashSet<T> (enumerable, Comparer);

			return set;
		}

        /// <summary>
        /// Take the union of this HashSet with other. Modifies this set.
        /// 
        /// Implementation note: GetSuggestedCapacity (to increase capacity in advance avoiding 
        /// multiple resizes ended up not being useful in practice; quickly gets to the 
        /// point where it's a wasteful check.
        /// </summary>
        /// <param name="other">enumerable with items to add</param>
		public void UnionWith (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in other)
				Add (item);
		}

		bool CheckIsSubsetOf (HashSet<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in this)
				if (!other.Contains (item))
					return false;

			return true;
		}

        /// <summary>
        /// Checks if this is a subset of other.
        /// 
        /// Implementation Notes:
        /// The following properties are used up-front to avoid element-wise checks:
        /// 1. If this is the empty set, then it's a subset of anything, including the empty set
        /// 2. If other has unique elements according to this equality comparer, and this has more
        /// elements than other, then it can't be a subset.
        /// 
        /// Furthermore, if other is a hashset using the same equality comparer, we can use a 
        /// faster element-wise check.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a subset of other; false if not</returns>
		public bool IsSubsetOf (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			if (count == 0)
				return true;

			var other_set = ToSet (other);

			if (count > other_set.Count)
				return false;

			return CheckIsSubsetOf (other_set);
		}

        /// <summary>
        /// Checks if this is a proper subset of other (i.e. strictly contained in)
        /// 
        /// Implementation Notes:
        /// The following properties are used up-front to avoid element-wise checks:
        /// 1. If this is the empty set, then it's a proper subset of a set that contains at least
        /// one element, but it's not a proper subset of the empty set.
        /// 2. If other has unique elements according to this equality comparer, and this has >=
        /// the number of elements in other, then this can't be a proper subset.
        /// 
        /// Furthermore, if other is a hashset using the same equality comparer, we can use a 
        /// faster element-wise check.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a proper subset of other; false if not</returns>
		public bool IsProperSubsetOf (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			if (count == 0)
				return true;

			var other_set = ToSet (other);

			if (count >= other_set.Count)
				return false;

			return CheckIsSubsetOf (other_set);
		}

		bool CheckIsSupersetOf (HashSet<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			foreach (var item in other)
				if (!Contains (item))
					return false;

			return true;
		}

        /// <summary>
        /// Checks if this is a superset of other
        /// 
        /// Implementation Notes:
        /// The following properties are used up-front to avoid element-wise checks:
        /// 1. If other has no elements (it's the empty set), then this is a superset, even if this
        /// is also the empty set.
        /// 2. If other has unique elements according to this equality comparer, and this has less 
        /// than the number of elements in other, then this can't be a superset
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a superset of other; false if not</returns>
		public bool IsSupersetOf (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			var other_set = ToSet (other);

			if (count < other_set.Count)
				return false;

			return CheckIsSupersetOf (other_set);
		}

        /// <summary>
        /// Checks if this is a proper superset of other (i.e. other strictly contained in this)
        /// 
        /// Implementation Notes: 
        /// This is slightly more complicated than above because we have to keep track if there
        /// was at least one element not contained in other.
        /// 
        /// The following properties are used up-front to avoid element-wise checks:
        /// 1. If this is the empty set, then it can't be a proper superset of any set, even if 
        /// other is the empty set.
        /// 2. If other is an empty set and this contains at least 1 element, then this is a proper
        /// superset.
        /// 3. If other has unique elements according to this equality comparer, and other's count
        /// is greater than or equal to this count, then this can't be a proper superset
        /// 
        /// Furthermore, if other has unique elements according to this equality comparer, we can
        /// use a faster element-wise check.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a proper superset of other; false if not</returns>
		public bool IsProperSupersetOf (IEnumerable<T> other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			var other_set = ToSet (other);

			if (count <= other_set.Count)
				return false;

			return CheckIsSupersetOf (other_set);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static IEqualityComparer<HashSet<T>> CreateSetComparer ()
		{
			return HashSetEqualityComparer<T>.Instance;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
		[SecurityPermission (SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", generation);
			info.AddValue("Comparer", comparer, typeof(IEqualityComparer<T>));
			info.AddValue("Capacity", (table == null) ? 0 : table.Length);
			if (table != null) {
				T[] tableArray = new T[count];
				CopyTo(tableArray);
				info.AddValue("Elements", tableArray, typeof(T[]));
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
		public virtual void OnDeserialization (object sender)
		{
			if (si != null)
			{
				generation = (int) si.GetValue("Version", typeof(int));
				comparer = (IEqualityComparer<T>) si.GetValue("Comparer", 
									      typeof(IEqualityComparer<T>));
				int capacity = (int) si.GetValue("Capacity", typeof(int));

				empty_slot = NO_SLOT;
				if (capacity > 0) {
					InitArrays(capacity);

					T[] tableArray = (T[]) si.GetValue("Elements", typeof(T[]));
					if (tableArray == null) 
						throw new SerializationException("Missing Elements");

					for (int iElement = 0; iElement < tableArray.Length; iElement++) {
						Add(tableArray[iElement]);
					}
				} else 
					table = null;

				si = null;
			}
		}


		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return new Enumerator (this);
		}

        /// <summary>
        /// Whether this is readonly
        /// </summary>
		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

        /// <summary>
        /// Add item to this hashset. This is the explicit implementation of the ICollection&lt;T&gt;
        /// interface. The other Add method returns bool indicating whether item was added.
        /// </summary>
        /// <param name="item">item to add</param>
		void ICollection<T>.Add (T item)
		{
			Add (item);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new Enumerator (this);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Enumerator GetEnumerator ()
		{
			return new Enumerator (this);
		}

        /// <summary>
        /// 
        /// </summary>
		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable {

			HashSet<T> hashset;
			int next;
			int stamp;

			T current;

			internal Enumerator (HashSet<T> hashset)
				: this ()
			{
				this.hashset = hashset;
				this.stamp = hashset.generation;
			}

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
			public bool MoveNext ()
			{
				CheckState ();

				if (next < 0)
					return false;

				while (next < hashset.touched) {
					int cur = next++;
					if (hashset.GetLinkHashCode (cur) != 0) {
						current = hashset.slots [cur];
						return true;
					}
				}

				next = NO_SLOT;
				return false;
			}

            /// <summary>
            /// 
            /// </summary>
			public T Current {
				get { return current; }
			}

            /// <summary>
            /// 
            /// </summary>
			object IEnumerator.Current {
				get {
					CheckState ();
					if (next <= 0)
						throw new InvalidOperationException ("Current is not valid");
					return current;
				}
			}

            /// <summary>
            /// 
            /// </summary>
			void IEnumerator.Reset ()
			{
				CheckState ();
				next = 0;
			}

            /// <summary>
            /// 
            /// </summary>
			public void Dispose ()
			{
				hashset = null;
			}

			void CheckState ()
			{
				if (hashset == null)
					throw new ObjectDisposedException (null);
				if (hashset.generation != stamp)
					throw new InvalidOperationException ("HashSet have been modified while it was iterated over");
			}
		}
	}
	
	sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
	{
		public static readonly HashSetEqualityComparer<T> Instance = new HashSetEqualityComparer<T> ();
			
		public bool Equals (HashSet<T> lhs, HashSet<T> rhs)
		{
			if (lhs == rhs)
				return true;

			if (lhs == null || rhs == null || lhs.Count != rhs.Count)
				return false;

			foreach (var item in lhs)
				if (!rhs.Contains (item))
					return false;

			return true;
		}

		public int GetHashCode (HashSet<T> hashset)
		{
			if (hashset == null)
				return 0;

			IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
			int hash = 0;
			foreach (var item in hashset)
				hash ^= comparer.GetHashCode (item);

			return hash;
		}
	}
}
