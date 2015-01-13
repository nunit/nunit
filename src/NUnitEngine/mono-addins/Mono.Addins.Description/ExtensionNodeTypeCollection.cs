//
// ExtensionNodeTypeCollection.cs
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
using System.Collections;

namespace Mono.Addins.Description
{
	/// <summary>
	/// A collection of node types.
	/// </summary>
	public class ExtensionNodeTypeCollection: ObjectDescriptionCollection<ExtensionNodeType>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeTypeCollection"/> class.
		/// </summary>
		public ExtensionNodeTypeCollection ()
		{
		}
		
		internal ExtensionNodeTypeCollection (object owner): base (owner)
		{
		}
		
		/// <summary>
		/// Gets the <see cref="Mono.Addins.Description.ExtensionNodeType"/> at the specified index.
		/// </summary>
		/// <param name='n'>
		/// The index.
		/// </param>
		public ExtensionNodeType this [int n] {
			get { return (ExtensionNodeType) List [n]; }
		}
		
		/// <summary>
		/// Gets the <see cref="Mono.Addins.Description.ExtensionNodeType"/> with the specified id.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		public ExtensionNodeType this [string id] {
			get {
				for (int n=0; n<List.Count; n++)
					if (((ExtensionNodeType) List [n]).Id == id)
						return (ExtensionNodeType) List [n];
				return null;
			}
		}
	}
}
