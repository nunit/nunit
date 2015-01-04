//
// AddinDependencyAttribute.cs
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
	/// Declares a dependency on an add-in or add-in host
	/// </summary>
	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple=true)]
	public class AddinDependencyAttribute: Attribute
	{
		string id;
		string version;
		
		/// <summary>
		/// Initializes the attribute
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in
		/// </param>
		/// <param name="version">
		/// Version of the add-in
		/// </param>
		public AddinDependencyAttribute (string id, string version)
		{
			this.id = id;
			this.version = version;
		}
		
		/// <summary>
		/// Identifier of the add-in
		/// </summary>
		public string Id {
			get { return id; }
		}
		
		/// <summary>
		/// Version of the add-in
		/// </summary>
		public string Version {
			get { return version; }
		}
		
	}
}
