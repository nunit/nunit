//
// Dependency.cs
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
using System.Xml.Serialization;
using Mono.Addins.Serialization;
using System.Xml;

namespace Mono.Addins.Description
{
	/// <summary>
	/// Definition of an add-in dependency.
	/// </summary>
	[XmlInclude (typeof(AddinDependency))]
	public abstract class Dependency: ObjectDescription
	{
		internal Dependency (XmlElement elem): base (elem)
		{
		}
		
		internal Dependency ()
		{
		}
		
		/// <summary>
		/// Gets the display name of the dependency.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public abstract string Name { get; }
		internal abstract bool CheckInstalled (AddinRegistry registry);
	}
}
