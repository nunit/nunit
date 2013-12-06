// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.Engine
{
	public class RecentFileEntry
	{
		public static readonly char Separator = ',';

		private string path;
		
		private Version clrVersion;

		public RecentFileEntry( string path )
		{
			this.path = path;
			this.clrVersion = Environment.Version;
		}

		public RecentFileEntry( string path, Version clrVersion )
		{
			this.path = path;
			this.clrVersion = clrVersion;
		}

		public string Path
		{
			get { return path; }
		}

		public Version CLRVersion
		{
			get { return clrVersion; }
		}

		public bool Exists
		{
			get { return path != null && System.IO.File.Exists( path ); }
		}

		public bool IsCompatibleCLRVersion
		{
			get { return clrVersion.Major <= Environment.Version.Major; }
		}

		public override string ToString()
		{
			return Path + Separator + CLRVersion.ToString();
		}

		public static RecentFileEntry Parse( string text )
		{
			int sepIndex = text.LastIndexOf( Separator );

			if ( sepIndex > 0 )
				try
				{
					return new RecentFileEntry( text.Substring( 0, sepIndex ), 
						new Version( text.Substring( sepIndex + 1 ) ) );
				}
				catch
				{
					//The last part was not a version, so fall through and return the whole text
				}
			
			return new RecentFileEntry( text );
		}
	}
}
