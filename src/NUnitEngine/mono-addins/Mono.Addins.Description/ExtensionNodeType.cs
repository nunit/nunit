//
// ExtensionNodeType.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Mono.Addins.Serialization;

namespace Mono.Addins.Description
{
	/// <summary>
	/// An extension node type definition.
	/// </summary>
	public sealed class ExtensionNodeType: ExtensionNodeSet
	{
		string typeName;
		string objectTypeName;
		string description;
		string addinId;
		NodeTypeAttributeCollection attributes;
		string customAttributeTypeName;
		
		// Cached clr type
		[NonSerialized]
		internal Type Type;
		
		// Cached serializable fields
		[NonSerialized]
		internal Dictionary<string,FieldData> Fields;
		
		// Cached serializable fields for the custom attribute
		[NonSerialized]
		internal Dictionary<string,FieldData> CustomAttributeFields;
		
		[NonSerialized]
		internal FieldData CustomAttributeMember;
		
		internal class FieldData {
			public MemberInfo Member;
			public bool Required;
			public bool Localizable;
			
			public void SetValue (object target, object val)
			{
				if (Member is FieldInfo)
					((FieldInfo)Member).SetValue (target, val);
				else
					((PropertyInfo)Member).SetValue (target, val, null);
			}
			
			public Type MemberType {
				get {
					return (Member is FieldInfo) ? ((FieldInfo)Member).FieldType : ((PropertyInfo)Member).PropertyType;
				}
			}
		}
		
		// Addin where this extension type is implemented
		internal string AddinId {
			get { return addinId; }
			set { addinId = value; }
		}
		
		/// <summary>
		/// Type that implements the extension node.
		/// </summary>
		/// <value>
		/// The full name of the type.
		/// </value>
		public string TypeName {
			get { return typeName != null ? typeName : string.Empty; }
			set { typeName = value; }
		}
		
		/// <summary>
		/// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
		/// </summary>
		/// <value>
		/// The name of the node.
		/// </value>
		public string NodeName {
			get { return Id; }
			set { Id = value; }
		}
		
		/// <summary>
		/// Type of the object that the extension creates (only valid for TypeNodeExtension).
		/// </summary>
		public string ObjectTypeName {
			get { return objectTypeName != null ? objectTypeName : string.Empty; }
			set { objectTypeName = value; }
		}
		
		/// <summary>
		/// Name of the custom attribute that can be used to declare nodes of this type
		/// </summary>
		public string ExtensionAttributeTypeName {
			get { return customAttributeTypeName ?? string.Empty; }
			set { customAttributeTypeName = value; }
		}
		
		/// <summary>
		/// Long description of the node type
		/// </summary>
		public string Description {
			get { return description != null ? description : string.Empty; }
			set { description = value; }
		}
		
		/// <summary>
		/// Attributes supported by the extension node type.
		/// </summary>
		public NodeTypeAttributeCollection Attributes {
			get {
				if (attributes == null) {
					attributes = new NodeTypeAttributeCollection (this);
					if (Element != null) {
						XmlElement atts = Element ["Attributes"];
						if (atts != null) {
							foreach (XmlNode node in atts.ChildNodes) {
								XmlElement e = node as XmlElement;
								if (e != null)
									attributes.Add (new NodeTypeAttribute (e));
							}
						}
					}
				}
				return attributes;
			}
		}

		internal ExtensionNodeType (XmlElement element): base (element)
		{
			XmlAttribute at = element.Attributes ["type"];
			if (at != null)
				typeName = at.Value;
			at = element.Attributes ["objectType"];
			if (at != null)
				objectTypeName = at.Value;
			at = element.Attributes ["customAttributeType"];
			if (at != null)
				customAttributeTypeName = at.Value;
			XmlElement de = element ["Description"];
			if (de != null)
				description = de.InnerText;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeType"/> class.
		/// </summary>
		public ExtensionNodeType ()
		{
		}
		
		/// <summary>
		///  Copies data from another node set 
		/// </summary>
		public void CopyFrom (ExtensionNodeType ntype)
		{
			base.CopyFrom (ntype);
			this.typeName = ntype.TypeName;
			this.objectTypeName = ntype.ObjectTypeName;
			this.description = ntype.Description;
			this.addinId = ntype.AddinId;
			Attributes.Clear ();
			foreach (NodeTypeAttribute att in ntype.Attributes) {
				NodeTypeAttribute catt = new NodeTypeAttribute ();
				catt.CopyFrom (att);
				Attributes.Add (catt);
			}
		}
			
		internal override string IdAttribute {
			get { return "name"; }
		}
		
		internal override void Verify (string location, StringCollection errors)
		{
			base.Verify (location, errors);
		}
		
		internal override void SaveXml (XmlElement parent, string nodeName)
		{
			base.SaveXml (parent, "ExtensionNode");
			
			XmlElement atts = Element ["Attributes"];
			if (Attributes.Count > 0) {
				if (atts == null) {
					atts = parent.OwnerDocument.CreateElement ("Attributes");
					Element.AppendChild (atts);
				}
				Attributes.SaveXml (atts);
			} else {
				if (atts != null)
					Element.RemoveChild (atts);
			}
			
			if (TypeName.Length > 0)
				Element.SetAttribute ("type", TypeName);
			else
				Element.RemoveAttribute ("type");
			
			if (ObjectTypeName.Length > 0)
				Element.SetAttribute ("objectType", ObjectTypeName);
			else
				Element.RemoveAttribute ("objectType");
			
			if (ExtensionAttributeTypeName.Length > 0)
				Element.SetAttribute ("customAttributeType", ExtensionAttributeTypeName);
			else
				Element.RemoveAttribute ("customAttributeType");

			SaveXmlDescription (Description);
		}
		
		internal override void Write (BinaryXmlWriter writer)
		{
			base.Write (writer);
			if (Id.Length == 0)
				Id = "Type";
			if (TypeName.Length == 0)
				typeName = "Mono.Addins.TypeExtensionNode";
			writer.WriteValue ("typeName", typeName);
			writer.WriteValue ("objectTypeName", objectTypeName);
			writer.WriteValue ("description", description);
			writer.WriteValue ("addinId", addinId);
			writer.WriteValue ("Attributes", attributes);
			writer.WriteValue ("customAttributeType", customAttributeTypeName);
		}
		
		internal override void Read (BinaryXmlReader reader)
		{
			base.Read (reader);
			typeName = reader.ReadStringValue ("typeName");
			objectTypeName = reader.ReadStringValue ("objectTypeName");
			if (!reader.IgnoreDescriptionData)
				description = reader.ReadStringValue ("description");
			addinId = reader.ReadStringValue ("addinId");
			if (!reader.IgnoreDescriptionData)
				attributes = (NodeTypeAttributeCollection) reader.ReadValue ("Attributes", new NodeTypeAttributeCollection (this));
			customAttributeTypeName = reader.ReadStringValue ("customAttributeType");
		}
	}
}
