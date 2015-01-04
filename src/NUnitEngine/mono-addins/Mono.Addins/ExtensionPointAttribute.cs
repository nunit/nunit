//
// ExtensionPointAttribute.cs
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
	/// Declares an extension point.
	/// </summary>
	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple=true)]
	public class ExtensionPointAttribute: Attribute
	{
		string path;
		Type nodeType;
		string nodeName;
		string desc;
		string name;
		Type objectType;
		string nodeTypeName;
		string objectTypeName;
		Type customAttributeType;
		string customAttributeTypeName;
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		public ExtensionPointAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="path">
		/// Extension path that identifies the extension point
		/// </param>
		public ExtensionPointAttribute (string path)
		{
			this.path = path;
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="path">
		/// Extension path that identifies the extension point
		/// </param>
		/// <param name="nodeType">
		/// Type of the extension node to be created for extensions
		/// </param>
		public ExtensionPointAttribute (string path, Type nodeType)
		{
			this.path = path;
			this.nodeType = nodeType;
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="path">
		/// Extension path that identifies the extension point
		/// </param>
		/// <param name="nodeName">
		/// Element name to be used when defining an extension in an XML manifest.
		/// </param>
		/// <param name="nodeType">
		/// Type of the extension node to be created for extensions
		/// </param>
		public ExtensionPointAttribute (string path, string nodeName, Type nodeType)
		{
			this.path = path;
			this.nodeType = nodeType;
			this.nodeName = nodeName;
		}
		
		/// <summary>
		/// Extension path that identifies the extension point
		/// </summary>
		public string Path {
			get { return path != null ? path : string.Empty; }
			set { path = value; }
		}
		
		/// <summary>
		/// Long description of the extension point.
		/// </summary>
		public string Description {
			get { return desc != null ? desc : string.Empty; }
			set { desc = value; }
		}
		
		/// <summary>
		/// Type of the extension node to be created for extensions
		/// </summary>
		public Type NodeType {
			get { return nodeType != null ? nodeType : typeof(TypeExtensionNode); }
			set { nodeType = value; nodeTypeName = value.FullName; }
		}
		
		/// <summary>
		/// Expected extension object type (when nodes are of type TypeExtensionNode)
		/// </summary>
		public Type ObjectType {
			get { return objectType; }
			set { objectType = value; objectTypeName = value.FullName; }
		}
		
		internal string NodeTypeName {
			get { return nodeTypeName != null ? nodeTypeName : typeof(TypeExtensionNode).FullName; }
			set { nodeTypeName = value; }
		}
		
		internal string ObjectTypeName {
			get { return objectTypeName; }
			set { objectTypeName = value; }
		}
		
		/// <summary>
		/// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
		/// </summary>
		public string NodeName {
			get { return nodeName != null && nodeName.Length > 0 ? nodeName : string.Empty; }
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
