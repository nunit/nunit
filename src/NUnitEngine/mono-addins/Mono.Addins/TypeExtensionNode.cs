//
// TypeExtensionNode.cs
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
using System.Xml;

namespace Mono.Addins
{
	/// <summary>
	/// An extension node which specifies a type.
	/// </summary>
	/// <remarks>
	/// This class is a kind of Mono.Addins.ExtensionNode which can be used to register
	/// types in an extension point. This is a very common case: a host application
	/// defines an interface, and add-ins create classes that implement that interface.
	/// The host will define an extension point which will use TypeExtensionNode as nodetext
	/// type. Add-ins will register the classes they implement in that extension point.
	/// 
	/// When the nodes of an extension point are of type TypeExtensionNode it is then
	/// possible to use query methods such as AddinManager.GetExtensionObjects(string),
	/// which will get all nodes in the provided extension path and will create an object
	/// for each node.
	/// 
	/// When declaring extension nodes in an add-in manifest, the class names can be
	/// specified using the 'class' or 'type' attribute. If none of those attributes is
	/// provided, the class name will be taken from the 'id' attribute.
	/// 
	/// TypeExtensionNode is the default extension type used when no type is provided
	/// in the definition of an extension point.
	/// </remarks>
	[ExtensionNode ("Type", Description="Specifies a class that will be used to create an extension object.")]
	[NodeAttribute ("class", typeof(Type), false, ContentType = ContentType.Class, Description="Name of the class. If a value is not provided, the class name will be taken from the 'id' attribute")]
	public class TypeExtensionNode: InstanceExtensionNode
	{
		string typeName;
		Type type;
		
		/// <summary>
		/// Reads the extension node data
		/// </summary>
		/// <param name='elem'>
		/// The element containing the extension data
		/// </param>
		/// <remarks>
		/// This method can be overriden to provide a custom method for reading extension node data from an element.
		/// The default implementation reads the attributes if the element and assigns the values to the fields
		/// and properties of the extension node that have the corresponding [NodeAttribute] decoration.
		/// </remarks>
		internal protected override void Read (NodeElement elem)
		{
			base.Read (elem);
			typeName = elem.GetAttribute ("type");
			if (typeName.Length == 0)
				typeName = elem.GetAttribute ("class");
			if (typeName.Length == 0)
				typeName = elem.GetAttribute ("id");
		}
		
		/// <summary>
		/// Creates a new extension object
		/// </summary>
		/// <returns>
		/// The extension object
		/// </returns>
		public override object CreateInstance ()
		{
			return Activator.CreateInstance (Type);
		}

		/// <summary>
		/// Type of the object that this node creates
		/// </summary>
		public Type Type {
			get {
				if (type == null) {
					if (typeName.Length == 0)
						throw new InvalidOperationException ("Type name not specified.");
					type = Addin.GetType (typeName, true);
				}
				return type;
			}
		}

		/// <summary>
		/// Name of the type of the object that this node creates
		/// </summary>
		/// <value>The name of the type.</value>
		public string TypeName {
			get {
				return typeName;
			}
		}
	}
	
	/// <summary>
	/// An extension node which specifies a type with custom extension metadata
	/// </summary>
	/// <remarks>
	/// This is the default type for type extension nodes bound to a custom extension attribute.
	/// </remarks>
	public class TypeExtensionNode<T>: TypeExtensionNode, IAttributedExtensionNode where T:CustomExtensionAttribute
	{
		T data;
		
		/// <summary>
		/// The custom attribute containing the extension metadata
		/// </summary>
		[NodeAttribute]
		public T Data {
			get { return data; }
			internal set { data = value; }
		}

		CustomExtensionAttribute IAttributedExtensionNode.Attribute {
			get { return data; }
		}
	}
}
