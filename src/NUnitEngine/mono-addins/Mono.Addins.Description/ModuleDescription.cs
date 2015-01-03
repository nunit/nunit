//
// ModuleDescription.cs
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Mono.Addins.Serialization;

namespace Mono.Addins.Description
{
	/// <summary>
	/// A module definition.
	/// </summary>
	/// <remarks>
	/// Optional modules can be used to declare extensions which will be registered only if some
	/// specified add-in dependencies can be satisfied.
	/// </remarks>
	public class ModuleDescription: ObjectDescription
	{
		StringCollection assemblies;
		StringCollection dataFiles;
		StringCollection ignorePaths;
		DependencyCollection dependencies;
		ExtensionCollection extensions;
		
		// Used only at run time
		internal RuntimeAddin RuntimeAddin;
		
		internal ModuleDescription (XmlElement element)
		{
			Element = element;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.ModuleDescription"/> class.
		/// </summary>
		public ModuleDescription ()
		{
		}

		internal void MergeWith (ModuleDescription module)
		{
			Dependencies.AddRange (module.Dependencies);
			Extensions.AddRange (module.Extensions);
		}

		/// <summary>
		/// Checks if this module depends on the specified add-in.
		/// </summary>
		/// <returns>
		/// <c>true</c> if there is a dependency.
		/// </returns>
		/// <param name='addinId'>
		/// Identifier of the add-in
		/// </param>
		public bool DependsOnAddin (string addinId)
		{
			AddinDescription desc = Parent as AddinDescription;
			if (desc == null)
				throw new InvalidOperationException ();
			
			foreach (Dependency dep in Dependencies) {
				AddinDependency adep = dep as AddinDependency;
				if (adep == null) continue;
				if (Addin.GetFullId (desc.Namespace, adep.AddinId, adep.Version) == addinId)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the list of paths to be ignored by the add-in scanner.
		/// </summary>
		public StringCollection IgnorePaths {
			get {
				if (ignorePaths == null)
					ignorePaths = new StringCollection ();
				return ignorePaths;
			}
		}
		
		/// <summary>
		/// Gets all external files
		/// </summary>
		/// <value>
		/// All files.
		/// </value>
		/// <remarks>
		/// External files are data files and assemblies explicitly referenced in the Runtime section of the add-in manifest.
		/// </remarks>
		public StringCollection AllFiles {
			get {
				StringCollection col = new StringCollection ();
				foreach (string s in Assemblies)
					col.Add (s);

				foreach (string d in DataFiles)
					col.Add (d);
					
				return col;
			}
		}
		
		/// <summary>
		/// Gets the list of external assemblies used by this module.
		/// </summary>
		public StringCollection Assemblies {
			get {
				if (assemblies == null) {
					if (Element != null)
						InitCollections ();
					else
						assemblies = new StringCollection ();
				}
				return assemblies;
			}
		}
		
		/// <summary>
		/// Gets the list of external data files used by this module
		/// </summary>
		public StringCollection DataFiles {
			get {
				if (dataFiles == null) {
					if (Element != null)
						InitCollections ();
					else
						dataFiles = new StringCollection ();
				}
				return dataFiles;
			}
		}
		
		/// <summary>
		/// Gets the dependencies of this module
		/// </summary>
		public DependencyCollection Dependencies {
			get {
				if (dependencies == null) {
					dependencies = new DependencyCollection (this);
					if (Element != null) {
						XmlNodeList elems = Element.SelectNodes ("Dependencies/*");

						foreach (XmlNode node in elems) {
							XmlElement elem = node as XmlElement;
							if (elem == null) continue;
							
							if (elem.Name == "Addin") {
								AddinDependency dep = new AddinDependency (elem);
								dependencies.Add (dep);
							} else if (elem.Name == "Assembly") {
								AssemblyDependency dep = new AssemblyDependency (elem);
								dependencies.Add (dep);
							}
						}
					}
				}
				return dependencies;
			}
		}
		
		/// <summary>
		/// Gets the extensions of this module
		/// </summary>
		public ExtensionCollection Extensions {
			get {
				if (extensions == null) {
					extensions = new ExtensionCollection (this);
					if (Element != null) {
						foreach (XmlElement elem in Element.SelectNodes ("Extension"))
							extensions.Add (new Extension (elem));
					}
				}
				return extensions;
			}
		}
		
		/// <summary>
		/// Adds an extension node to the module.
		/// </summary>
		/// <returns>
		/// The extension node.
		/// </returns>
		/// <param name='path'>
		/// Path that identifies the extension point.
		/// </param>
		/// <param name='nodeName'>
		/// Node name.
		/// </param>
		/// <remarks>
		/// This method creates a new Extension object for the provided path if none exist.
		/// </remarks>
		public ExtensionNodeDescription AddExtensionNode (string path, string nodeName)
		{
			ExtensionNodeDescription node = new ExtensionNodeDescription (nodeName);
			GetExtension (path).ExtensionNodes.Add (node);
			return node;
		}
		
		/// <summary>
		/// Gets an extension instance.
		/// </summary>
		/// <returns>
		/// The extension instance.
		/// </returns>
		/// <param name='path'>
		/// Path that identifies the extension point that the extension extends.
		/// </param>
		/// <remarks>
		/// This method creates a new Extension object for the provided path if none exist.
		/// </remarks>
		public Extension GetExtension (string path)
		{
			foreach (Extension e in Extensions) {
				if (e.Path == path)
					return e;
			}
			Extension ex = new Extension (path);
			Extensions.Add (ex);
			return ex;
		}
		
		internal override void SaveXml (XmlElement parent)
		{
			CreateElement (parent, "Module");
			
			if (assemblies != null || dataFiles != null || ignorePaths != null) {
				XmlElement runtime = GetRuntimeElement ();
				
				while (runtime.FirstChild != null)
					runtime.RemoveChild (runtime.FirstChild);
					
				if (assemblies != null) {
					foreach (string s in assemblies) {
						XmlElement asm = Element.OwnerDocument.CreateElement ("Import");
						asm.SetAttribute ("assembly", s);
						runtime.AppendChild (asm);
					}
				}
				if (dataFiles != null) {
					foreach (string s in dataFiles) {
						XmlElement asm = Element.OwnerDocument.CreateElement ("Import");
						asm.SetAttribute ("file", s);
						runtime.AppendChild (asm);
					}
				}
				if (ignorePaths != null) {
					foreach (string s in ignorePaths) {
						XmlElement asm = Element.OwnerDocument.CreateElement ("ScanExclude");
						asm.SetAttribute ("path", s);
						runtime.AppendChild (asm);
					}
				}
				runtime.AppendChild (Element.OwnerDocument.CreateTextNode ("\n"));
			}
			
			// Save dependency information
			
			if (dependencies != null) {
				XmlElement deps = GetDependenciesElement ();
				dependencies.SaveXml (deps);
				deps.AppendChild (Element.OwnerDocument.CreateTextNode ("\n"));
				
				if (extensions != null)
					extensions.SaveXml (Element);
			}
		}
		
		/// <summary>
		/// Adds an add-in reference (there is a typo in the method name)
		/// </summary>
		/// <param name='id'>
		/// Identifier of the add-in.
		/// </param>
		/// <param name='version'>
		/// Version of the add-in.
		/// </param>
		public void AddAssemblyReference (string id, string version)
		{
			XmlElement deps = GetDependenciesElement ();
			if (deps.SelectSingleNode ("Addin[@id='" + id + "']") != null)
				return;
			
			XmlElement dep = Element.OwnerDocument.CreateElement ("Addin");
			dep.SetAttribute ("id", id);
			dep.SetAttribute ("version", version);
			deps.AppendChild (dep);
		}
		
		XmlElement GetDependenciesElement ()
		{
			XmlElement de = Element ["Dependencies"];
			if (de != null)
				return de;

			de = Element.OwnerDocument.CreateElement ("Dependencies");
			Element.AppendChild (de);
			return de;
		}
		
		XmlElement GetRuntimeElement ()
		{
			XmlElement de = Element ["Runtime"];
			if (de != null)
				return de;

			de = Element.OwnerDocument.CreateElement ("Runtime");
			Element.AppendChild (de);
			return de;
		}
		
		void InitCollections ()
		{
			dataFiles = new StringCollection ();
			assemblies = new StringCollection ();
			
			XmlNodeList elems = Element.SelectNodes ("Runtime/*");
			foreach (XmlElement elem in elems) {
				if (elem.LocalName == "Import") {
					string asm = elem.GetAttribute ("assembly");
					if (asm.Length > 0) {
						assemblies.Add (asm);
					} else {
						string file = elem.GetAttribute ("file");
						if (file.Length > 0)
							dataFiles.Add (file);
					}
				} else if (elem.LocalName == "ScanExclude") {
					string path = elem.GetAttribute ("path");
					if (path.Length > 0)
						IgnorePaths.Add (path);
				}
			}
		}
		
		internal override void Verify (string location, StringCollection errors)
		{
			Dependencies.Verify (location + "Module/", errors);
			Extensions.Verify (location + "Module/", errors);
		}

		internal override void Write (BinaryXmlWriter writer)
		{
			writer.WriteValue ("Assemblies", Assemblies);
			writer.WriteValue ("DataFiles", DataFiles);
			writer.WriteValue ("Dependencies", Dependencies);
			writer.WriteValue ("Extensions", Extensions);
			writer.WriteValue ("IgnorePaths", ignorePaths);
		}
		
		internal override void Read (BinaryXmlReader reader)
		{
			assemblies = (StringCollection) reader.ReadValue ("Assemblies", new StringCollection ());
			dataFiles = (StringCollection) reader.ReadValue ("DataFiles", new StringCollection ());
			dependencies = (DependencyCollection) reader.ReadValue ("Dependencies", new DependencyCollection (this));
			extensions = (ExtensionCollection) reader.ReadValue ("Extensions", new ExtensionCollection (this));
			ignorePaths = (StringCollection) reader.ReadValue ("IgnorePaths", new StringCollection ());
		}
	}
}
