//
// AddinRegistry.cs
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
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using Mono.Addins.Database;
using Mono.Addins.Description;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Addins
{
	/// <summary>
	/// An add-in registry.
	/// </summary>
	/// <remarks>
	/// An add-in registry is a data structure used by the add-in engine to locate add-ins to load.
	/// 
	/// A registry can be configured to look for add-ins in several directories. However, add-ins
	/// copied to those directories won't be detected until an explicit add-in scan is requested.
	/// The registry can be updated by an application by calling Registry.Update(), or by a user by
	/// running the 'mautil' add-in setup tool.
	/// 
	/// The registry has information about the location of every add-in and a timestamp of the last
	/// check, so the Update method will only scan new or modified add-ins. An application can
	/// add a call to Registry.Update() in the Main method to detect all new add-ins every time the
	/// app is started.
	/// 
	/// Every add-in added to the registry is parsed and validated, and if there is any error it
	/// will be rejected. The registry is also in charge of scanning the add-in assemblies and look
	/// for extensions and other information declared using custom attributes. That information is
	/// merged with the manifest information (if there is one) to create a complete add-in
	/// description ready to be used at run-time.
	/// 
	/// Mono.Addins allows sharing an add-in registry among several applications. In this context,
	/// all applications sharing the registry share the same extension point model, and it is
	/// possible to implement add-ins which extend several hosts.
	/// </remarks>
	public class AddinRegistry: IDisposable
	{
		AddinDatabase database;
		StringCollection addinDirs;
		string basePath;
		string currentDomain;
		string startupDirectory;
		string addinsDir;
		string databaseDir;
		
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="registryPath">
		/// Location of the add-in registry.
		/// </param>
		/// <remarks>
		/// Creates a new add-in registry located in the provided path.
		/// The add-in registry will look for add-ins in an 'addins'
		/// subdirectory of the provided registryPath.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public AddinRegistry (string registryPath): this (null, registryPath, null, null, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="registryPath">
		/// Location of the add-in registry.
		/// </param>
		/// <param name="startupDirectory">
		/// Location of the application.
		/// </param>
		/// <remarks>
		/// Creates a new add-in registry located in the provided path.
		/// The add-in registry will look for add-ins in an 'addins'
		/// subdirectory of the provided registryPath.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public AddinRegistry (string registryPath, string startupDirectory): this (null, registryPath, startupDirectory, null, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinRegistry"/> class.
		/// </summary>
		/// <param name='registryPath'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='startupDirectory'>
		/// Location of the application.
		/// </param>
		/// <param name='addinsDir'>
		/// Add-ins directory. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <remarks>
		/// Creates a new add-in registry located in the provided path.
		/// Configuration information about the add-in registry will be stored in
		/// 'registryPath'. The add-in registry will look for add-ins in the provided
		/// 'addinsDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public AddinRegistry (string registryPath, string startupDirectory, string addinsDir): this (null, registryPath, startupDirectory, addinsDir, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinRegistry"/> class.
		/// </summary>
		/// <param name='registryPath'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='startupDirectory'>
		/// Location of the application.
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
		/// Creates a new add-in registry located in the provided path.
		/// Configuration information about the add-in registry will be stored in
		/// 'registryPath'. The add-in registry will look for add-ins in the provided
		/// 'addinsDir' directory. Cached information about add-ins will be stored in
		/// the 'databaseDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public AddinRegistry (string registryPath, string startupDirectory, string addinsDir, string databaseDir): this (null, registryPath, startupDirectory, addinsDir, databaseDir)
		{
		}
		
		internal AddinRegistry (AddinEngine engine, string registryPath, string startupDirectory, string addinsDir, string databaseDir)
		{
			basePath = Path.GetFullPath (Util.NormalizePath (registryPath));
			
			if (addinsDir != null) {
				addinsDir = Util.NormalizePath (addinsDir);
				if (Path.IsPathRooted (addinsDir))
					this.addinsDir = Path.GetFullPath (addinsDir);
				else
					this.addinsDir = Path.GetFullPath (Path.Combine (basePath, addinsDir));
			} else
				this.addinsDir = Path.Combine (basePath, "addins");
			
			if (databaseDir != null) {
				databaseDir = Util.NormalizePath (databaseDir);
				if (Path.IsPathRooted (databaseDir))
					this.databaseDir = Path.GetFullPath (databaseDir);
				else
					this.databaseDir = Path.GetFullPath (Path.Combine (basePath, databaseDir));
			}
			else
				this.databaseDir = Path.GetFullPath (basePath);

			// Look for add-ins in the hosts directory and in the default
			// addins directory
			addinDirs = new StringCollection ();
			addinDirs.Add (DefaultAddinsFolder);
			
			// Initialize the database after all paths have been set
			database = new AddinDatabase (engine, this);
			
			// Get the domain corresponding to the startup folder
			if (startupDirectory != null && startupDirectory.Length > 0) {
				this.startupDirectory = Util.NormalizePath (startupDirectory);
				currentDomain = database.GetFolderDomain (null, this.startupDirectory);
			} else
				currentDomain = AddinDatabase.GlobalDomain;
		}

		/// <summary>
		/// Gets the global registry.
		/// </summary>
		/// <returns>
		/// The global registry
		/// </returns>
		/// <remarks>
		/// The global add-in registry is created in "~/.config/mono.addins",
		/// and it is the default registry used when none is specified.
		/// </remarks>
		public static AddinRegistry GetGlobalRegistry ()
		{
			return GetGlobalRegistry (null, null);
		}
		
		internal static AddinRegistry GetGlobalRegistry (AddinEngine engine, string startupDirectory)
		{
			AddinRegistry reg = new AddinRegistry (engine, GlobalRegistryPath, startupDirectory, null, null);
			string baseDir;
			if (Util.IsWindows)
				baseDir = Environment.GetFolderPath (Environment.SpecialFolder.CommonProgramFiles); 
			else
				baseDir = "/etc";
			
			reg.GlobalAddinDirectories.Add (Path.Combine (baseDir, "mono.addins"));
			return reg;
		}
		
		internal bool UnknownDomain {
			get { return currentDomain == AddinDatabase.UnknownDomain; }
		}
		
		internal static string GlobalRegistryPath {
			get {
				string customDir = Environment.GetEnvironmentVariable ("MONO_ADDINS_GLOBAL_REGISTRY");
				if (customDir != null && customDir.Length > 0)
					return Path.GetFullPath (Util.NormalizePath (customDir));
				
				string path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData); 
				path = Path.Combine (path, "mono.addins");
				return Path.GetFullPath (path);
			}
		}
		
		internal string CurrentDomain {
			get { return currentDomain; }
		}
		
		/// <summary>
		/// Location of the add-in registry.
		/// </summary>
		public string RegistryPath {
			get { return basePath; }
		}
		
		/// <summary>
		/// Disposes the add-in engine.
		/// </summary>
		public void Dispose ()
		{
			database.Shutdown ();
		}
		
		/// <summary>
		/// Returns an add-in from the registry.
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in.
		/// </param>
		/// <returns>
		/// The add-in, or 'null' if not found.
		/// </returns>
		/// <remarks>
		/// The add-in identifier may optionally include a version number, for example: "TextEditor.Xml,1.2"
		/// </remarks>
		public Addin GetAddin (string id)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return null;
			Addin ad = database.GetInstalledAddin (currentDomain, id);
			if (ad != null && IsRegisteredForUninstall (ad.Id))
				return null;
			return ad;
		}
		
		/// <summary>
		/// Returns an add-in from the registry.
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in.
		/// </param>
		/// <param name="exactVersionMatch">
		/// 'true' if the exact add-in version must be found.
		/// </param>
		/// <returns>
		/// The add-in, or 'null' if not found.
		/// </returns>
		/// <remarks>
		/// The add-in identifier may optionally include a version number, for example: "TextEditor.Xml,1.2".
		/// In this case, if the exact version is not found and exactVersionMatch is 'false', it will
		/// return one than is compatible with the required version.
		/// </remarks>
		public Addin GetAddin (string id, bool exactVersionMatch)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return null;
			Addin ad = database.GetInstalledAddin (currentDomain, id, exactVersionMatch);
			if (ad != null && IsRegisteredForUninstall (ad.Id))
				return null;
			return ad;
		}
		
		/// <summary>
		/// Gets all add-ins or add-in roots registered in the registry.
		/// </summary>
		/// <returns>
		/// The addins.
		/// </returns>
		/// <param name='flags'>
		/// Flags.
		/// </param>
		public Addin[] GetModules (AddinSearchFlags flags)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return new Addin [0];
			AddinSearchFlagsInternal f = (AddinSearchFlagsInternal)(int)flags;
			return database.GetInstalledAddins (currentDomain, f | AddinSearchFlagsInternal.ExcludePendingUninstall).ToArray ();
		}
		
		/// <summary>
		/// Gets all add-ins registered in the registry.
		/// </summary>
		/// <returns>
		/// Add-ins registered in the registry.
		/// </returns>
		public Addin[] GetAddins ()
		{
			return GetModules (AddinSearchFlags.IncludeAddins);
		}
		
		/// <summary>
		/// Gets all add-in roots registered in the registry.
		/// </summary>
		/// <returns>
		/// Descriptions of all add-in roots.
		/// </returns>
		public Addin[] GetAddinRoots ()
		{
			return GetModules (AddinSearchFlags.IncludeRoots);
		}
		
		/// <summary>
		/// Loads an add-in description
		/// </summary>
		/// <param name="progressStatus">
		/// Progress tracker.
		/// </param>
		/// <param name="file">
		/// Name of the file to load
		/// </param>
		/// <returns>
		/// An add-in description
		/// </returns>
		/// <remarks>
		/// This method loads an add-in description from a file. The file can be an XML manifest or an
		/// assembly that implements an add-in.
		/// </remarks>
		public AddinDescription GetAddinDescription (IProgressStatus progressStatus, string file)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return null;
			string outFile = Path.GetTempFileName ();
			try {
				database.ParseAddin (progressStatus, currentDomain, file, outFile, false);
			}
			catch {
				File.Delete (outFile);
				throw;
			}
			
			try {
				AddinDescription desc = AddinDescription.Read (outFile);
				if (desc != null) {
					desc.AddinFile = file;
					desc.OwnerDatabase = database;
				}
				return desc;
			}
			catch {
				// Errors are already reported using the progress status object
				return null;
			}
			finally {
				File.Delete (outFile);
			}
		}
		
		/// <summary>
		/// Reads an XML add-in manifest
		/// </summary>
		/// <param name="file">
		/// Path to the XML file
		/// </param>
		/// <returns>
		/// An add-in description
		/// </returns>
		public AddinDescription ReadAddinManifestFile (string file)
		{
			AddinDescription desc = AddinDescription.Read (file);
			if (currentDomain != AddinDatabase.UnknownDomain) {
				desc.OwnerDatabase = database;
				desc.Domain = currentDomain;
			}
			return desc;
		}

		/// <summary>
		/// Reads an XML add-in manifest
		/// </summary>
		/// <param name="reader">
		/// Reader that contains the XML
		/// </param>
		/// <param name="baseFile">
		/// Base path to use to discover add-in files
		/// </param>
		/// <returns>
		/// An add-in description
		/// </returns>
		public AddinDescription ReadAddinManifestFile (TextReader reader, string baseFile)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return null;
			AddinDescription desc = AddinDescription.Read (reader, baseFile);
			desc.OwnerDatabase = database;
			desc.Domain = currentDomain;
			return desc;
		}
		
		/// <summary>
		/// Checks whether an add-in is enabled.
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in.
		/// </param>
		/// <returns>
		/// 'true' if the add-in is enabled.
		/// </returns>
		public bool IsAddinEnabled (string id)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return false;
			return database.IsAddinEnabled (currentDomain, id);
		}
		
		/// <summary>
		/// Enables an add-in.
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in
		/// </param>
		/// <remarks>
		/// If the enabled add-in depends on other add-ins which are disabled,
		/// those will automatically be enabled too.
		/// </remarks>
		public void EnableAddin (string id)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return;
			database.EnableAddin (currentDomain, id, true);
		}
		
		/// <summary>
		/// Disables an add-in.
		/// </summary>
		/// <param name="id">
		/// Identifier of the add-in.
		/// </param>
		/// <remarks>
		/// When an add-in is disabled, all extension points it defines will be ignored
		/// by the add-in engine. Other add-ins which depend on the disabled add-in will
		/// also automatically be disabled.
		/// </remarks>
		public void DisableAddin (string id)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return;
			database.DisableAddin (currentDomain, id);
		}

		/// <summary>
		/// Registers a set of add-ins for uninstallation.
		/// </summary>
		/// <param name='id'>
		/// Identifier of the add-in
		/// </param>
		/// <param name='files'>
		/// Files to be uninstalled
		/// </param>
		/// <remarks>
		/// This method can be used to instruct the add-in manager to uninstall
		/// an add-in the next time the registry is updated. This is useful
		/// when an add-in manager can't delete an add-in because if it is
		/// loaded.
		/// </remarks>
		public void RegisterForUninstall (string id, IEnumerable<string> files)
		{
			database.RegisterForUninstall (currentDomain, id, files);
		}
		
		/// <summary>
		/// Determines whether an add-in is registered for uninstallation
		/// </summary>
		/// <returns>
		/// <c>true</c> if the add-in is registered for uninstallation
		/// </returns>
		/// <param name='addinId'>
		/// Identifier of the add-in
		/// </param>
		public bool IsRegisteredForUninstall (string addinId)
		{
			return database.IsRegisteredForUninstall (currentDomain, addinId);
		}
		
		/// <summary>
		/// Gets a value indicating whether there are pending add-ins to be uninstalled installed
		/// </summary>
		public bool HasPendingUninstalls {
			get { return database.HasPendingUninstalls (currentDomain); }
		}
		
		/// <summary>
		/// Internal use only
		/// </summary>
		public void DumpFile (string file)
		{
			Mono.Addins.Serialization.BinaryXmlReader.DumpFile (file);
		}
		
		/// <summary>
		/// Resets the configuration files of the registry
		/// </summary>
		public void ResetConfiguration ()
		{
			database.ResetConfiguration ();
		}
		
		internal void NotifyDatabaseUpdated ()
		{
			if (startupDirectory != null)
				currentDomain = database.GetFolderDomain (null, startupDirectory);
		}

		/// <summary>
		/// Updates the add-in registry.
		/// </summary>
		/// <remarks>
		/// This method must be called after modifying, installing or uninstalling add-ins.
		/// 
		/// When calling Update, every add-in added to the registry is parsed and validated,
		/// and if there is any error it will be rejected. It will also cache add-in information
		/// needed at run-time.
		/// 
		/// If during the update operation the registry finds new add-ins or detects that some
		/// add-ins have been deleted, the loaded extension points will be updated to include
		/// or exclude extension nodes from those add-ins.
		/// </remarks>
		public void Update ()
		{
			Update (new ConsoleProgressStatus (false));
		}

		/// <summary>
		/// Updates the add-in registry.
		/// </summary>
		/// <param name="monitor">
		/// Progress monitor to keep track of the update operation.
		/// </param>
		/// <remarks>
		/// This method must be called after modifying, installing or uninstalling add-ins.
		/// 
		/// When calling Update, every add-in added to the registry is parsed and validated,
		/// and if there is any error it will be rejected. It will also cache add-in information
		/// needed at run-time.
		/// 
		/// If during the update operation the registry finds new add-ins or detects that some
		/// add-ins have been deleted, the loaded extension points will be updated to include
		/// or exclude extension nodes from those add-ins.
		/// </remarks>
		public void Update (IProgressStatus monitor)
		{
			database.Update (monitor, currentDomain);
		}

		/// <summary>
		/// Regenerates the cached data of the add-in registry.
		/// </summary>
		/// <param name="monitor">
		/// Progress monitor to keep track of the rebuild operation.
		/// </param>
		public void Rebuild (IProgressStatus monitor)
		{
			database.Repair (monitor, currentDomain);

			// A full rebuild may cause the domain to change
			if (!string.IsNullOrEmpty (startupDirectory))
				currentDomain = database.GetFolderDomain (null, startupDirectory);
		}
		
		/// <summary>
		/// Registers an extension. Only AddinFileSystemExtension extensions are supported right now.
		/// </summary>
		/// <param name='extension'>
		/// The extension to register
		/// </param>
		public void RegisterExtension (object extension)
		{
			database.RegisterExtension (extension);
		}
		
		/// <summary>
		/// Unregisters an extension.
		/// </summary>
		/// <param name='extension'>
		/// The extension to unregister
		/// </param>
		public void UnregisterExtension (object extension)
		{
			database.UnregisterExtension (extension);
		}
		
		internal void CopyExtensionsFrom (AddinRegistry other)
		{
			database.CopyExtensions (other.database);
		}
		
		internal Addin GetAddinForHostAssembly (string filePath)
		{
			if (currentDomain == AddinDatabase.UnknownDomain)
				return null;
			return database.GetAddinForHostAssembly (currentDomain, filePath);
		}
		
		internal bool AddinDependsOn (string id1, string id2)
		{
			return database.AddinDependsOn (currentDomain, id1, id2);
		}
		
		internal void ScanFolders (IProgressStatus monitor, string folderToScan, StringCollection filesToIgnore)
		{
			database.ScanFolders (monitor, currentDomain, folderToScan, filesToIgnore);
		}
		
		internal void ParseAddin (IProgressStatus progressStatus, string file, string outFile)
		{
			database.ParseAddin (progressStatus, currentDomain, file, outFile, true);
		}
		
		/// <summary>
		/// Gets the default add-ins folder of the registry.
		/// </summary>
		/// <remarks>
		/// For every add-in registry there is an add-in folder where the registry will look for add-ins by default.
		/// This folder is an "addins" subdirectory of the directory where the repository is located. In most cases,
		/// this folder will only contain .addins files referencing other more convenient locations for add-ins.
		/// </remarks>
		public string DefaultAddinsFolder {
			get { return addinsDir; }
		}
		
		internal string AddinCachePath {
			get { return databaseDir; }
		}
	
		internal StringCollection GlobalAddinDirectories {
			get { return addinDirs; }
		}

		internal string StartupDirectory {
			get {
				return startupDirectory;
			}
		}
		
		internal bool CreateHostAddinsFile (string hostFile)
		{
			hostFile = Path.GetFullPath (hostFile);
			string baseName = Path.GetFileNameWithoutExtension (hostFile);
			if (!Directory.Exists (database.HostsPath))
				Directory.CreateDirectory (database.HostsPath);
			
			foreach (string s in Directory.GetFiles (database.HostsPath, baseName + "*.addins")) {
				try {
					using (StreamReader sr = new StreamReader (s)) {
						XmlTextReader tr = new XmlTextReader (sr);
						tr.MoveToContent ();
						string host = tr.GetAttribute ("host-reference");
						if (host == hostFile)
							return false;
					}
				}
				catch {
					// Ignore this file
				}
			}
			
			string file = Path.Combine (database.HostsPath, baseName) + ".addins";
			int n=1;
			while (File.Exists (file)) {
				file = Path.Combine (database.HostsPath, baseName) + "_" + n + ".addins";
				n++;
			}
			
			using (StreamWriter sw = new StreamWriter (file)) {
				XmlTextWriter tw = new XmlTextWriter (sw);
				tw.Formatting = Formatting.Indented;
				tw.WriteStartElement ("Addins");
				tw.WriteAttributeString ("host-reference", hostFile);
				tw.WriteStartElement ("Directory");
				tw.WriteAttributeString ("shared", "false");
				tw.WriteString (Path.GetDirectoryName (hostFile));
				tw.WriteEndElement ();
				tw.Close ();
			}
			return true;
		}
		
#pragma warning disable 1591
		[Obsolete]
		public static string[] GetRegisteredStartupFolders (string registryPath)
		{
			string dbDir = Path.Combine (registryPath, "addin-db-" + AddinDatabase.VersionTag);
			dbDir = Path.Combine (dbDir, "hosts");
			
			if (!Directory.Exists (dbDir))
				return new string [0];
			
			ArrayList dirs = new ArrayList ();
			
			foreach (string s in Directory.GetFiles (dbDir, "*.addins")) {
				try {
					using (StreamReader sr = new StreamReader (s)) {
						XmlTextReader tr = new XmlTextReader (sr);
						tr.MoveToContent ();
						string host = tr.GetAttribute ("host-reference");
						host = Path.GetDirectoryName (host);
						if (!dirs.Contains (host))
							dirs.Add (host);
					}
				}
				catch {
					// Ignore this file
				}
			}
			return (string[]) dirs.ToArray (typeof(string));
		}
#pragma warning restore 1591
	}
	
	/// <summary>
	/// Addin search flags.
	/// </summary>
	[Flags]
	public enum AddinSearchFlags
	{
		/// <summary>
		/// Add-ins are included in the search
		/// </summary>
		IncludeAddins = 1,
		/// <summary>
		/// Add-in roots are included in the search
		/// </summary>
		IncludeRoots = 1 << 1,
		/// <summary>
		/// Both add-in and add-in roots are included in the search
		/// </summary>
		IncludeAll = IncludeAddins | IncludeRoots,
		/// <summary>
		/// Only the latest version of every add-in or add-in root is included in the search
		/// </summary>
		LatestVersionsOnly = 1 << 3
	}
}
