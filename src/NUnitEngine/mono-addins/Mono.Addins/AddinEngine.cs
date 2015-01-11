//
// AddinService.cs
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
using System.Linq;
using System.Xml;
using System.Collections;
using System.Reflection;

using Mono.Addins.Description;
using Mono.Addins.Database;
using Mono.Addins.Localization;
using System.Collections.Generic;

namespace Mono.Addins
{
	/// <summary>
	/// An add-in engine.
	/// </summary>
	/// <remarks>
	/// This class allows hosting several independent add-in engines in a single application domain.
	/// In general, applications use the AddinManager class to query and manage extensions. This class is static,
	/// so the API is easily accessible. However, some kind applications may need to use several isolated
	/// add-in engines, and in this case the AddinManager class can't be used, because it is bound to a single
	/// add-in engine. Those applications can instead create several instances of the AddinEngine class. Each
	/// add-in engine can be independently initialized with different add-in registries and extension models.
	/// </remarks>
	public class AddinEngine: ExtensionContext
	{
		bool initialized;
		string startupDirectory;
		AddinRegistry registry;
		IAddinInstaller installer;
		
		bool checkAssemblyLoadConflicts;
		Dictionary<string,RuntimeAddin> loadedAddins = new Dictionary<string,RuntimeAddin> ();
		Dictionary<string,ExtensionNodeSet> nodeSets = new Dictionary<string, ExtensionNodeSet> ();
		Hashtable autoExtensionTypes = new Hashtable ();
		Dictionary<Assembly,RuntimeAddin> loadedAssemblies = new Dictionary<Assembly,RuntimeAddin> ();
		AddinLocalizer defaultLocalizer;
		IProgressStatus defaultProgressStatus = new ConsoleProgressStatus (false);
		
		/// <summary>
		/// Raised when there is an error while loading an add-in
		/// </summary>
		public static event AddinErrorEventHandler AddinLoadError;
		
		/// <summary>
		/// Raised when an add-in is loaded
		/// </summary>
		public static event AddinEventHandler AddinLoaded;
		
		/// <summary>
		/// Raised when an add-in is unloaded
		/// </summary>
		public static event AddinEventHandler AddinUnloaded;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinEngine"/> class.
		/// </summary>
		public AddinEngine ()
		{
		}
		
		/// <summary>
		/// Initializes the add-in engine
		/// </summary>
		/// <param name="configDir">
		/// Location of the add-in registry.
		/// </param>
		/// <remarks>The add-in engine needs to be initialized before doing any add-in operation.
		/// When initialized with this method, it will look for add-in in the add-in registry
		/// located in the specified path.
		/// </remarks>
		public void Initialize (string configDir)
		{
			if (initialized)
				return;
			
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			Initialize (asm, configDir, null, null);
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <param name='configDir'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='addinsDir'>
		/// Add-ins directory. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// Configuration information about the add-in registry will be stored in the
		/// provided location. The add-in engine will look for add-ins in the provided
		/// 'addinsDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public void Initialize (string configDir, string addinsDir)
		{
			if (initialized)
				return;
			
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			Initialize (asm, configDir, addinsDir, null);
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <param name='configDir'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='addinsDir'>
		/// Add-ins directory. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <param name='databaseDir'>
		/// Location of the add-in database. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// Configuration information about the add-in registry will be stored in the
		/// provided location. The add-in engine will look for add-ins in the provided
		/// 'addinsDir' directory. Cached information about add-ins will be stored in
		/// the 'databaseDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public void Initialize (string configDir, string addinsDir, string databaseDir)
		{
			if (initialized)
				return;
			
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			Initialize (asm, configDir, addinsDir, databaseDir);
		}
		
		internal void Initialize (Assembly startupAsm, string configDir, string addinsDir, string databaseDir)
		{
			lock (LocalLock) {
				if (initialized)
					return;
				
				Initialize (this);
				
				string asmFile = new Uri (startupAsm.CodeBase).LocalPath;
				startupDirectory = System.IO.Path.GetDirectoryName (asmFile);
				
				string customDir = Environment.GetEnvironmentVariable ("MONO_ADDINS_REGISTRY");
				if (customDir != null && customDir.Length > 0)
					configDir = customDir;

				if (string.IsNullOrEmpty (configDir))
					registry = AddinRegistry.GetGlobalRegistry (this, startupDirectory);
				else
					registry = new AddinRegistry (this, configDir, startupDirectory, addinsDir, databaseDir);

				if (registry.CreateHostAddinsFile (asmFile) || registry.UnknownDomain)
					registry.Update (new ConsoleProgressStatus (false));
				
				initialized = true;
				
				ActivateRoots ();
				OnAssemblyLoaded (null, null);
				AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler (OnAssemblyLoaded);
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
			}
		}

		Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
		{
			lock (LocalLock) {
				// MS.NET is more strict than Mono when loading assemblies. Assemblies loaded in the "Load" context can't see assemblies loaded
				// in the "LoadFrom" context, unless assemblies are explicitly resolved in the AssemblyResolve event.
				return loadedAddins.Values.Where(a => a.AssembliesLoaded).SelectMany(a => a.Assemblies).FirstOrDefault(a => a.FullName.ToString () == args.Name);
			}
		}
		
		/// <summary>
		/// Finalizes the add-in engine.
		/// </summary>
		public void Shutdown ()
		{
			lock (LocalLock) {
				initialized = false;
				AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler (OnAssemblyLoaded);
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
				loadedAddins = new Dictionary<string, RuntimeAddin>();
				loadedAssemblies = new Dictionary<Assembly, RuntimeAddin> ();
				registry.Dispose ();
				registry = null;
				startupDirectory = null;
				ClearContext ();
			}
		}
		
		/// <summary>
		/// Sets the default localizer to be used for this add-in engine
		/// </summary>
		/// <param name="localizer">
		/// The add-in localizer
		/// </param>
		public void InitializeDefaultLocalizer (IAddinLocalizer localizer)
		{
			CheckInitialized ();
			lock (LocalLock) {
				if (localizer != null)
					defaultLocalizer = new AddinLocalizer (localizer);
				else
					defaultLocalizer = null;
			}
		}
		
		internal string StartupDirectory {
			get { return startupDirectory; }
		}
		
		/// <summary>
		/// Gets whether the add-in engine has been initialized.
		/// </summary>
		public bool IsInitialized {
			get { return initialized; }
		}
		
		/// <summary>
		/// Gets the default add-in installer
		/// </summary>
		/// <remarks>
		/// The default installer is used by the CheckInstalled method to request
		/// the installation of missing add-ins.
		/// </remarks>
		public IAddinInstaller DefaultInstaller {
			get { return installer; }
			set { installer = value; }
		}
		
		/// <summary>
		/// Gets the default localizer for this add-in engine
		/// </summary>
		public AddinLocalizer DefaultLocalizer {
			get {
				CheckInitialized ();
				var loc = defaultLocalizer;
				return loc ?? NullLocalizer.Instance; 
			}
		}
		
		internal ExtensionContext DefaultContext {
			get { return this; }
		}
		
		/// <summary>
		/// Gets the localizer for the add-in that is invoking this property
		/// </summary>
		public AddinLocalizer CurrentLocalizer {
			get {
				CheckInitialized ();
				Assembly asm = Assembly.GetCallingAssembly ();
				RuntimeAddin addin = GetAddinForAssembly (asm);
				if (addin != null)
					return addin.Localizer;
				else
					return DefaultLocalizer;
			}
		}
		
		/// <summary>
		/// Gets a reference to the RuntimeAddin object for the add-in that is invoking this property
		/// </summary>
		public RuntimeAddin CurrentAddin {
			get {
				CheckInitialized ();
				Assembly asm = Assembly.GetCallingAssembly ();
				return GetAddinForAssembly (asm);
			}
		}
		
		/// <summary>
		/// Gets the add-in registry bound to this add-in engine
		/// </summary>
		public AddinRegistry Registry {
			get {
				CheckInitialized ();
				return registry;
			}
		}
		
		internal RuntimeAddin GetAddinForAssembly (Assembly asm)
		{
			ValidateAddinRoots ();
			RuntimeAddin ad;
			loadedAssemblies.TryGetValue (asm, out ad);
			return ad;
		}
		
		/// <summary>
		/// Checks if the provided add-ins are installed, and requests the installation of those
		/// which aren't.
		/// </summary>
		/// <param name="message">
		/// Message to show to the user when new add-ins have to be installed.
		/// </param>
		/// <param name="addinIds">
		/// List of IDs of the add-ins to be checked.
		/// </param>
		/// <remarks>
		/// This method checks if the specified add-ins are installed.
		/// If some of the add-ins are not installed, it will use
		/// the installer assigned to the DefaultAddinInstaller property
		/// to install them. If the installation fails, or if DefaultAddinInstaller
		/// is not set, an exception will be thrown.
		/// </remarks>
		public void CheckInstalled (string message, params string[] addinIds)
		{
			ArrayList notInstalled = new ArrayList ();
			foreach (string id in addinIds) {
				Addin addin = Registry.GetAddin (id, false);
				if (addin != null) {
					// The add-in is already installed
					// If the add-in is disabled, enable it now
					if (!addin.Enabled)
						addin.Enabled = true;
				} else {
					notInstalled.Add (id);
				}
			}
			if (notInstalled.Count == 0)
				return;

			var ins = installer;

			if (ins == null)
				throw new InvalidOperationException ("Add-in installer not set");
			
			// Install the add-ins
			ins.InstallAddins (Registry, message, (string[]) notInstalled.ToArray (typeof(string)));
		}
		
		// Enables or disables conflict checking while loading assemblies.
		// Disabling makes loading faster, but less safe.
		internal bool CheckAssemblyLoadConflicts {
			get { return checkAssemblyLoadConflicts; }
			set { checkAssemblyLoadConflicts = value; }
		}
		
		/// <summary>
		/// Checks if an add-in has been loaded.
		/// </summary>
		/// <param name="id">
		/// Full identifier of the add-in.
		/// </param>
		/// <returns>
		/// True if the add-in is loaded.
		/// </returns>
		public bool IsAddinLoaded (string id)
		{
			CheckInitialized ();
			ValidateAddinRoots ();
			return loadedAddins.ContainsKey (Addin.GetIdName (id));
		}
		
		internal RuntimeAddin GetAddin (string id)
		{
			ValidateAddinRoots ();
			RuntimeAddin a;
			loadedAddins.TryGetValue (Addin.GetIdName (id), out a);
			return a;
		}
		
		internal void ActivateAddin (string id)
		{
			ActivateAddinExtensions (id);
		}
		
		internal void UnloadAddin (string id)
		{
			RemoveAddinExtensions (id);
			
			RuntimeAddin addin = GetAddin (id);
			if (addin != null) {
				addin.UnloadExtensions ();
				lock (LocalLock) {
					var loadedAddinsCopy = new Dictionary<string,RuntimeAddin> (loadedAddins);
					loadedAddinsCopy.Remove (Addin.GetIdName (id));
					loadedAddins = loadedAddinsCopy;
					if (addin.AssembliesLoaded) {
						var loadedAssembliesCopy = new Dictionary<Assembly,RuntimeAddin> ();
						foreach (Assembly asm in addin.Assemblies)
							loadedAssembliesCopy.Remove (asm);
						loadedAssemblies = loadedAssembliesCopy;
					}
				}
				ReportAddinUnload (id);
			}
		}
		
		/// <summary>
		/// Forces the loading of an add-in.
		/// </summary>
		/// <param name="statusMonitor">
		/// Status monitor to keep track of the loading process.
		/// </param>
		/// <param name="id">
		/// Full identifier of the add-in to load.
		/// </param>
		/// <remarks>
		/// This method loads all assemblies that belong to an add-in in memory.
		/// All add-ins on which the specified add-in depends will also be loaded.
		/// Notice that in general add-ins don't need to be explicitely loaded using
		/// this method, since the add-in engine will load them on demand.
		/// </remarks>
		public void LoadAddin (IProgressStatus statusMonitor, string id)
		{
			CheckInitialized ();
			if (LoadAddin (statusMonitor, id, true)) {
				var adn = GetAddin (id);
				adn.EnsureAssembliesLoaded ();
			}
		}
		
		internal bool LoadAddin (IProgressStatus statusMonitor, string id, bool throwExceptions)
		{
			try {
				lock (LocalLock) {
					if (IsAddinLoaded (id))
						return true;

					if (!Registry.IsAddinEnabled (id)) {
						string msg = GettextCatalog.GetString ("Disabled add-ins can't be loaded.");
						ReportError (msg, id, null, false);
						if (throwExceptions)
							throw new InvalidOperationException (msg);
						return false;
					}

					ArrayList addins = new ArrayList ();
					Stack depCheck = new Stack ();
					ResolveLoadDependencies (addins, depCheck, id, false);
					addins.Reverse ();
					
					if (statusMonitor != null)
						statusMonitor.SetMessage ("Loading Addins");
					
					for (int n=0; n<addins.Count; n++) {
						
						if (statusMonitor != null)
							statusMonitor.SetProgress ((double) n / (double)addins.Count);
						
						Addin iad = (Addin) addins [n];
						if (IsAddinLoaded (iad.Id))
							continue;

						if (statusMonitor != null)
							statusMonitor.SetMessage (string.Format(GettextCatalog.GetString("Loading {0} add-in"), iad.Id));
						
						if (!InsertAddin (statusMonitor, iad))
							return false;
					}
					return true;
				}
			}
			catch (Exception ex) {
				ReportError ("Add-in could not be loaded: " + ex.Message, id, ex, false);
				if (statusMonitor != null)
					statusMonitor.ReportError ("Add-in '" + id + "' could not be loaded.", ex);
				if (throwExceptions)
					throw;
				return false;
			}
		}

		internal override void ResetCachedData ()
		{
			foreach (RuntimeAddin ad in loadedAddins.Values)
				ad.Addin.ResetCachedData ();
			base.ResetCachedData ();
		}
			
		bool InsertAddin (IProgressStatus statusMonitor, Addin iad)
		{
			try {
				RuntimeAddin p = new RuntimeAddin (this);
				
				// Read the config file and load the add-in assemblies
				AddinDescription description = p.Load (iad);
				
				// Register the add-in
				var loadedAddinsCopy = new Dictionary<string,RuntimeAddin> (loadedAddins);
				loadedAddinsCopy [Addin.GetIdName (p.Id)] = p;
				loadedAddins = loadedAddinsCopy;
				
				if (!AddinDatabase.RunningSetupProcess) {
					// Load the extension points and other addin data
					
					RegisterNodeSets (iad.Id, description.ExtensionNodeSets);

					foreach (ConditionTypeDescription cond in description.ConditionTypes) {
						Type ctype = p.GetType (cond.TypeName, true);
						RegisterCondition (cond.Id, ctype);
					}
				}
					
				foreach (ExtensionPoint ep in description.ExtensionPoints)
					InsertExtensionPoint (p, ep);
				
				// Fire loaded event
				NotifyAddinLoaded (p);
				ReportAddinLoad (p.Id);
				return true;
			}
			catch (Exception ex) {
				ReportError ("Add-in could not be loaded", iad.Id, ex, false);
				if (statusMonitor != null)
					statusMonitor.ReportError ("Add-in '" + iad.Id + "' could not be loaded.", ex);
				return false;
			}
		}
		
		internal void RegisterAssemblies (RuntimeAddin addin)
		{
			lock (LocalLock) {
				var loadedAssembliesCopy = new Dictionary<Assembly,RuntimeAddin> (loadedAssemblies);
				foreach (Assembly asm in addin.Assemblies)
					loadedAssembliesCopy [asm] = addin;
				loadedAssemblies = loadedAssembliesCopy;
			}
		}
		
		internal void InsertExtensionPoint (RuntimeAddin addin, ExtensionPoint ep)
		{
			CreateExtensionPoint (ep);
			foreach (ExtensionNodeType nt in ep.NodeSet.NodeTypes) {
				if (nt.ObjectTypeName.Length > 0) {
					Type ntype = addin.GetType (nt.ObjectTypeName, true);
					RegisterAutoTypeExtensionPoint (ntype, ep.Path);
				}
			}
		}
		
		bool ResolveLoadDependencies (ArrayList addins, Stack depCheck, string id, bool optional)
		{
			if (IsAddinLoaded (id))
				return true;
				
			if (depCheck.Contains (id))
				throw new InvalidOperationException ("A cyclic addin dependency has been detected.");

			depCheck.Push (id);

			Addin iad = Registry.GetAddin (id);
			if (iad == null || !iad.Enabled) {
				if (optional)
					return false;
				else if (iad != null && !iad.Enabled)
					throw new MissingDependencyException (GettextCatalog.GetString ("The required addin '{0}' is disabled.", id));
				else
					throw new MissingDependencyException (GettextCatalog.GetString ("The required addin '{0}' is not installed.", id));
			}

			// If this addin has already been requested, bring it to the head
			// of the list, so it is loaded earlier than before.
			addins.Remove (iad);
			addins.Add (iad);
			
			foreach (Dependency dep in iad.AddinInfo.Dependencies) {
				AddinDependency adep = dep as AddinDependency;
				if (adep != null) {
					try {
						string adepid = Addin.GetFullId (iad.AddinInfo.Namespace, adep.AddinId, adep.Version);
						ResolveLoadDependencies (addins, depCheck, adepid, false);
					} catch (MissingDependencyException) {
						if (optional)
							return false;
						else
							throw;
					}
				}
			}
			
			if (iad.AddinInfo.OptionalDependencies != null) {
				foreach (Dependency dep in iad.AddinInfo.OptionalDependencies) {
					AddinDependency adep = dep as AddinDependency;
					if (adep != null) {
						string adepid = Addin.GetFullId (iad.Namespace, adep.AddinId, adep.Version);
						if (!ResolveLoadDependencies (addins, depCheck, adepid, true))
						return false;
					}
				}
			}
				
			depCheck.Pop ();
			return true;
		}
		
		void RegisterNodeSets (string addinId, ExtensionNodeSetCollection nsets)
		{
			lock (LocalLock) {
				var nodeSetsCopy = new Dictionary<string,ExtensionNodeSet> (nodeSets);
				foreach (ExtensionNodeSet nset in nsets) {
					nset.SourceAddinId = addinId;
					nodeSetsCopy [nset.Id] = nset;
				}
				nodeSets = nodeSetsCopy;
			}
		}
		
		internal void UnregisterAddinNodeSets (string addinId)
		{
			lock (LocalLock) {
				var nodeSetsCopy = new Dictionary<string,ExtensionNodeSet> (nodeSets);
				foreach (var nset in nodeSetsCopy.Values.Where (n => n.SourceAddinId == addinId).ToArray ())
					nodeSetsCopy.Remove (nset.Id);
				nodeSets = nodeSetsCopy;
			}
		}
		
		internal string GetNodeTypeAddin (ExtensionNodeSet nset, string type, string callingAddinId)
		{
			ExtensionNodeType nt = FindType (nset, type, callingAddinId);
			if (nt != null)
				return nt.AddinId;
			else
				return null;
		}
		
		internal ExtensionNodeType FindType (ExtensionNodeSet nset, string name, string callingAddinId)
		{
			if (nset == null)
				return null;

			foreach (ExtensionNodeType nt in nset.NodeTypes) {
				if (nt.Id == name)
					return nt;
			}
			
			foreach (string ns in nset.NodeSets) {
				ExtensionNodeSet regSet;
				if (!nodeSets.TryGetValue (ns, out regSet)) {
					ReportError ("Unknown node set: " + ns, callingAddinId, null, false);
					return null;
				}
				ExtensionNodeType nt = FindType (regSet, name, callingAddinId);
				if (nt != null)
					return nt;
			}
			return null;
		}
		
		internal void RegisterAutoTypeExtensionPoint (Type type, string path)
		{
			autoExtensionTypes [type] = path;
		}

		internal void UnregisterAutoTypeExtensionPoint (Type type, string path)
		{
			autoExtensionTypes.Remove (type);
		}
		
		internal string GetAutoTypeExtensionPoint (Type type)
		{
			return autoExtensionTypes [type] as string;
		}

		void OnAssemblyLoaded (object s, AssemblyLoadEventArgs a)
		{
			if (a != null) {
				lock (pendingRootChecks) {
					pendingRootChecks.Add (a.LoadedAssembly);
				}
			}
		}

		List<Assembly> pendingRootChecks = new List<Assembly> ();

		internal void ValidateAddinRoots ()
		{
			List<Assembly> copy = null;
			lock (pendingRootChecks) {
				if (pendingRootChecks.Count > 0) {
					copy = new List<Assembly> (pendingRootChecks);
					pendingRootChecks.Clear ();
				}
			}
			if (copy != null) {
				foreach (Assembly asm in copy)
					CheckHostAssembly (asm);
			}
		}

		internal void ActivateRoots ()
		{
			lock (pendingRootChecks)
				pendingRootChecks.Clear ();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies ())
				CheckHostAssembly (asm);
		}
		
		void CheckHostAssembly (Assembly asm)
		{
			if (AddinDatabase.RunningSetupProcess || asm is System.Reflection.Emit.AssemblyBuilder || asm.IsDynamic)
				return;
			string codeBase;
			try {
				codeBase = asm.CodeBase;
			} catch {
				return;
			}

			Uri u;
			if (!Uri.TryCreate (codeBase, UriKind.Absolute, out u))
				return;

			string asmFile = u.LocalPath;
			Addin ainfo;
			try {
				ainfo = Registry.GetAddinForHostAssembly (asmFile);
			} catch (Exception ex) {
				// GetAddinForHostAssembly may crash if the add-in db has been corrupted. In this case, update the db
				// and try getting the add-in info again. If it crashes again, then this is a bug.
				defaultProgressStatus.ReportError ("Add-in description could not be loaded.", ex);
				Registry.Update (null);
				ainfo = Registry.GetAddinForHostAssembly (asmFile);
			}

			if (ainfo != null && !IsAddinLoaded (ainfo.Id)) {
				AddinDescription adesc = null;
				try {
					adesc = ainfo.Description;
				} catch (Exception ex) {
					defaultProgressStatus.ReportError ("Add-in description could not be loaded.", ex);
				}
				if (adesc == null || adesc.FilesChanged ()) {
					// If the add-in has changed, update the add-in database.
					// We do it here because once loaded, add-in roots can't be
					// reloaded like regular add-ins.
					Registry.Update (null);
					ainfo = Registry.GetAddinForHostAssembly (asmFile);
					if (ainfo == null)
						return;
				}
				LoadAddin (null, ainfo.Id, false);
			}
		}
		
		/// <summary>
		/// Creates a new extension context.
		/// </summary>
		/// <returns>
		/// The new extension context.
		/// </returns>
		/// <remarks>
		/// Extension contexts can be used to query the extension model using particular condition values.
		/// </remarks>
		public ExtensionContext CreateExtensionContext ()
		{
			CheckInitialized ();
			return CreateChildContext ();
		}
		
		internal void CheckInitialized ()
		{
			if (!initialized)
				throw new InvalidOperationException ("Add-in engine not initialized.");
		}
		
		internal void ReportError (string message, string addinId, Exception exception, bool fatal)
		{
			var handler = AddinLoadError;
			if (handler != null)
				handler (null, new AddinErrorEventArgs (message, addinId, exception));
			else {
				Console.WriteLine (message);
				if (exception != null)
					Console.WriteLine (exception);
			}
		}
		
		internal void ReportAddinLoad (string id)
		{
			var handler = AddinLoaded;
			if (handler != null) {
				try {
					handler (null, new AddinEventArgs (id));
				} catch {
					// Ignore subscriber exceptions
				}
			}
		}
		
		internal void ReportAddinUnload (string id)
		{
			var handler = AddinUnloaded;
			if (handler != null) {
				try {
					handler (null, new AddinEventArgs (id));
				} catch {
					// Ignore subscriber exceptions
				}
			}
		}
	}
		
}
