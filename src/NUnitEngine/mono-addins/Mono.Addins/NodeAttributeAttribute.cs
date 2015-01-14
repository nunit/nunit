//
// NodeAttributeAttribute.cs
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
	/// Indicates that a field or property is bound to a node attribute
	/// </summary>
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple=true)]
	public class NodeAttributeAttribute: Attribute
	{
		string name;
		bool required;
		bool localizable;
		Type type;
		string typeName;
		string description;
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		public NodeAttributeAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		public NodeAttributeAttribute (string name)
			:this (name, false, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="description">
		/// Description of the attribute.
		/// </param>
		public NodeAttributeAttribute (string name, string description)
			:this (name, false, description)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="required">
		/// Indicates whether the attribute is required or not.
		/// </param>
		public NodeAttributeAttribute (string name, bool required)
			: this (name, required, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="required">
		/// Indicates whether the attribute is required or not.
		/// </param>
		/// <param name="description">
		/// Description of the attribute.
		/// </param>
		public NodeAttributeAttribute (string name, bool required, string description)
		{
			this.name = name;
			this.required = required;
			this.description = description;
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="type">
		/// Type of the extension node attribute.
		/// </param>
		/// <remarks>
		/// The type of the attribute is only required when applying this attribute at class level.
		/// It is not required when it is applied to a field, since the attribute type will be the type of the field.
		/// </remarks>
		public NodeAttributeAttribute (string name, Type type)
			: this (name, type, false, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="type">
		/// Type of the extension node attribute.
		/// </param>
		/// <param name="description">
		/// Description of the attribute.
		/// </param>
		/// <remarks>
		/// The type of the attribute is only required when applying this attribute at class level.
		/// It is not required when it is applied to a field, since the attribute type will be the type of the field.
		/// </remarks>
		public NodeAttributeAttribute (string name, Type type, string description)
			: this (name, type, false, description)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="type">
		/// Type of the extension node attribute.
		/// </param>
		/// <param name="required">
		/// Indicates whether the attribute is required or not.
		/// </param>
		/// <remarks>
		/// The type of the attribute is only required when applying this attribute at class level.
		/// It is not required when it is applied to a field, since the attribute type will be the type of the field.
		/// </remarks>
		public NodeAttributeAttribute (string name, Type type, bool required)
			: this (name, type, false, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">
		/// XML name of the attribute.
		/// </param>
		/// <param name="type">
		/// Type of the extension node attribute.
		/// </param>
		/// <param name="required">
		/// Indicates whether the attribute is required or not.
		/// </param>
		/// <param name="description">
		/// Description of the attribute.
		/// </param>
		/// <remarks>
		/// The type of the attribute is only required when applying this attribute at class level.
		/// It is not required when it is applied to a field, since the attribute type will be the type of the field.
		/// </remarks>
		public NodeAttributeAttribute (string name, Type type, bool required, string description)
		{
			this.name = name;
			this.type = type;
			this.required = required;
			this.description = description;
		}
		
		/// <summary>
		/// XML name of the attribute.
		/// </summary>
		/// <remarks>
		/// If the name is not specified, the field name to which the [NodeAttribute]
		/// is applied will be used as name. Providing a name is mandatory when applying
		/// [NodeAttribute] at class level.
		/// </remarks>
		public string Name {
			get { return name != null ? name : string.Empty; }
			set { name = value; }
		}
		
		/// <summary>
		/// Indicates whether the attribute is required or not.
		/// </summary>
		public bool Required {
			get { return required; }
			set { required = value; }
		}
		
		/// <summary>
		/// Type of the extension node attribute.
		/// </summary>
		/// <remarks>
		/// To be used only when applying [NodeAttribute] at class level. It is not required when it
		/// is applied to a field, since the attribute type will be the type of the field.
		/// </remarks>
		public Type Type {
			get { return type; }
			set { type = value; typeName = type.FullName; }
		}
		
		internal string TypeName {
			get { return typeName; }
			set { typeName = value; type = null; }
		}
		
		/// <summary>
		/// Description of the attribute.
		/// </summary>
		/// <remarks>
		/// To be used in the extension point documentation.
		/// </remarks>
		public string Description {
			get { return description != null ? description : string.Empty; }
			set { description = value; }
		}

		/// <summary>
		/// When set to True, the value of the field or property is expected to be a string id which
		/// will be localized by the add-in engine
		/// </summary>
		public bool Localizable {
			get { return localizable; }
			set { localizable = value; }
		}
		
		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <remarks>
		/// Allows specifying the type of the content of a string attribute.
		/// This value is for documentation purposes only.
		/// </remarks>
		public ContentType ContentType { get; set; }
	}
}
