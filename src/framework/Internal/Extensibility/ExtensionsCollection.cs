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

		public ExtensionsCollection() : this(1) {}

		public ExtensionsCollection( int levels )
		{
			if ( levels < 1 )
				levels = 1;
			else if ( levels > MAX_LEVELS )
				levels = MAX_LEVELS;

			lists = new ObjectList[levels];
		}

	    public int Levels
	    {
            get { return lists.Length; }   
	    }

		public void Add( object extension )
		{
			Add( extension, DEFAULT_LEVEL );
		}

		public void Add( object extension, int level )
		{
			if ( level < 0 || level >= lists.Length )
				throw new ArgumentOutOfRangeException("level", level, 
					"Value must be between 0 and " + lists.Length );

			if ( lists[level] == null )
				lists[level] = new ObjectList();

			lists[level].Add( extension );
		}

		public void Remove( object extension )
		{
			foreach( IList list in lists )
				list.Remove( extension );
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new ExtensionsEnumerator( lists );
		}

		public class ExtensionsEnumerator : IEnumerator
		{
			private ObjectList[] lists;
			private IEnumerator listEnum;
			private int currentLevel;

			public ExtensionsEnumerator( ObjectList[] lists )
			{
				this.lists = lists;
				Reset();
			}

			#region IEnumerator Members

			public void Reset()
			{
				this.listEnum = null;
				this.currentLevel = -1;
			}

			public object Current
			{
				get
				{
					return listEnum.Current;
				}
			}

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
