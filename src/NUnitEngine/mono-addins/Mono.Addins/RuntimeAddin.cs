//
// RuntimeAddin.cs
//
// Author:
//   Lluis Sanchez Gual,
//   Georg Wächter
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Resources;
using System.Globalization;

using Mono.Addins.Description;
using Mono.Addins.Localization;

namespace Mono.Addins
{
	/// <summary>
	/// Run-time representation of an add-in.
	/// </summary>
	public class RuntimeAddin
	{
		string id;
		string baseDirectory;
		string privatePath;
		Addin ainfo;
		RuntimeAddin parentAddin;
		
		Assembly[] assemblies;
		RuntimeAddin[] depAddins;
		ResourceManager[] resourceManagers;
		AddinLocalizer localizer;
		ModuleDescription module;
		AddinEngine addinEngine;
		
		internal RuntimeAddin (AddinEngine addinEngine)
		{
			this.addinEngine = addinEngine;
		}
		
		internal RuntimeAddin (AddinEngine addinEngine, RuntimeAddin parentAddin, ModuleDescription module)
		{
			this.addinEngine = addinEngine;
			this.parentAddin = parentAddin;
			this.module = module;
			id = parentAddin.id;
			baseDirectory = parentAddin.baseDirectory;
			privatePath = parentAddin.privatePath;
			ainfo = parentAddin.ainfo;
			localizer = parentAddin.localizer;
			module.RuntimeAddin = this;
		}
		
		internal ModuleDescription Module {
			get { return module; }
		}
		
		internal Assembly[] Assemblies {
			get {
				EnsureAssembliesLoaded ();
				return assemblies;
			}
		}
		
		/// <summary>
		/// Identifier of the add-in.
		/// </summary>
		public string Id {
			get { return Addin.GetIdName (id); }
		}
		
		/// <summary>
		/// Version of the add-in.
		/// </summary>
		public string Version {
			get { return Addin.GetIdVersion (id); }
		}
		
		internal Addin Addin {
			get { return ainfo; }
		}

		/// <summary>
		/// Returns a string that represents the current RuntimeAddin.
		/// </summary>
		/// <returns>
		/// A string that represents the current RuntimeAddin.
		/// </returns>
		public override string ToString ()
		{
			return ainfo.ToString ();
		}

		ResourceManager[] GetResourceManagers ()
		{
			if (resourceManagers != null)
				return resourceManagers;
			
			EnsureAssembliesLoaded ();
			ArrayList managersList = new ArrayList ();

			// Search for embedded resource files
			foreach (Assembly asm in assemblies)
			{
				foreach (string res in asm.GetManifestResourceNames ()) {
					if (res.EndsWith (".resources"))
						managersList.Add (new ResourceManager (res.Substring (0, res.Length - ".resources".Length), asm));
				}
			}

			return resourceManagers = (ResourceManager[]) managersList.ToArray (typeof(ResourceManager));
		}

		/// <summary>
		/// Gets a resource string
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <returns>
		/// The value of the resource string, or null if the resource can't be found.
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public string GetResourceString (string name)
		{
			return (string) GetResourceObject (name, true, null);
		}

		/// <summary>
		/// Gets a resource string
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <param name="throwIfNotFound">
		/// When set to true, an exception will be thrown if the resource is not found.
		/// </param>
		/// <returns>
		/// The value of the resource string
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public string GetResourceString (string name, bool throwIfNotFound)
		{
			return (string) GetResourceObject (name, throwIfNotFound, null);
		}

		/// <summary>
		/// Gets a resource string
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <param name="throwIfNotFound">
		/// When set to true, an exception will be thrown if the resource is not found.
		/// </param>
		/// <param name="culture">
		/// Culture of the resource
		/// </param>
		/// <returns>
		/// The value of the resource string
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public string GetResourceString (string name, bool throwIfNotFound, CultureInfo culture)
		{
			return (string) GetResourceObject (name, throwIfNotFound, culture);
		}

		/// <summary>
		/// Gets a resource object
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <returns>
		/// Value of the resource
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public object GetResourceObject (string name)
		{
			return GetResourceObject (name, true, null);
		}

		/// <summary>
		/// Gets a resource object
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <param name="throwIfNotFound">
		/// When set to true, an exception will be thrown if the resource is not found.
		/// </param>
		/// <returns>
		/// Value of the resource
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public object GetResourceObject (string name, bool throwIfNotFound)
		{
			return GetResourceObject (name, throwIfNotFound, null);
		}

		/// <summary>
		/// Gets a resource object
		/// </summary>
		/// <param name="name">
		/// Name of the resource
		/// </param>
		/// <param name="throwIfNotFound">
		/// When set to true, an exception will be thrown if the resource is not found.
		/// </param>
		/// <param name="culture">
		/// Culture of the resource
		/// </param>
		/// <returns>
		/// Value of the resource
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public object GetResourceObject (string name, bool throwIfNotFound, CultureInfo culture)
		{
			// Look in resources of this add-in
			foreach (ResourceManager manager in GetAllResourceManagers ()) {
				object t = manager.GetObject (name, culture);
				if (t != null)
					return t;
			}

			// Look in resources of dependent add-ins
			foreach (RuntimeAddin addin in GetAllDependencies ()) {
				object t = addin.GetResourceObject (name, false, culture);
				if (t != null)
					return t;
			}

			if (throwIfNotFound)
				throw new InvalidOperationException ("Resource object '" + name + "' not found in add-in '" + id + "'");

			return null;
		}

		/// <summary>
		/// Gets a type defined in the add-in
		/// </summary>
		/// <param name="typeName">
		/// Full name of the type
		/// </param>
		/// <returns>
		/// A type.
		/// </returns>
		/// <remarks>
		/// The type will be looked up in the assemblies that implement the add-in,
		/// and recursivelly in all add-ins on which it depends.
		/// 
		/// This method throws an InvalidOperationException if the type can't be found.
		/// </remarks>
		public Type GetType (string typeName)
		{
			return GetType (typeName, true);
		}
		
		/// <summary>
		/// Gets a type defined in the add-in
		/// </summary>
		/// <param name="typeName">
		/// Full name of the type
		/// </param>
		/// <param name="throwIfNotFound">
		/// Indicates whether the method should throw an exception if the type can't be found.
		/// </param>
		/// <returns>
		/// A <see cref="Type"/>
		/// </returns>
		/// <remarks>
		/// The type will be looked up in the assemblies that implement the add-in,
		/// and recursivelly in all add-ins on which it depends.
		/// 
		/// If the type can't be found, this method throw a InvalidOperationException if
		/// 'throwIfNotFound' is 'true', or 'null' otherwise.
		/// </remarks>
		public Type GetType (string typeName, bool throwIfNotFound)
		{
			EnsureAssembliesLoaded ();
			
			// Look in the addin assemblies
			
			Type at = Type.GetType (typeName, false);
			if (at != null)
				return at;
			
			foreach (Assembly asm in GetAllAssemblies ()) {
				Type t = asm.GetType (typeName, false);
				if (t != null)
					return t;
			}
			
			// Look in the dependent add-ins
			foreach (RuntimeAddin addin in GetAllDependencies ()) {
				Type t = addin.GetType (typeName, false);
				if (t != null)
					return t;
			}
			
			if (throwIfNotFound)
				throw new InvalidOperationException ("Type '" + typeName + "' not found in add-in '" + id + "'");
			return null;
		}
		
		IEnumerable<ResourceManager> GetAllResourceManagers ()
		{
			foreach (ResourceManager rm in GetResourceManagers ())
				yield return rm;
			
			if (parentAddin != null) {
				foreach (ResourceManager rm in parentAddin.GetResourceManagers ())
					yield return rm;
			}
		}
		
		IEnumerable<Assembly> GetAllAssemblies ()
		{
			foreach (Assembly asm in Assemblies)
				yield return asm;
			
			// Look in the parent addin assemblies
			
			if (parentAddin != null) {
				foreach (Assembly asm in parentAddin.Assemblies)
					yield return asm;
			}
		}
		
		IEnumerable<RuntimeAddin> GetAllDependencies ()
		{
			// Look in the dependent add-ins
			foreach (RuntimeAddin addin in GetDepAddins ())
				yield return addin;
			
			if (parentAddin != null) {
				// Look in the parent dependent add-ins
				foreach (RuntimeAddin addin in parentAddin.GetDepAddins ())
					yield return addin;
			}
		}
		
		/// <summary>
		/// Creates an instance of a type defined in the add-in
		/// </summary>
		/// <param name="typeName">
		/// Name of the type.
		/// </param>
		/// <returns>
		/// A new instance of the type
		/// </returns>
		/// <remarks>
		/// The type will be looked up in the assemblies that implement the add-in,
		/// and recursivelly in all add-ins on which it depends.
		/// 
		/// This method throws an InvalidOperationException if the type can't be found.
		/// 
		/// The specified type must have a default constructor.
		/// </remarks>
		public object CreateInstance (string typeName)
		{
			return CreateInstance (typeName, true);
		}
		
		/// <summary>
		/// Creates an instance of a type defined in the add-in
		/// </summary>
		/// <param name="typeName">
		/// Name of the type.
		/// </param>
		/// <param name="throwIfNotFound">
		/// Indicates whether the method should throw an exception if the type can't be found.
		/// </param>
		/// <returns>
		/// A new instance of the type
		/// </returns>
		/// <remarks>
		/// The type will be looked up in the assemblies that implement the add-in,
		/// and recursivelly in all add-ins on which it depends.
		/// 
		/// If the type can't be found, this method throw a InvalidOperationException if
		/// 'throwIfNotFound' is 'true', or 'null' otherwise.
		/// 
		/// The specified type must have a default constructor.
		/// </remarks>
		public object CreateInstance (string typeName, bool throwIfNotFound)
		{
			Type type = GetType (typeName, throwIfNotFound);
			if (type == null)
				return null;
			else
				return Activator.CreateInstance (type, true);
		}
		
		/// <summary>
		/// Gets the path of an add-in file
		/// </summary>
		/// <param name="fileName">
		/// Relative path of the file
		/// </param>
		/// <returns>
		/// Full path of the file
		/// </returns>
		/// <remarks>
		/// This method can be used to get the full path of a data file deployed together with the add-in.
		/// </remarks>
		public string GetFilePath (string fileName)
		{
			return Path.Combine (baseDirectory, fileName);
		}

		/// <summary>
		/// Gets the path of an add-in file
		/// </summary>
		/// <param name="filePath">
		/// Components of the file path
		/// </param>
		/// <returns>
		/// Full path of the file
		/// </returns>
		/// <remarks>
		/// This method can be used to get the full path of a data file deployed together with the add-in.
		/// </remarks>
		public string GetFilePath (params string[] filePath)
		{
			return Path.Combine (baseDirectory, string.Join ("" + Path.DirectorySeparatorChar, filePath));
		}
		
		/// <summary>
		/// Path to a directory where add-ins can store private configuration or status data
		/// </summary>
		public string PrivateDataPath {
			get {
				if (privatePath == null) {
					privatePath = ainfo.PrivateDataPath;
					if (!Directory.Exists (privatePath))
						Directory.CreateDirectory (privatePath);
				}
				return privatePath;
			}
		}
		
		/// <summary>
		/// Gets the content of a resource
		/// </summary>
		/// <param name="resourceName">
		/// Name of the resource
		/// </param>
		/// <returns>
		/// Content of the resource, or null if not found
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public Stream GetResource (string resourceName)
		{
			return GetResource (resourceName, false);
		}
		
		/// <summary>
		/// Gets the content of a resource
		/// </summary>
		/// <param name="resourceName">
		/// Name of the resource
		/// </param>
		/// <param name="throwIfNotFound">
		/// When set to true, an exception will be thrown if the resource is not found.
		/// </param>
		/// <returns>
		/// Content of the resource.
		/// </returns>
		/// <remarks>
		/// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
		/// </remarks>
		public Stream GetResource (string resourceName, bool throwIfNotFound)
		{
			EnsureAssembliesLoaded ();
			
			// Look in the addin assemblies
			
			foreach (Assembly asm in GetAllAssemblies ()) {
				Stream res = asm.GetManifestResourceStream (resourceName);
				if (res != null)
					return res;
			}
			
			// Look in the dependent add-ins
			foreach (RuntimeAddin addin in GetAllDependencies ()) {
				Stream res = addin.GetResource (resourceName);
				if (res != null)
					return res;
			}
			
			if (throwIfNotFound)
				throw new InvalidOperationException ("Resource '" + resourceName + "' not found in add-in '" + id + "'");
				
			return null;
		}
		
		/// <summary>
		/// Localizer which can be used to localize strings defined in this add-in
		/// </summary>
		public AddinLocalizer Localizer {
			get {
				if (localizer != null)
					return localizer;
				else
					return addinEngine.DefaultLocalizer;
			}
		}
		
		internal RuntimeAddin GetModule (ModuleDescription module)
		{
			// If requesting the root module, return this
			if (module == module.ParentAddinDescription.MainModule)
				return this;
			
			if (module.RuntimeAddin != null)
				return module.RuntimeAddin;
			
			RuntimeAddin addin = new RuntimeAddin (addinEngine, this, module);
			return addin;
		}
		
		internal AddinDescription Load (Addin iad)
		{
			ainfo = iad;
			
			AddinDescription description = iad.Description;
			id = description.AddinId;
			baseDirectory = description.BasePath;
			module = description.MainModule;
			module.RuntimeAddin = this;
			
			if (description.Localizer != null) {
				string cls = description.Localizer.GetAttribute ("type");
				
				// First try getting one of the stock localizers. If none of found try getting the type.
				object fob = null;
				Type t = Type.GetType ("Mono.Addins.Localization." + cls + "Localizer, " + GetType().Assembly.FullName, false);
				if (t != null)
					fob = Activator.CreateInstance (t);
				
				if (fob == null)
					fob = CreateInstance (cls, true);
				
				IAddinLocalizerFactory factory = fob as IAddinLocalizerFactory;
				if (factory == null)
					throw new InvalidOperationException ("Localizer factory type '" + cls + "' must implement IAddinLocalizerFactory");
				localizer = new AddinLocalizer (factory.CreateLocalizer (this, description.Localizer));
			}
			
			return description;
		}
		
		RuntimeAddin[] GetDepAddins ()
		{
			if (depAddins != null)
				return depAddins;
			
			ArrayList plugList = new ArrayList ();
			string ns = ainfo.Description.Namespace;
			
			// Collect dependent ids
			foreach (Dependency dep in module.Dependencies) {
				AddinDependency pdep = dep as AddinDependency;
				if (pdep != null) {
					RuntimeAddin adn = addinEngine.GetAddin (Addin.GetFullId (ns, pdep.AddinId, pdep.Version));
					if (adn != null)
						plugList.Add (adn);
					else
						addinEngine.ReportError ("Add-in dependency not loaded: " + pdep.FullAddinId, module.ParentAddinDescription.AddinId, null, false);
				}
			}
			return depAddins = (RuntimeAddin[]) plugList.ToArray (typeof(RuntimeAddin));
		}
		
		void LoadModule (ModuleDescription module, ArrayList asmList)
		{
			// Load the assemblies
			foreach (string s in module.Assemblies) {
				Assembly asm = null;

				// don't load the assembly if it's already loaded
				string asmPath = Path.Combine (baseDirectory, s);
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
					// Sorry, you can't load addins from
					// dynamic assemblies as get_Location
					// throws a NotSupportedException
                    if (a is System.Reflection.Emit.AssemblyBuilder || a.IsDynamic) {
						continue;
					}
					
					try {
						if (a.Location == asmPath) {
							asm = a;
							break;
						}
					} catch (NotSupportedException) {
						// Some assemblies don't have a location
					}
				}

				if (asm == null) {
					asm = Assembly.LoadFrom (asmPath);
				}

				asmList.Add (asm);
			}
		}
		
		internal void UnloadExtensions ()
		{
			addinEngine.UnregisterAddinNodeSets (id);
		}
		
		bool CheckAddinDependencies (ModuleDescription module, bool forceLoadAssemblies)
		{
			foreach (Dependency dep in module.Dependencies) {
				AddinDependency pdep = dep as AddinDependency;
				if (pdep == null)
					continue;
				if (!addinEngine.IsAddinLoaded (pdep.FullAddinId))
					return false;
				if (forceLoadAssemblies)
					addinEngine.GetAddin (pdep.FullAddinId).EnsureAssembliesLoaded ();
			}
			return true;
		}
		
		internal bool AssembliesLoaded {
			get { return assemblies != null; }
		}
		
		internal void EnsureAssembliesLoaded ()
		{
			if (assemblies != null)
				return;
			
			ArrayList asmList = new ArrayList ();
			
			// Load the assemblies of the module
			CheckAddinDependencies (module, true);
			LoadModule (module, asmList);
			
			assemblies = (Assembly[]) asmList.ToArray (typeof(Assembly));
			addinEngine.RegisterAssemblies (this);
		}
	}
}
