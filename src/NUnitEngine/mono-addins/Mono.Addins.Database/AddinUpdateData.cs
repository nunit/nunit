//
// AddinUpdateData.cs
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
using Mono.Addins.Description;
using System.Collections.Generic;

namespace Mono.Addins.Database
{
	class AddinUpdateData
	{
		// This table collects information about extensions. Each path (key)
		// has a RootExtensionPoint object with information about the addin that
		// defines the extension point and the addins which extend it
		Dictionary<string,List<RootExtensionPoint>> pathHash = new Dictionary<string,List<RootExtensionPoint>> ();
		
		// Collects globally defined node sets. Key is node set name. Value is
		// a RootExtensionPoint
		Dictionary<string,List<RootExtensionPoint>> nodeSetHash = new Dictionary<string,List<RootExtensionPoint>> ();
		
		Dictionary<string,List<ExtensionPoint>> objectTypeExtensions = new Dictionary<string,List<ExtensionPoint>> ();
		
		Dictionary<string,List<ExtensionNodeType>> customAttributeTypeExtensions = new Dictionary<string,List<ExtensionNodeType>> ();
		
		internal int RelExtensionPoints;
		internal int RelExtensions;
		internal int RelNodeSetTypes;
		internal int RelExtensionNodes;
		
		class RootExtensionPoint
		{
			public AddinDescription Description;
			public ExtensionPoint ExtensionPoint;
		}
		
		IProgressStatus monitor;
		
		public AddinUpdateData (AddinDatabase database, IProgressStatus monitor)
		{
			this.monitor = monitor;
		}
		
		public void RegisterNodeSet (AddinDescription description, ExtensionNodeSet nset)
		{
			List<RootExtensionPoint> extensions;
			if (nodeSetHash.TryGetValue (nset.Id, out extensions)) {
				// Extension point already registered
				List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints (nset.Id, description, description.MainModule, extensions);
				if (compatExtensions.Count > 0) {
					foreach (ExtensionPoint einfo in compatExtensions)
						einfo.NodeSet.MergeWith (null, nset);
					return;
				}
			}
			// Create a new extension set
			RootExtensionPoint rep = new RootExtensionPoint ();
			rep.ExtensionPoint = new ExtensionPoint ();
			rep.ExtensionPoint.SetNodeSet (nset);
			rep.ExtensionPoint.RootAddin = description.AddinId;
			rep.ExtensionPoint.Path = nset.Id;
			rep.Description = description;
			if (extensions == null) {
				extensions = new List<RootExtensionPoint> ();
				nodeSetHash [nset.Id] = extensions;
			}
			extensions.Add (rep);
		}
		
		public void RegisterExtensionPoint (AddinDescription description, ExtensionPoint ep)
		{
			List<RootExtensionPoint> extensions;
			if (pathHash.TryGetValue (ep.Path, out extensions)) {
				// Extension point already registered
				List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints (ep.Path, description, description.MainModule, extensions);
				if (compatExtensions.Count > 0) {
					foreach (ExtensionPoint einfo in compatExtensions)
						einfo.MergeWith (null, ep);
					RegisterObjectTypes (ep);
					return;
				}
			}
			// Create a new extension
			RootExtensionPoint rep = new RootExtensionPoint ();
			rep.ExtensionPoint = ep;
			rep.ExtensionPoint.RootAddin = description.AddinId;
			rep.Description = description;
			if (extensions == null) {
				extensions = new List<RootExtensionPoint> ();
				pathHash [ep.Path] = extensions;
			}
			extensions.Add (rep);
			RegisterObjectTypes (ep);
		}
			
		void RegisterObjectTypes (ExtensionPoint ep)
		{
			// Register extension points bound to a node type
			
			foreach (ExtensionNodeType nt in ep.NodeSet.NodeTypes) {
				if (nt.ObjectTypeName.Length > 0) {
					List<ExtensionPoint> list;
					if (!objectTypeExtensions.TryGetValue (nt.ObjectTypeName, out list)) {
						list = new List<ExtensionPoint> ();
						objectTypeExtensions [nt.ObjectTypeName] = list;
					}
					list.Add (ep);
				}
				if (nt.ExtensionAttributeTypeName.Length > 0) {
					List<ExtensionNodeType> list;
					if (!customAttributeTypeExtensions.TryGetValue (nt.ExtensionAttributeTypeName, out list)) {
						list = new List<ExtensionNodeType> ();
						customAttributeTypeExtensions [nt.ExtensionAttributeTypeName] = list;
					}
					list.Add (nt);
				}
			}
		}

		public void RegisterExtension (AddinDescription description, ModuleDescription module, Extension extension)
		{
			if (extension.Path.StartsWith ("$")) {
				string[] objectTypes = extension.Path.Substring (1).Split (',');
				bool found = false;
				foreach (string s in objectTypes) {
					List<ExtensionPoint> list;
					if (objectTypeExtensions.TryGetValue (s, out list)) {
						found = true;
						foreach (ExtensionPoint ep in list) {
							if (IsAddinCompatible (ep.ParentAddinDescription, description, module)) {
								extension.Path = ep.Path;
								RegisterExtension (description, module, ep.Path);
							}
						}
					}
				}
				if (!found)
					monitor.ReportWarning ("The add-in '" + description.AddinId + "' is trying to register the class '" + extension.Path.Substring (1) + "', but there isn't any add-in defining a suitable extension point");
			}
			else if (extension.Path.StartsWith ("%")) {
				string[] objectTypes = extension.Path.Substring (1).Split (',');
				bool found = false;
				foreach (string s in objectTypes) {
					List<ExtensionNodeType> list;
					if (customAttributeTypeExtensions.TryGetValue (s, out list)) {
						found = true;
						foreach (ExtensionNodeType nt in list) {
							ExtensionPoint ep = (ExtensionPoint) ((ExtensionNodeSet)nt.Parent).Parent;
							if (IsAddinCompatible (ep.ParentAddinDescription, description, module)) {
								extension.Path = ep.Path;
								foreach (ExtensionNodeDescription node in extension.ExtensionNodes)
									node.NodeName = nt.NodeName;
								RegisterExtension (description, module, ep.Path);
							}
						}
					}
				}
				if (!found)
					monitor.ReportWarning ("The add-in '" + description.AddinId + "' is trying to register the class '" + extension.Path.Substring (1) + "', but there isn't any add-in defining a suitable extension point");
			}
		}
		
		public void RegisterExtension (AddinDescription description, ModuleDescription module, string path)
		{
			List<RootExtensionPoint> extensions;
			if (!pathHash.TryGetValue (path, out extensions)) {
				// Root add-in extension points are registered before any other kind of extension,
				// so we should find it now.
				extensions = GetParentExtensionInfo (path);
			}
			if (extensions == null) {
				monitor.ReportWarning ("The add-in '" + description.AddinId + "' is trying to extend '" + path + "', but there isn't any add-in defining this extension point");
				return;
			}
			
			bool found = false;
			foreach (RootExtensionPoint einfo in extensions) {
				if (IsAddinCompatible (einfo.Description, description, module)) {
					if (!einfo.ExtensionPoint.Addins.Contains (description.AddinId))
						einfo.ExtensionPoint.Addins.Add (description.AddinId);
					found = true;
					if (monitor.LogLevel > 2) {
						monitor.Log ("  * " + einfo.Description.AddinId + "(" + einfo.Description.Domain + ") <- " + path);
					}
				}
			}
			if (!found)
				monitor.ReportWarning ("The add-in '" + description.AddinId + "' is trying to extend '" + path + "', but there isn't any compatible add-in defining this extension point");
		}
		
		List<ExtensionPoint> GetCompatibleExtensionPoints (string path, AddinDescription description, ModuleDescription module, List<RootExtensionPoint> rootExtensionPoints)
		{
			List<ExtensionPoint> list = new List<ExtensionPoint> ();
			foreach (RootExtensionPoint rep in rootExtensionPoints) {
				
				// Find an extension point defined in a root add-in which is compatible with the version of the extender dependency
				if (IsAddinCompatible (rep.Description, description, module))
					list.Add (rep.ExtensionPoint);
			}
			return list;
		}
		
		List<RootExtensionPoint> GetParentExtensionInfo (string path)
		{
			int i = path.LastIndexOf ('/');
			if (i == -1)
				return null;
			string np = path.Substring (0, i);
			List<RootExtensionPoint> ep;
			if (pathHash.TryGetValue (np, out ep))
				return ep;
			else
				return GetParentExtensionInfo (np);
		}
		
		bool IsAddinCompatible (AddinDescription installedDescription, AddinDescription description, ModuleDescription module)
		{
			if (installedDescription == description)
				return true;
			if (installedDescription.Domain != AddinDatabase.GlobalDomain) {
				if (description.Domain != AddinDatabase.GlobalDomain && description.Domain != installedDescription.Domain)
					return false;
			} else if (description.Domain != AddinDatabase.GlobalDomain)
				return false;
				
			string addinId = Addin.GetFullId (installedDescription.Namespace, installedDescription.LocalId, null);
			string requiredVersion = null;
			
			IEnumerable deps;
			if (module == description.MainModule)
				deps = module.Dependencies;
			else {
				ArrayList list = new ArrayList ();
				list.AddRange (module.Dependencies);
				list.AddRange (description.MainModule.Dependencies);
				deps = list;
			}
			foreach (object dep in deps) {
				AddinDependency adep = dep as AddinDependency;
				if (adep != null && Addin.GetFullId (description.Namespace, adep.AddinId, null) == addinId) {
					requiredVersion = adep.Version;
					break;
				}
			}
			if (requiredVersion == null)
				return false;

			// Check if the required version is between rep.Description.CompatVersion and rep.Description.Version
			if (Addin.CompareVersions (installedDescription.Version, requiredVersion) > 0)
				return false;
			if (installedDescription.CompatVersion.Length > 0 && Addin.CompareVersions (installedDescription.CompatVersion, requiredVersion) < 0)
				return false;
			
			return true;
		}
	}
}
