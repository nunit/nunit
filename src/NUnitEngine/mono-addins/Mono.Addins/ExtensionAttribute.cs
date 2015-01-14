//
// ExtensionAttribute.cs
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
	/// Declares a type extension.
	/// </summary>
	/// <remarks>
	/// When applied to a class, specifies that the class is an extension
	/// class to be registered in a matching extension point.
	/// </remarks>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple=true)]
	public class ExtensionAttribute: Attribute
	{
		string path;
		string nodeName;
		string id;
		string insertBefore;
		string insertAfter;
		string typeName;
		Type type;

		/// <summary>
		/// Initializes a new instance of the ExtensionAttribute class.
		/// </summary>
		public ExtensionAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="path">
		/// Path of the extension point.
		/// </param>
		/// <remarks>The path is only required if there are several extension points defined for the same type.</remarks>
		public ExtensionAttribute (string path)
		{
			this.path = path;
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="type">
		/// Type defining the extension point being extended
		/// </param>
		/// <remarks>
		/// This constructor can be used to explicitly specify the type that defines the extension point
		/// to be extended. By default, Mono.Addins will try to find any extension point defined in any
		/// of the base classes or interfaces. The type parameter can be used when there is more than one
		/// base type providing an extension point.
		/// </remarks>
		public ExtensionAttribute (Type type)
		{
			Type = type;
		}
		
		/// <summary>
		/// Path of the extension point being extended
		/// </summary>
		/// <remarks>
		/// The path is only required if there are several extension points defined for the same type.
		/// </remarks>
		public string Path {
			get { return path ?? string.Empty; }
			set { path = value; }
		}

		/// <summary>
		/// Name of the extension node
		/// </summary>
		/// <remarks>
		/// Extension points may require extensions to use a specific node name.
		/// This is needed when an extension point may contain several different types of nodes.
		/// </remarks>
		public string NodeName {
			get { return !string.IsNullOrEmpty (nodeName) ? nodeName : "Type"; }
			set { nodeName = value; }
		}
		
		/// <summary>
		/// Identifier of the extension node.
		/// </summary>
		/// <remarks>
		/// The ExtensionAttribute.InsertAfter and ExtensionAttribute.InsertBefore
		/// properties can be used to specify the relative location of a node. The nodes
		/// referenced in those properties must be defined either in the add-in host
		/// being extended, or in any add-in on which this add-in depends.
		/// </remarks>
		public string Id {
			get { return id ?? string.Empty; }
			set { id = value; }
		}

		/// <summary>
		/// Identifier of the extension node before which this node has to be added in the extension point.
		/// </summary>
		/// <remarks>
		/// The ExtensionAttribute.InsertAfter and ExtensionAttribute.InsertBefore
		/// properties can be used to specify the relative location of a node. The nodes
		/// referenced in those properties must be defined either in the add-in host
		/// being extended, or in any add-in on which this add-in depends.
		/// </remarks>
		public string InsertBefore {
			get { return insertBefore ?? string.Empty; }
			set { insertBefore = value; }
		}
		
		/// <summary>
		/// Identifier of the extension node after which this node has to be added in the extension point.
		/// </summary>
		public string InsertAfter {
			get { return insertAfter ?? string.Empty; }
			set { insertAfter = value; }
		}
		
		/// <summary>
		/// Type defining the extension point being extended
		/// </summary>
		/// <remarks>
		/// This property can be used to explicitly specify the type that defines the extension point
		/// to be extended. By default, Mono.Addins will try to find any extension point defined in any
		/// of the base classes or interfaces. This property can be used when there is more than one
		/// base type providing an extension point.
		/// </remarks>
		public Type Type {
			get { return type; }
			set { type = value; typeName = type.FullName; }
		}
		
		internal string TypeName {
			get { return typeName ?? string.Empty; }
			set { typeName = value; }
		}
	}
}
