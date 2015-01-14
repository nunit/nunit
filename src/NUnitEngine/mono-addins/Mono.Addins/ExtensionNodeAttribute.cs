//
// ExtensionNodeAttribute.cs
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
	/// This attribute can be applied to an ExtensionNode subclass to specify the default name and description.
	/// </summary>
	/// <remarks>
	/// This information will be used when an extension point does not define a name or description for a node type.
	/// </remarks>
	[AttributeUsage (AttributeTargets.Class)]
	public class ExtensionNodeAttribute: Attribute
	{
		string nodeName;
		string description;
		string customAttributeTypeName;
		Type customAttributeType;
		
		/// <summary>
		/// Initializes the attribute
		/// </summary>
		public ExtensionNodeAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes the attribute
		/// </summary>
		/// <param name="nodeName">
		/// Name of the node
		/// </param>
		public ExtensionNodeAttribute (string nodeName)
		{
			this.nodeName = nodeName;
		}
		
		/// <summary>
		/// Initializes the attribute
		/// </summary>
		/// <param name="nodeName">
		/// Name of the node
		/// </param>
		/// <param name="description">
		/// Description of the node
		/// </param>
		public ExtensionNodeAttribute (string nodeName, string description)
		{
			this.nodeName = nodeName;
			this.description = description;
		}
		
		/// <summary>
		/// Default name of the extension node
		/// </summary>
		public string NodeName {
			get { return nodeName != null ? nodeName : string.Empty; }
			set { nodeName = value; }
		}
		
		/// <summary>
		/// Default description of the extension node type
		/// </summary>
		public string Description {
			get { return description != null ? description : string.Empty; }
			set { description = value; }
		}
		
		/// <summary>
		/// Type of a custom attribute which can be used to specify metadata for this extension node type
		/// </summary>
		public Type ExtensionAttributeType {
			get { return customAttributeType; }
			set { customAttributeType = value; customAttributeTypeName = value.FullName; }
		}
		
		internal string ExtensionAttributeTypeName {
			get { return customAttributeTypeName ?? string.Empty; }
			set { customAttributeTypeName = value; }
		}
	}
}
