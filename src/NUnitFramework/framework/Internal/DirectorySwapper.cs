// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Utility class that allows changing the current directory 
	/// for the duration of some lexical scope and guaranteeing
	/// that it will be restored upon exit.
	/// 
	/// Use it as follows:
	///    using( new DirectorySwapper( @"X:\New\Path" )
	///    {
	///        // Code that operates in the new current directory
	///    }
	///    
	/// Instantiating DirectorySwapper without a path merely
	/// saves the current directory, but does not change it.
	/// </summary>
	public class DirectorySwapper : IDisposable
	{
		private string savedDirectoryName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySwapper"/> class,
        /// saving the CurrentDirectory.
        /// </summary>
		public DirectorySwapper() : this( null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySwapper"/> class,
        /// saving the CurrentDirectory and setting it to a new value.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
		public DirectorySwapper( string directoryName )
		{
			savedDirectoryName = Environment.CurrentDirectory;
			
			if ( directoryName != null && directoryName != string.Empty )
				Environment.CurrentDirectory = directoryName;
		}

        /// <summary>
        /// Restores the saved value of CurrentDirectory.
        /// </summary>
		public void Dispose()
		{
			Environment.CurrentDirectory = savedDirectoryName;
		}
	}
}
