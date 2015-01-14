// 
// CustomExtensionAttribute.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Mono.Addins
{
	/// <summary>
	/// Base class for custon extension attributes.
	/// </summary>
	/// <remarks>
	/// Custom extension attributes can be used to declare extensions with custom metadata.
	/// All custom extension attributes must subclass CustomExtensionAttribute.
	/// </remarks>
	public class CustomExtensionAttribute: Attribute
	{
		string id;
		string insertBefore;
		string insertAfter;
		string path;
		
		internal const string PathFieldKey = "__path";
		
		/// <summary>
		/// Identifier of the node
		/// </summary>
		[NodeAttributeAttribute ("id")]
		public string Id {
			get { return id; }
			set { id = value; }
		}
		
		/// <summary>
		/// Identifier of the node before which this node has to be placed
		/// </summary>
		[NodeAttributeAttribute ("insertbefore")]
		public string InsertBefore {
			get { return insertBefore; }
			set { insertBefore = value; }
		}
		
		/// <summary>
		/// Identifier of the node after which this node has to be placed
		/// </summary>
		[NodeAttributeAttribute ("insertafter")]
		public string InsertAfter {
			get { return insertAfter; }
			set { insertAfter = value; }
		}
		
		/// <summary>
		/// Path of the extension point being extended.
		/// </summary>
		/// <remarks>
		/// This property is optional and useful only when there are several extension points which allow
		/// using this custom attribute to define extensions.
		/// </remarks>
		[NodeAttributeAttribute ("__path")]
		public string Path {
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// The extension node bound to this attribute
		/// </summary>
		public ExtensionNode ExtensionNode { get; internal set; }


		/// <summary>
		/// The add-in that registered this extension node.
		/// </summary>
		/// <remarks>
		/// This property provides access to the resources and types of the add-in that created this extension node.
		/// </remarks>
		public RuntimeAddin Addin {
			get { return ExtensionNode.Addin; }
		}
	}
}

