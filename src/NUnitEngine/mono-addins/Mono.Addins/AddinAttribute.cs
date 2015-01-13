//
// AddinAttribute.cs
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
using Mono.Addins.Description;

namespace Mono.Addins
{
	/// <summary>
	/// Marks an assembly as being an add-in.
	/// </summary>
	[AttributeUsage (AttributeTargets.Assembly)]
	public class AddinAttribute: Attribute
	{
		string id;
		string version;
		string ns;
		string category;
		bool enabledByDefault = true;
		AddinFlags flags;
		string compatVersion;
		string url;
		
		/// <summary>
		/// Initializes an add-in marker attribute
		/// </summary>
		public AddinAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes an add-in marker attribute
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in
		/// </param>
		public AddinAttribute (string id)
		{
			this.id = id;
		}
		
		/// <summary>
		/// Initializes an add-in marker attribute
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in
		/// </param>
		/// <param name="version">
		/// Version of the add-in
		/// </param>
		public AddinAttribute (string id, string version)
		{
			this.id = id;
			this.version = version;
		}
		
		/// <summary>
		/// Identifier of the add-in.
		/// </summary>
		public string Id {
			get { return id != null ? id : string.Empty; }
			set { id = value; }
		}
		
		/// <summary>
		/// Version of the add-in.
		/// </summary>
		public string Version {
			get { return version != null ? version : string.Empty; }
			set { version = value; }
		}
		
		/// <summary>
		/// Version of the add-in with which this add-in is backwards compatible.
		/// </summary>
		public string CompatVersion {
			get { return compatVersion != null ? compatVersion : string.Empty; }
			set { compatVersion = value; }
		}
		
		/// <summary>
		/// Namespace of the add-in
		/// </summary>
		public string Namespace {
			get { return ns != null ? ns : string.Empty; }
			set { ns = value; }
		}
		
		/// <summary>
		/// Category of the add-in
		/// </summary>
		public string Category {
			get { return category != null ? category : string.Empty; }
			set { category = value; }
		}
		
		/// <summary>
		/// Url to a web page with more information about the add-in
		/// </summary>
		public string Url {
			get { return url != null ? url : string.Empty; }
			set { url = value; }
		}
		
		/// <summary>
		/// When set to True, the add-in will be automatically enabled after installing.
		/// It's True by default.
		/// </summary>
		public bool EnabledByDefault {
			get { return this.enabledByDefault; }
			set { this.enabledByDefault = value; }
		}
		
		/// <summary>
		/// Add-in flags
		/// </summary>
		public AddinFlags Flags {
			get { return this.flags; }
			set { this.flags = value; }
		}
	}
}
