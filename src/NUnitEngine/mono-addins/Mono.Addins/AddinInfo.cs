//
// AddinInfo.cs
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
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Mono.Addins.Description;

namespace Mono.Addins
{
	internal class AddinInfo
	{
		string id = "";
		string namspace = "";
		string name = "";
		string version = "";
		string baseVersion = "";
		string author = "";
		string copyright = "";
		string url = "";
		string description = "";
		string category = "";
		bool defaultEnabled = true;
		bool isroot;
		DependencyCollection dependencies;
		DependencyCollection optionalDependencies;
		AddinPropertyCollection properties;
		
		private AddinInfo ()
		{
			dependencies = new DependencyCollection ();
			optionalDependencies = new DependencyCollection ();
		}
		
		public string Id {
			get { return Addin.GetFullId (namspace, id, version); }
		}
		
		public string LocalId {
			get { return id; }
			set { id = value; }
		}
		
		public string Namespace {
			get { return namspace; }
			set { namspace = value; }
		}
		
		public bool IsRoot {
			get { return isroot; }
			set { isroot = value; }
		}
		
		public string Name {
			get {
				string s = Properties.GetPropertyValue ("Name");
				if (s.Length > 0)
					return s;
				if (name != null && name.Length > 0)
					return name;
				string sid = id;
				if (sid.StartsWith ("__"))
					sid = sid.Substring (2);
				return Addin.GetFullId (namspace, sid, null); 
			}
			set { name = value; }
		}
		
		public string Version {
			get { return version; }
			set { version = value; }
		}
		
		public string BaseVersion {
			get { return baseVersion; }
			set { baseVersion = value; }
		}
		
		public string Author {
			get {
				string s = Properties.GetPropertyValue ("Author");
				if (s.Length > 0)
					return s;
				return author;
			}
			set { author = value; }
		}
		
		public string Copyright {
			get {
				string s = Properties.GetPropertyValue ("Copyright");
				if (s.Length > 0)
					return s;
				return copyright;
			}
			set { copyright = value; }
		}
		
		public string Url {
			get {
				string s = Properties.GetPropertyValue ("Url");
				if (s.Length > 0)
					return s;
				return url;
			}
			set { url = value; }
		}
		
		public string Description {
			get {
				string s = Properties.GetPropertyValue ("Description");
				if (s.Length > 0)
					return s;
				return description;
			}
			set { description = value; }
		}
		
		public string Category {
			get {
				string s = Properties.GetPropertyValue ("Category");
				if (s.Length > 0)
					return s;
				return category;
			}
			set { category = value; }
		}
		
		public bool EnabledByDefault {
			get { return defaultEnabled; }
			set { defaultEnabled = value; }
		}
		
		public DependencyCollection Dependencies {
			get { return dependencies; }
		}
		
		public DependencyCollection OptionalDependencies {
			get { return optionalDependencies; }
		}
		
		public AddinPropertyCollection Properties {
			get { return properties; }
		}
		
		internal static AddinInfo ReadFromDescription (AddinDescription description)
		{
			AddinInfo info = new AddinInfo ();
			info.id = description.LocalId;
			info.namspace = description.Namespace;
			info.name = description.Name;
			info.version = description.Version;
			info.author = description.Author;
			info.copyright = description.Copyright;
			info.url = description.Url;
			info.description = description.Description;
			info.category = description.Category;
			info.baseVersion = description.CompatVersion;
			info.isroot = description.IsRoot;
			info.defaultEnabled = description.EnabledByDefault;
			
			foreach (Dependency dep in description.MainModule.Dependencies)
				info.Dependencies.Add (dep);
				
			foreach (ModuleDescription mod in description.OptionalModules) {
				foreach (Dependency dep in mod.Dependencies)
					info.OptionalDependencies.Add (dep);
			}
			info.properties = description.Properties;
			
			return info;
		}
		
		public bool SupportsVersion (string version)
		{
			if (Addin.CompareVersions (Version, version) > 0)
				return false;
			if (baseVersion == "")
				return true;
			return Addin.CompareVersions (BaseVersion, version) >= 0;
		}
		
		public int CompareVersionTo (AddinInfo other)
		{
			return Addin.CompareVersions (this.version, other.Version);
		}
	}
}
