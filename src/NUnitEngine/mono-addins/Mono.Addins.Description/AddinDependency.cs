//
// AddinDependency.cs
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
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;
using Mono.Addins.Serialization;

namespace Mono.Addins.Description
{
	/// <summary>
	/// Definition of a dependency of an add-in on another add-in.
	/// </summary>
	[XmlType ("AddinReference")]
	public class AddinDependency: Dependency
	{
		string id;
		string version;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.AddinDependency"/> class.
		/// </summary>
		public AddinDependency ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.AddinDependency"/> class.
		/// </summary>
		/// <param name='fullId'>
		/// Full identifier of the add-in (includes version)
		/// </param>
		public AddinDependency (string fullId)
		{
			Addin.GetIdParts (fullId, out id, out version);
			id = "::" + id;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.AddinDependency"/> class.
		/// </summary>
		/// <param name='id'>
		/// Identifier of the add-in.
		/// </param>
		/// <param name='version'>
		/// Version of the add-in.
		/// </param>
		public AddinDependency (string id, string version)
		{
			this.id = id;
			this.version = version;
		}
		
		internal AddinDependency (XmlElement elem): base (elem)
		{
			id = elem.GetAttribute ("id");
			version = elem.GetAttribute ("version");
		}
		
		internal override void Verify (string location, StringCollection errors)
		{
			VerifyNotEmpty (location + "Dependencies/Addin/", errors, "id", AddinId);
			VerifyNotEmpty (location + "Dependencies/Addin/", errors, "version", Version);
		}
		
		internal override void SaveXml (XmlElement parent)
		{
			CreateElement (parent, "Addin"); 
			Element.SetAttribute ("id", AddinId);
			Element.SetAttribute ("version", Version);
		}
		
		/// <summary>
		/// Gets the full addin identifier.
		/// </summary>
		/// <value>
		/// The full addin identifier.
		/// </value>
		/// <remarks>
		/// Includes namespace and version number. For example: MonoDevelop.TextEditor,1.0
		/// </remarks>
		public string FullAddinId {
			get {
				AddinDescription desc = ParentAddinDescription;
				if (desc == null)
					return Addin.GetFullId (null, AddinId, Version);
				else
					return Addin.GetFullId (desc.Namespace, AddinId, Version);
			}
		}
		
		/// <summary>
		/// Gets or sets the addin identifier.
		/// </summary>
		/// <value>
		/// The addin identifier.
		/// </value>
		public string AddinId {
			get { return id != null ? ParseString (id) : string.Empty; }
			set { id = value; }
		}
		
		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public string Version {
			get { return version != null ? ParseString (version) : string.Empty; }
			set { version = value; }
		}
		
		/// <summary>
		/// Display name of the dependency.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public override string Name {
			get { return AddinId + " v" + Version; }
		}
		
		internal override bool CheckInstalled (AddinRegistry registry)
		{
			Addin[] addins = registry.GetAddins ();
			foreach (Addin addin in addins) {
				if (addin.Id == id && addin.SupportsVersion (version)) {
					return true;
				}
			}
			return false;
		}

		internal override void Write (BinaryXmlWriter writer)
		{
			base.Write (writer);
			writer.WriteValue ("id", ParseString (id));
			writer.WriteValue ("version", ParseString (version));
		}
		
		internal override void Read (BinaryXmlReader reader)
		{
			base.Read (reader);
			id = reader.ReadStringValue ("id");
			version = reader.ReadStringValue ("version");
		}
	}
}
