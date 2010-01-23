// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

namespace NUnit.Core.Extensibility
{
    /// <summary>
    /// ExtensionsCollection holds multiple extensions of a common type.
    /// </summary>
	public class ExtensionsCollection : IEnumerable
	{
		private static readonly int DEFAULT_LEVEL = 0;
		private static readonly int MAX_LEVELS = 10;

		private ObjectList[] lists;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsCollection"/> class.
        /// </summary>
		public ExtensionsCollection() : this(1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsCollection"/> class.
        /// </summary>
        /// <param name="levels">The number of levels supported.</param>
		public ExtensionsCollection( int levels )
		{
			if ( levels < 1 )
				levels = 1;
			else if ( levels > MAX_LEVELS )
				levels = MAX_LEVELS;

			lists = new ObjectList[levels];
		}

        /// <summary>
        /// Gets the levels.
        /// </summary>
        /// <value>The levels.</value>
	    public int Levels
	    {
            get { return lists.Length; }   
	    }

        /// <summary>
        /// Adds the specified decorator extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
		public void Add( object extension )
		{
			Add( extension, DEFAULT_LEVEL );
		}

        /// <summary>
        /// Adds the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="level">The level.</param>
		public void Add( object extension, int level )
		{
			if ( level < 0 || level >= lists.Length )
				throw new ArgumentOutOfRangeException("level", level, 
					"Value must be between 0 and " + lists.Length );

			if ( lists[level] == null )
				lists[level] = new ObjectList();

			lists[level].Add( extension );
		}

        /// <summary>
        /// Removes the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
		public void Remove( object extension )
		{
			foreach( IList list in lists )
				list.Remove( extension );
		}

		#region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
		public IEnumerator GetEnumerator()
		{
			return new ExtensionsEnumerator( lists );
		}

        /// <summary>
        /// ExtensionsEnumerator supports iteration through an ExtensionsCollection
        /// </summary>
		public class ExtensionsEnumerator : IEnumerator
		{
			private ObjectList[] lists;
			private IEnumerator listEnum;
			private int currentLevel;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtensionsEnumerator"/> class.
            /// </summary>
            /// <param name="lists">The lists.</param>
			public ExtensionsEnumerator( ObjectList[] lists )
			{
				this.lists = lists;
				Reset();
			}

			#region IEnumerator Members

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
			public void Reset()
			{
				this.listEnum = null;
				this.currentLevel = -1;
			}

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The enumerator is positioned before the first element of the collection or after the last element.
            /// </exception>
			public object Current
			{
				get
				{
					return listEnum.Current;
				}
			}

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
			public bool MoveNext()
			{
				if ( listEnum != null && listEnum.MoveNext() )
					return true;

				while ( ++currentLevel < lists.Length )
				{
					IList list = lists[currentLevel];
					if ( list != null )
					{
						listEnum = list.GetEnumerator();
						if ( listEnum.MoveNext() )
							return true;
					}
				}

				return false;
			}

			#endregion
		}

		#endregion
	}
}
