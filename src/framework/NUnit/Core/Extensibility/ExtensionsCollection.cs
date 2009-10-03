// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Core.Extensibility
{
	public class ExtensionsCollection : IEnumerable
	{
		private static readonly int DEFAULT_LEVEL = 0;
		private static readonly int MAX_LEVELS = 10;

		private ArrayList[] lists;

		public ExtensionsCollection() : this(1) {}

		public ExtensionsCollection( int levels )
		{
			if ( levels < 1 )
				levels = 1;
			else if ( levels > MAX_LEVELS )
				levels = MAX_LEVELS;

			lists = new ArrayList[levels];
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
				lists[level] = new ArrayList();

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
			private ArrayList[] lists;
			private IEnumerator listEnum;
			private int currentLevel;

			public ExtensionsEnumerator( ArrayList[] lists )
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
