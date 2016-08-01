// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections.Generic;

namespace NUnit.Engine
{
	/// <summary>
	/// The IRecentFiles interface is used to isolate the app
	/// from various implementations of recent files.
	/// </summary>
	public interface IRecentFiles
	{ 
		/// <summary>
		/// The max number of files saved
		/// </summary>
		int MaxFiles { get; set; }

		/// <summary>
		/// Get a list of all the file entries
		/// </summary>
		/// <returns>The most recent file list</returns>
		IList<string> Entries { get; }

		/// <summary>
		/// Set the most recent file name, reordering
		/// the saved names as needed and removing the oldest
		/// if the max number of files would be exceeded.
		/// The current CLR version is used to create the entry.
		/// </summary>
		void SetMostRecent( string fileName );

		/// <summary>
		/// Remove a file from the list
		/// </summary>
		/// <param name="fileName">The name of the file to remove</param>
		void Remove( string fileName );
	}
}
