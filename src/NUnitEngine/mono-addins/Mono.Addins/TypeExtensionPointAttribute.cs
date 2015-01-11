//
// TypeExtensionPointAttribute.cs
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
	/// Declares an extension point bound to a type
	/// </summary>
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=true)]
	public class TypeExtensionPointAttribute: Attribute
	{
		string path;
		string nodeName;
		Type nodeType;
		string nodeTypeName;
		string desc;
		string name;
		Type customAttributeType;
		string customAttributeTypeName;
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		public TypeExtensionPointAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="path">
		/// Path that identifies the extension point
		/// </param>
		public TypeExtensionPointAttribute (string path)
		{
			this.path = path;
		}
		
		/// <summary>
		/// Path that identifies the extension point
		/// </summary>
		public string Path {
			get { return path != null ? path : string.Empty; }
			set { path = value; }
		}
		
		/// <summary>
		/// Description of the extension point.
		/// </summary>
		public string Description {
			get { return desc != null ? desc : string.Empty; }
			set { desc = value; }
		}
		
		/// <summary>
		/// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
		/// </summary>
		public string NodeName {
			get { return nodeName != null && nodeName.Length > 0 ? nodeName : "Type"; }
			set { nodeName = value; }
		}
		
		/// <summary>
		/// Display name of the extension point.
		/// </summary>
		public string Name {
			get { return name != null ? name : string.Empty; }
			set { name = value; }
		}

		/// <summary>
		/// Type of the extension node to be created for extensions
		/// </summary>
		public Type NodeType {
			get { return nodeType != null ? nodeType : typeof(TypeExtensionNode); }
			set { nodeType = value; nodeTypeName = value.FullName; }
		}

		internal string NodeTypeName {
			get { return nodeTypeName != null ? nodeTypeName : typeof(TypeExtensionNode).FullName; }
			set { nodeTypeName = value; nodeType = null; }
		}
		
		/// <summary>
		/// Type of the custom attribute to be used to specify metadata for the extension point
		/// </summary>
		public Type ExtensionAttributeType {
			get { return this.customAttributeType; }
			set { this.customAttributeType = value; customAttributeTypeName = value.FullName; }
		}

		internal string ExtensionAttributeTypeName {
			get { return this.customAttributeTypeName; }
			set { this.customAttributeTypeName = value; }
		}
	}
}
