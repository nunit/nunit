//
// AddinRootAttribute.cs
//
// Author:
//   Lluis Sanchez Gual
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

namespace Mono.Addins
{
	/// <summary>
	/// Marks an assembly as being an add-in root.
	/// </summary>
	/// <remarks>
	/// An add-in root is an assembly which can be extended by add-ins.
	/// </remarks>
	[AttributeUsage (AttributeTargets.Assembly)]
	public class AddinRootAttribute: AddinAttribute
	{
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		public AddinRootAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in root
		/// </param>
		public AddinRootAttribute (string id): base (id)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in root
		/// </param>
		/// <param name="version">
		/// Version of the add-in root
		/// </param>
		public AddinRootAttribute (string id, string version): base (id, version)
		{
		}
		
	}
}
