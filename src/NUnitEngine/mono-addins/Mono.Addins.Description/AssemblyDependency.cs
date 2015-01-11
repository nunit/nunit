//
// AssemblyDependency.cs
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
	/// Definition of a dependency of an add-in on an assembly.
	/// </summary>
	[XmlType ("AssemblyDependency")]
	public class AssemblyDependency: Dependency
	{
		string fullName;
		string package;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.AssemblyDependency"/> class.
		/// </summary>
		public AssemblyDependency ()
		{
		}
		
		internal AssemblyDependency (XmlElement elem): base (elem)
		{
			fullName = elem.GetAttribute ("name");
			package = elem.GetAttribute ("package");
		}
		
		internal override void Verify (string location, StringCollection errors)
		{
			VerifyNotEmpty (location + "Dependencies/Assembly/", errors, "name", FullName);
		}
		
		internal override void SaveXml (XmlElement parent)
		{
			CreateElement (parent, "Assembly"); 
			Element.SetAttribute ("name", FullName);
			Element.SetAttribute ("package", Package);
		}
		
		/// <summary>
		/// Gets or sets the full name of the assembly
		/// </summary>
		/// <value>
		/// The full name of the assembly.
		/// </value>
		public string FullName {
			get { return fullName != null ? fullName : string.Empty; }
			set { fullName = value; }
		}
		
		/// <summary>
		/// Gets or sets the name of the package that provides the assembly.
		/// </summary>
		/// <value>
		/// The name of the package that provides the assembly.
		/// </value>
		public string Package {
			get { return package != null ? package : string.Empty; }
			set { package = value; }
		}
		
		/// <summary>
		/// Display name of the dependency
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public override string Name {
			get {
				if (Package.Length > 0)
					return FullName + " " + GettextCatalog.GetString ("(provided by {0})", Package);
				else
					return FullName;
			}
		}
		
		internal override bool CheckInstalled (AddinRegistry registry)
		{
			// TODO
			return true;
		}
		
		internal override void Write (BinaryXmlWriter writer)
		{
			base.Write (writer);
			writer.WriteValue ("fullName", fullName);
			writer.WriteValue ("package", package);
		}
		
		internal override void Read (BinaryXmlReader reader)
		{
			base.Read (reader);
			fullName = reader.ReadStringValue ("fullName");
			package = reader.ReadStringValue ("package");
		}
	}
}
