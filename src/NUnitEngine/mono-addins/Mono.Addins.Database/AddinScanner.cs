//
// AddinScanner.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
using System.ComponentModel;

using Mono.Addins.Description;

namespace Mono.Addins.Database
{
	class AddinScanner: MarshalByRefObject
	{
		AddinDatabase database;
		AddinFileSystemExtension fs;
		Dictionary<IAssemblyReflector,object> coreAssemblies = new Dictionary<IAssemblyReflector, object> ();
		
		public AddinScanner (AddinDatabase database, AddinScanResult scanResult, IProgressStatus monitor)
		{
			this.database = database;
			fs = database.FileSystem;
		}
		
		public void ScanFolder (IProgressStatus monitor, string path, string domain, AddinScanResult scanResult)
		{
			path = Path.GetFullPath (path);
			
			// Avoid folders including each other
			if (!scanResult.VisitFolder (path))
				return;
			
			AddinScanFolderInfo folderInfo;
			if (!database.GetFolderInfoForPath (monitor, path, out folderInfo)) {
				// folderInfo file was corrupt.
				// Just in case, we are going to regenerate all relation data.
				if (!fs.DirectoryExists (path))
					scanResult.RegenerateRelationData = true;
			} else {
				// Directory is included but it doesn't exist. Ignore it.
				if (folderInfo == null && !fs.DirectoryExists (path))
					return;
			}
			
			// if domain is null it means that a new domain has to be created.
			
			bool sharedFolder = domain == AddinDatabase.GlobalDomain;
			bool isNewFolder = folderInfo == null;
			
			if (isNewFolder) {
				// No folder info. It is the first time this folder is scanned.
				// There is no need to store this object if the folder does not
				// contain add-ins.
				folderInfo = new AddinScanFolderInfo (path);
			}
			
			if (!sharedFolder && (folderInfo.SharedFolder || folderInfo.Domain != domain)) {
				// If the folder already has a domain, reuse it
				if (domain == null && folderInfo.RootsDomain != null && folderInfo.RootsDomain != AddinDatabase.GlobalDomain)
					domain = folderInfo.RootsDomain;
				else if (domain == null) {
					folderInfo.Domain = domain = database.GetUniqueDomainId ();
					scanResult.RegenerateRelationData = true;
				}
				else {
					folderInfo.Domain = domain;
					if (!isNewFolder) {
						// Domain has changed. Update the folder info and regenerate everything.
						scanResult.RegenerateRelationData = true;
						scanResult.RegisterModifiedFolderInfo (folderInfo);
					}
				}
			}
			else if (!folderInfo.SharedFolder && sharedFolder) {
				scanResult.RegenerateRelationData = true;
			}
			
			folderInfo.SharedFolder = sharedFolder;
			
			// If there is no domain assigned to the host, get one now
			if (scanResult.Domain == AddinDatabase.UnknownDomain)
				scanResult.Domain = domain;
			
			// Discard folders not belonging to the required domain
			if (scanResult.Domain != null && domain != scanResult.Domain && domain != AddinDatabase.GlobalDomain) {
				return;
			}
			
			if (monitor.LogLevel > 1 && !scanResult.LocateAssembliesOnly)
				monitor.Log ("Checking: " + path);
			
			if (fs.DirectoryExists (path))
			{
				IEnumerable<string> files = fs.GetFiles (path);
				
				// First of all, look for .addin files. Addin files must be processed before
				// assemblies, because they may add files to the ignore list (i.e., assemblies
				// included in .addin files won't be scanned twice).
				foreach (string file in files) {
					if (file.EndsWith (".addin.xml") || file.EndsWith (".addin")) {
						RegisterFileToScan (monitor, file, scanResult, folderInfo);
					}
				}
				
				// Now scan assemblies. They can also add files to the ignore list.
				
				foreach (string file in files) {
					string ext = Path.GetExtension (file).ToLower ();
					if (ext == ".dll" || ext == ".exe") {
						RegisterFileToScan (monitor, file, scanResult, folderInfo);
						scanResult.AddAssemblyLocation (file);
					}
				}
				
				// Finally scan .addins files
				
				foreach (string file in files) {
					if (Path.GetExtension (file).EndsWith (".addins")) {
						ScanAddinsFile (monitor, file, domain, scanResult);
					}
				}
			}
			else if (!scanResult.LocateAssembliesOnly) {
				// The folder has been deleted. All add-ins defined in that folder should also be deleted.
				scanResult.RegenerateRelationData = true;
				scanResult.ChangesFound = true;
				if (scanResult.CheckOnly)
					return;
				database.DeleteFolderInfo (monitor, folderInfo);
			}
			
			if (scanResult.LocateAssembliesOnly)
				return;
			
			// Look for deleted add-ins.
			
			UpdateDeletedAddins (monitor, folderInfo, scanResult);
		}
		
		public void UpdateDeletedAddins (IProgressStatus monitor, AddinScanFolderInfo folderInfo, AddinScanResult scanResult)
		{
			ArrayList missing = folderInfo.GetMissingAddins (fs);
			if (missing.Count > 0) {
				if (fs.DirectoryExists (folderInfo.Folder))
					scanResult.RegisterModifiedFolderInfo (folderInfo);
				scanResult.ChangesFound = true;
				if (scanResult.CheckOnly)
					return;
					
				foreach (AddinFileInfo info in missing) {
					database.UninstallAddin (monitor, info.Domain, info.AddinId, info.File, scanResult);
				}
			}
		}
		
		void RegisterFileToScan (IProgressStatus monitor, string file, AddinScanResult scanResult, AddinScanFolderInfo folderInfo)
		{
			if (scanResult.LocateAssembliesOnly)
				return;

			AddinFileInfo finfo = folderInfo.GetAddinFileInfo (file);
			bool added = false;
			   
			if (finfo != null && (!finfo.IsAddin || finfo.Domain == folderInfo.GetDomain (finfo.IsRoot)) && fs.GetLastWriteTime (file) == finfo.LastScan && !scanResult.RegenerateAllData) {
				if (finfo.ScanError) {
					// Always schedule the file for scan if there was an error in a previous scan.
					// However, don't set ChangesFound=true, in this way if there isn't any other
					// change in the registry, the file won't be scanned again.
					scanResult.AddFileToScan (file, folderInfo);
					added = true;
				}
			
				if (!finfo.IsAddin)
					return;
				
				if (database.AddinDescriptionExists (finfo.Domain, finfo.AddinId)) {
					// It is an add-in and it has not changed. Paths in the ignore list
					// are still valid, so they can be used.
					if (finfo.IgnorePaths != null)
						scanResult.AddPathsToIgnore (finfo.IgnorePaths);
					return;
				}
			}
			
			scanResult.ChangesFound = true;
			
			if (!scanResult.CheckOnly && !added)
				scanResult.AddFileToScan (file, folderInfo);
		}
		
		public void ScanFile (IProgressStatus monitor, string file, AddinScanFolderInfo folderInfo, AddinScanResult scanResult)
		{
			if (scanResult.IgnorePath (file)) {
				// The file must be ignored. Maybe it caused a crash in a previous scan, or it
				// might be included by a .addin file (in which case it will be scanned when processing
				// the .addin file).
				folderInfo.SetLastScanTime (file, null, false, fs.GetLastWriteTime (file), true);
				return;
			}
			
			string ext = Path.GetExtension (file).ToLower ();
			if ((ext == ".dll" || ext == ".exe") && !Util.IsManagedAssembly (file)) {
				// Ignore dlls and exes which are not managed assemblies
				folderInfo.SetLastScanTime (file, null, false, fs.GetLastWriteTime (file), true);
				return;
			}

			if (monitor.LogLevel > 1)
				monitor.Log ("Scanning file: " + file);
			
			// Log the file to be scanned, so in case of a process crash the main process
			// will know what crashed
			monitor.Log ("plog:scan:" + file);
			
			string scannedAddinId = null;
			bool scannedIsRoot = false;
			bool scanSuccessful = false;
			AddinDescription config = null;
			
			try {
				if (ext == ".dll" || ext == ".exe")
					scanSuccessful = ScanAssembly (monitor, file, scanResult, out config);
				else
					scanSuccessful = ScanConfigAssemblies (monitor, file, scanResult, out config);

				if (config != null) {
					
					AddinFileInfo fi = folderInfo.GetAddinFileInfo (file);
					
					// If version is not specified, make up one
					if (config.Version.Length == 0) {
						config.Version = "0.0.0.0";
					}
					
					if (config.LocalId.Length == 0) {
						// Generate an internal id for this add-in
						config.LocalId = database.GetUniqueAddinId (file, (fi != null ? fi.AddinId : null), config.Namespace, config.Version);
						config.HasUserId = false;
					}
					
					// Check errors in the description
					StringCollection errors = config.Verify (fs);
					
					if (database.IsGlobalRegistry && config.AddinId.IndexOf ('.') == -1) {
						errors.Add ("Add-ins registered in the global registry must have a namespace.");
					}
					    
					if (errors.Count > 0) {
						scanSuccessful = false;
						monitor.ReportError ("Errors found in add-in '" + file + ":", null);
						foreach (string err in errors)
							monitor.ReportError (err, null);
					}
				
					// Make sure all extensions sets are initialized with the correct add-in id
					
					config.SetExtensionsAddinId (config.AddinId);
					
					scanResult.ChangesFound = true;
					
					// If the add-in already existed, try to reuse the relation data it had.
					// Also, the dependencies of the old add-in need to be re-analized
					
					AddinDescription existingDescription = null;
					bool res = database.GetAddinDescription (monitor, folderInfo.Domain, config.AddinId, config.AddinFile, out existingDescription);
					
					// If we can't get information about the old assembly, just regenerate all relation data
					if (!res)
						scanResult.RegenerateRelationData = true;
					
					string replaceFileName = null;
					
					if (existingDescription != null) {
						// Reuse old relation data
						config.MergeExternalData (existingDescription);
						Util.AddDependencies (existingDescription, scanResult);
						replaceFileName = existingDescription.FileName;
					}
					
					// If the scanned file results in an add-in version different from the one obtained from
					// previous scans, the old add-in needs to be uninstalled.
					if (fi != null && fi.IsAddin && fi.AddinId != config.AddinId) {
						database.UninstallAddin (monitor, folderInfo.Domain, fi.AddinId, fi.File, scanResult);
						
						// If the add-in version has changed, regenerate everything again since old data can't be reused
						if (Addin.GetIdName (fi.AddinId) == Addin.GetIdName (config.AddinId))
							scanResult.RegenerateRelationData = true;
					}
					
					// If a description could be generated, save it now (if the scan was successful)
					if (scanSuccessful) {
						
						// Assign the domain
						if (config.IsRoot) {
							if (folderInfo.RootsDomain == null) {
								if (scanResult.Domain != null && scanResult.Domain != AddinDatabase.UnknownDomain && scanResult.Domain != AddinDatabase.GlobalDomain)
									folderInfo.RootsDomain = scanResult.Domain;
								else
									folderInfo.RootsDomain = database.GetUniqueDomainId ();
							}
							config.Domain = folderInfo.RootsDomain;
						} else
							config.Domain = folderInfo.Domain;
						
						if (config.IsRoot && scanResult.HostIndex != null) {
							// If the add-in is a root, register its assemblies
							foreach (string f in config.MainModule.Assemblies) {
								string asmFile = Path.Combine (config.BasePath, f);
								scanResult.HostIndex.RegisterAssembly (asmFile, config.AddinId, config.AddinFile, config.Domain);
							}
						}
						
						// Finally save
						
						if (database.SaveDescription (monitor, config, replaceFileName)) {
							// The new dependencies also have to be updated
							Util.AddDependencies (config, scanResult);
							scanResult.AddAddinToUpdate (config.AddinId);
							scannedAddinId = config.AddinId;
							scannedIsRoot = config.IsRoot;
							return;
						}
					}
				}
			}
			catch (Exception ex) {
				monitor.ReportError ("Unexpected error while scanning file: " + file, ex);
			}
			finally {
				AddinFileInfo ainfo = folderInfo.SetLastScanTime (file, scannedAddinId, scannedIsRoot, fs.GetLastWriteTime (file), !scanSuccessful);
				
				if (scanSuccessful && config != null) {
					// Update the ignore list in the folder info object. To be used in the next scan
					foreach (string df in config.AllIgnorePaths) {
						string path = Path.Combine (config.BasePath, df);
						ainfo.AddPathToIgnore (Path.GetFullPath (path));
					}
				}
				
				monitor.Log ("plog:endscan");
			}
		}
		
		public AddinDescription ScanSingleFile (IProgressStatus monitor, string file, AddinScanResult scanResult)
		{
			AddinDescription config = null;
			
			if (monitor.LogLevel > 1)
				monitor.Log ("Scanning file: " + file);
				
			monitor.Log ("plog:scan:" + file);
			
			try {
				string ext = Path.GetExtension (file).ToLower ();
				bool scanSuccessful;
				
				if (ext == ".dll" || ext == ".exe")
					scanSuccessful = ScanAssembly (monitor, file, scanResult, out config);
				else
					scanSuccessful = ScanConfigAssemblies (monitor, file, scanResult, out config);

				if (scanSuccessful && config != null) {
					
					config.Domain = "global";
					if (config.Version.Length == 0)
						config.Version = "0.0.0.0";
					
					if (config.LocalId.Length == 0) {
						// Generate an internal id for this add-in
						config.LocalId = database.GetUniqueAddinId (file, "", config.Namespace, config.Version);
					}
				}
			}
			catch (Exception ex) {
				monitor.ReportError ("Unexpected error while scanning file: " + file, ex);
			} finally {
				monitor.Log ("plog:endscan");
			}
			return config;
		}
		
		public void ScanAddinsFile (IProgressStatus monitor, string file, string domain, AddinScanResult scanResult)
		{
			XmlTextReader r = null;
			ArrayList directories = new ArrayList ();
			ArrayList directoriesWithSubdirs = new ArrayList ();
			string basePath = Path.GetDirectoryName (file);
			
			try {
				r = new XmlTextReader (fs.OpenTextFile (file));
				r.MoveToContent ();
				if (r.IsEmptyElement)
					return;
				r.ReadStartElement ();
				r.MoveToContent ();
				while (r.NodeType != XmlNodeType.EndElement) {
					if (r.NodeType == XmlNodeType.Element && r.LocalName == "Directory") {
						string subs = r.GetAttribute ("include-subdirs");
						string sdom;
						string share = r.GetAttribute ("shared");
						if (share == "true")
							sdom = AddinDatabase.GlobalDomain;
						else if (share == "false")
							sdom = null;
						else
							sdom = domain; // Inherit the domain
						
						string path = r.ReadElementString ().Trim ();
						if (path.Length > 0) {
							path = Util.NormalizePath (path);
							if (subs == "true")
								directoriesWithSubdirs.Add (new string[] {path, sdom});
							else
								directories.Add (new string[] {path, sdom});
						}
					}
					else if (r.NodeType == XmlNodeType.Element && r.LocalName == "GacAssembly") {
						string aname = r.ReadElementString ().Trim ();
						if (aname.Length > 0) {
							aname = Util.NormalizePath (aname);
							aname = Util.GetGacPath (aname);
							if (aname != null) {
								// Gac assemblies always use the global domain
								directories.Add (new string[] {aname, AddinDatabase.GlobalDomain});
							}
						}
					}
					else if (r.NodeType == XmlNodeType.Element && r.LocalName == "Exclude") {
						string path = r.ReadElementString ().Trim ();
						if (path.Length > 0) {
							path = Util.NormalizePath (path);
							if (!Path.IsPathRooted (path))
								path = Path.Combine (basePath, path);
							scanResult.AddPathToIgnore (Path.GetFullPath (path));
						}
					}
					else
						r.Skip ();
					r.MoveToContent ();
				}
			} catch (Exception ex) {
				monitor.ReportError ("Could not process addins file: " + file, ex);
				return;
			} finally {
				if (r != null)
					r.Close ();
			}
			
			foreach (string[] d in directories) {
				string dir = d[0];
				if (!Path.IsPathRooted (dir))
					dir = Path.Combine (basePath, dir);
				ScanFolder (monitor, dir, d[1], scanResult);
			}
			foreach (string[] d in directoriesWithSubdirs) {
				string dir = d[0];
				if (!Path.IsPathRooted (dir))
					dir = Path.Combine (basePath, dir);
				ScanFolderRec (monitor, dir, d[1], scanResult);
			}
		}
		
		public void ScanFolderRec (IProgressStatus monitor, string dir, string domain, AddinScanResult scanResult)
		{
			ScanFolder (monitor, dir, domain, scanResult);
			
			if (!fs.DirectoryExists (dir))
				return;
				
			foreach (string sd in fs.GetDirectories (dir))
				ScanFolderRec (monitor, sd, domain, scanResult);
		}
		
		bool ScanConfigAssemblies (IProgressStatus monitor, string filePath, AddinScanResult scanResult, out AddinDescription config)
		{
			config = null;
			
			try {
				IAssemblyReflector reflector = GetReflector (monitor, scanResult, filePath);
				
				string basePath = Path.GetDirectoryName (filePath);
				
				using (var s = fs.OpenFile (filePath)) {
					config = AddinDescription.Read (s, basePath);
				}
				config.FileName = filePath;
				config.SetBasePath (basePath);
				config.AddinFile = filePath;
				
				return ScanDescription (monitor, reflector, config, null, scanResult);
			}
			catch (Exception ex) {
				// Something went wrong while scanning the assembly. We'll ignore it for now.
				monitor.ReportError ("There was an error while scanning add-in: " + filePath, ex);
				return false;
			}
		}
		
		IAssemblyReflector GetReflector (IProgressStatus monitor, AddinScanResult scanResult, string filePath)
		{
			IAssemblyReflector reflector = fs.GetReflectorForFile (scanResult, filePath);
			object coreAssembly;
			if (!coreAssemblies.TryGetValue (reflector, out coreAssembly)) {
				if (monitor.LogLevel > 1)
					monitor.Log ("Using assembly reflector: " + reflector.GetType ());
				coreAssemblies [reflector] = coreAssembly = reflector.LoadAssembly (GetType().Assembly.Location);
			}
			return reflector;
		}
		
		bool ScanAssembly (IProgressStatus monitor, string filePath, AddinScanResult scanResult, out AddinDescription config)
		{
			config = null;
				
			try {
				IAssemblyReflector reflector = GetReflector (monitor, scanResult, filePath);
				object asm = reflector.LoadAssembly (filePath);
				if (asm == null)
					throw new Exception ("Could not load assembly: " + filePath);
				
				// Get the config file from the resources, if there is one
				
				if (!ScanEmbeddedDescription (monitor, filePath, reflector, asm, out config))
					return false;
				
				if (config == null || config.IsExtensionModel) {
					// In this case, only scan the assembly if it has the Addin attribute.
					AddinAttribute att = (AddinAttribute) reflector.GetCustomAttribute (asm, typeof(AddinAttribute), false);
					if (att == null) {
						config = null;
						return true;
					}

					if (config == null)
						config = new AddinDescription ();
				}
				
				config.SetBasePath (Path.GetDirectoryName (filePath));
				config.AddinFile = filePath;
				
				string rasmFile = Path.GetFileName (filePath);
				if (!config.MainModule.Assemblies.Contains (rasmFile))
					config.MainModule.Assemblies.Add (rasmFile);
				
				return ScanDescription (monitor, reflector, config, asm, scanResult);
			}
			catch (Exception ex) {
				// Something went wrong while scanning the assembly. We'll ignore it for now.
				monitor.ReportError ("There was an error while scanning assembly: " + filePath, ex);
				return false;
			}
		}

		static bool ScanEmbeddedDescription (IProgressStatus monitor, string filePath, IAssemblyReflector reflector, object asm, out AddinDescription config)
		{
			config = null;
			foreach (string res in reflector.GetResourceNames (asm)) {
				if (res.EndsWith (".addin") || res.EndsWith (".addin.xml")) {
					using (Stream s = reflector.GetResourceStream (asm, res)) {
						AddinDescription ad = AddinDescription.Read (s, Path.GetDirectoryName (filePath));
						if (config != null) {
							if (!config.IsExtensionModel && !ad.IsExtensionModel) {
								// There is more than one add-in definition
								monitor.ReportError ("Duplicate add-in definition found in assembly: " + filePath, null);
								return false;
							}
							config = AddinDescription.Merge (config, ad);
						}
						else
							config = ad;
					}
				}
			}
			return true;
		}

		bool ScanDescription (IProgressStatus monitor, IAssemblyReflector reflector, AddinDescription config, object rootAssembly, AddinScanResult scanResult)
		{
			// First of all scan the main module
			
			ArrayList assemblies = new ArrayList ();
			
			try {
				string rootAsmFile = null;
				
				if (rootAssembly != null) {
					ScanAssemblyAddinHeaders (reflector, config, rootAssembly, scanResult);
					ScanAssemblyImports (reflector, config.MainModule, rootAssembly);
					assemblies.Add (rootAssembly);
					rootAsmFile = Path.GetFileName (config.AddinFile);
				}
				
				// The assembly list may be modified while scanning the headears, so
				// we use a for loop instead of a foreach
				for (int n=0; n<config.MainModule.Assemblies.Count; n++) {
					string s = config.MainModule.Assemblies [n];
					string asmFile = Path.GetFullPath (Path.Combine (config.BasePath, s));
					scanResult.AddPathToIgnore (asmFile);
					if (s == rootAsmFile || config.MainModule.IgnorePaths.Contains (s))
						continue;
					object asm = reflector.LoadAssembly (asmFile);
					assemblies.Add (asm);
					ScanAssemblyAddinHeaders (reflector, config, asm, scanResult);
					ScanAssemblyImports (reflector, config.MainModule, asm);
				}
				
				// Add all data files to the ignore file list. It avoids scanning assemblies
				// which are included as 'data' in an add-in.
				foreach (string df in config.MainModule.DataFiles) {
					string file = Path.Combine (config.BasePath, df);
					scanResult.AddPathToIgnore (Path.GetFullPath (file));
				}
				foreach (string df in config.MainModule.IgnorePaths) {
					string path = Path.Combine (config.BasePath, df);
					scanResult.AddPathToIgnore (Path.GetFullPath (path));
				}
				
				// The add-in id and version must be already assigned at this point
				
				// Clean host data from the index. New data will be added.
				if (scanResult.HostIndex != null)
					scanResult.HostIndex.RemoveHostData (config.AddinId, config.AddinFile);

				foreach (object asm in assemblies)
					ScanAssemblyContents (reflector, config, config.MainModule, asm, scanResult);
				
			} catch (Exception ex) {
				ReportReflectionException (monitor, ex, config, scanResult);
				return false;
			}
			
			// Extension node types may have child nodes declared as attributes. Find them.
			
			Hashtable internalNodeSets = new Hashtable ();
			
			ArrayList setsCopy = new ArrayList ();
			setsCopy.AddRange (config.ExtensionNodeSets);
			foreach (ExtensionNodeSet eset in setsCopy)
				ScanNodeSet (reflector, config, eset, assemblies, internalNodeSets);
			
			foreach (ExtensionPoint ep in config.ExtensionPoints) {
				ScanNodeSet (reflector, config, ep.NodeSet, assemblies, internalNodeSets);
			}
		
			// Now scan all modules
			
			if (!config.IsRoot) {
				foreach (ModuleDescription mod in config.OptionalModules) {
					try {
						var asmList = new List<Tuple<string,object>> ();
						for (int n=0; n<mod.Assemblies.Count; n++) {
							string s = mod.Assemblies [n];
							if (mod.IgnorePaths.Contains (s))
								continue;
							string asmFile = Path.Combine (config.BasePath, s);
							object asm = reflector.LoadAssembly (asmFile);
							asmList.Add (new Tuple<string,object> (asmFile,asm));
							scanResult.AddPathToIgnore (Path.GetFullPath (asmFile));
							ScanAssemblyImports (reflector, mod, asm);
						}
						// Add all data files to the ignore file list. It avoids scanning assemblies
						// which are included as 'data' in an add-in.
						foreach (string df in mod.DataFiles) {
							string file = Path.Combine (config.BasePath, df);
							scanResult.AddPathToIgnore (Path.GetFullPath (file));
						}
						foreach (string df in mod.IgnorePaths) {
							string path = Path.Combine (config.BasePath, df);
							scanResult.AddPathToIgnore (Path.GetFullPath (path));
						}
						
						foreach (var asm in asmList)
							ScanSubmodule (monitor, mod, reflector, config, scanResult, asm.Item1, asm.Item2);

					} catch (Exception ex) {
						ReportReflectionException (monitor, ex, config, scanResult);
					}
				}
			}
			
			config.StoreFileInfo ();
			return true;
		}

		bool ScanSubmodule (IProgressStatus monitor, ModuleDescription mod, IAssemblyReflector reflector, AddinDescription config, AddinScanResult scanResult, string assemblyName, object asm)
		{
			AddinDescription mconfig;
			ScanEmbeddedDescription (monitor, assemblyName, reflector, asm, out mconfig);
			if (mconfig != null) {
				if (!mconfig.IsExtensionModel) {
					monitor.ReportError ("Submodules can't define new add-ins: " + assemblyName, null);
					return false;
				}
				if (mconfig.OptionalModules.Count != 0) {
					monitor.ReportError ("Submodules can't define nested submodules: " + assemblyName, null);
					return false;
				}
				if (mconfig.ConditionTypes.Count != 0) {
					monitor.ReportError ("Submodules can't define condition types: " + assemblyName, null);
					return false;
				}
				if (mconfig.ExtensionNodeSets.Count != 0) {
					monitor.ReportError ("Submodules can't define extension node sets: " + assemblyName, null);
					return false;
				}
				if (mconfig.ExtensionPoints.Count != 0) {
					monitor.ReportError ("Submodules can't define extension points sets: " + assemblyName, null);
					return false;
				}
				mod.MergeWith (mconfig.MainModule);
			}
			ScanAssemblyContents (reflector, config, mod, asm, scanResult);
			return true;
		}

		void ReportReflectionException (IProgressStatus monitor, Exception ex, AddinDescription config, AddinScanResult scanResult)
		{
			scanResult.AddFileToWithFailure (config.AddinFile);
			monitor.ReportWarning ("[" + config.AddinId + "] Could not load some add-in assemblies: " + ex.Message);
			if (monitor.LogLevel <= 1)
			    return;
			
			ReflectionTypeLoadException rex = ex as ReflectionTypeLoadException;
			if (rex != null) {
				foreach (Exception e in rex.LoaderExceptions)
					monitor.Log ("Load exception: " + e);
			}
		}
		
		void ScanAssemblyAddinHeaders (IAssemblyReflector reflector, AddinDescription config, object asm, AddinScanResult scanResult)
		{
			// Get basic add-in information
			AddinAttribute att = (AddinAttribute) reflector.GetCustomAttribute (asm, typeof(AddinAttribute), false);
			if (att != null) {
				if (att.Id.Length > 0)
					config.LocalId = att.Id;
				if (att.Version.Length > 0)
					config.Version = att.Version;
				if (att.Namespace.Length > 0)
					config.Namespace = att.Namespace;
				if (att.Category.Length > 0)
					config.Category = att.Category;
				if (att.CompatVersion.Length > 0)
					config.CompatVersion = att.CompatVersion;
				if (att.Url.Length > 0)
					config.Url = att.Url;
				config.IsRoot = att is AddinRootAttribute;
				config.EnabledByDefault = att.EnabledByDefault;
				config.Flags = att.Flags;
			}
			
			// Author attributes
			
			object[] atts = reflector.GetCustomAttributes (asm, typeof(AddinAuthorAttribute), false);
			foreach (AddinAuthorAttribute author in atts) {
				if (config.Author.Length == 0)
					config.Author = author.Name;
				else
					config.Author += ", " + author.Name;
			}
			
			// Name
			
			atts = reflector.GetCustomAttributes (asm, typeof(AddinNameAttribute), false);
			foreach (AddinNameAttribute at in atts) {
				if (string.IsNullOrEmpty (at.Locale))
					config.Name = at.Name;
				else
					config.Properties.SetPropertyValue ("Name", at.Name, at.Locale);
			}
			
			// Description
			
			object catt = reflector.GetCustomAttribute (asm, typeof(AssemblyDescriptionAttribute), false);
			if (catt != null && config.Description.Length == 0)
				config.Description = ((AssemblyDescriptionAttribute)catt).Description;
			
			atts = reflector.GetCustomAttributes (asm, typeof(AddinDescriptionAttribute), false);
			foreach (AddinDescriptionAttribute at in atts) {
				if (string.IsNullOrEmpty (at.Locale))
					config.Description = at.Description;
				else
					config.Properties.SetPropertyValue ("Description", at.Description, at.Locale);
			}
			
			// Copyright
			
			catt = reflector.GetCustomAttribute (asm, typeof(AssemblyCopyrightAttribute), false);
			if (catt != null && config.Copyright.Length == 0)
				config.Copyright = ((AssemblyCopyrightAttribute)catt).Copyright;
			
			// Category

			catt = reflector.GetCustomAttribute (asm, typeof(AddinCategoryAttribute), false);
			if (catt != null && config.Category.Length == 0)
				config.Category = ((AddinCategoryAttribute)catt).Category;
			
			// Url

			catt = reflector.GetCustomAttribute (asm, typeof(AddinUrlAttribute), false);
			if (catt != null && config.Url.Length == 0)
				config.Url = ((AddinUrlAttribute)catt).Url;
			
			// Flags

			catt = reflector.GetCustomAttribute (asm, typeof(AddinFlagsAttribute), false);
			if (catt != null)
				config.Flags |= ((AddinFlagsAttribute)catt).Flags;

			// Localizer
			
			AddinLocalizerGettextAttribute locat = (AddinLocalizerGettextAttribute) reflector.GetCustomAttribute (asm, typeof(AddinLocalizerGettextAttribute), false);
			if (locat != null) {
				ExtensionNodeDescription node = new ExtensionNodeDescription ();
				if (!string.IsNullOrEmpty (locat.Catalog))
					node.SetAttribute ("catalog", locat.Catalog);
				if (!string.IsNullOrEmpty (locat.Location))
					node.SetAttribute ("location", locat.Catalog);
				config.Localizer = node;
			}
			
			// Optional modules
			
			atts = reflector.GetCustomAttributes (asm, typeof(AddinModuleAttribute), false);
			foreach (AddinModuleAttribute mod in atts) {
				if (mod.AssemblyFile.Length > 0) {
					ModuleDescription module = new ModuleDescription ();
					module.Assemblies.Add (mod.AssemblyFile);
					config.OptionalModules.Add (module);
				}
			}
		}
		
		void ScanAssemblyImports (IAssemblyReflector reflector, ModuleDescription module, object asm)
		{
			object[] atts = reflector.GetCustomAttributes (asm, typeof(ImportAddinAssemblyAttribute), false);
			foreach (ImportAddinAssemblyAttribute import in atts) {
				if (!string.IsNullOrEmpty (import.FilePath)) {
					module.Assemblies.Add (import.FilePath);
					if (!import.Scan)
						module.IgnorePaths.Add (import.FilePath);
				}
			}
			atts = reflector.GetCustomAttributes (asm, typeof(ImportAddinFileAttribute), false);
			foreach (ImportAddinFileAttribute import in atts) {
				if (!string.IsNullOrEmpty (import.FilePath))
					module.DataFiles.Add (import.FilePath);
			}
		}
		
		void ScanAssemblyContents (IAssemblyReflector reflector, AddinDescription config, ModuleDescription module, object asm, AddinScanResult scanResult)
		{
			bool isMainModule = module == config.MainModule;
			
			// Get dependencies
			
			object[] deps = reflector.GetCustomAttributes (asm, typeof(AddinDependencyAttribute), false);
			foreach (AddinDependencyAttribute dep in deps) {
				AddinDependency adep = new AddinDependency ();
				adep.AddinId = dep.Id;
				adep.Version = dep.Version;
				module.Dependencies.Add (adep);
			}
			
			if (isMainModule) {
				
				// Get properties
				
				object[] props = reflector.GetCustomAttributes (asm, typeof(AddinPropertyAttribute), false);
				foreach (AddinPropertyAttribute prop in props)
					config.Properties.SetPropertyValue (prop.Name, prop.Value, prop.Locale);
			
				// Get extension points
				
				object[] extPoints = reflector.GetCustomAttributes (asm, typeof(ExtensionPointAttribute), false);
				foreach (ExtensionPointAttribute ext in extPoints) {
					ExtensionPoint ep = config.AddExtensionPoint (ext.Path);
					ep.Description = ext.Description;
					ep.Name = ext.Name;
					ExtensionNodeType nt = ep.AddExtensionNode (ext.NodeName, ext.NodeTypeName);
					nt.ExtensionAttributeTypeName = ext.ExtensionAttributeTypeName;
				}
			}
			
			// Look for extension nodes declared using assembly attributes
			
			foreach (CustomAttribute att in reflector.GetRawCustomAttributes (asm, typeof(CustomExtensionAttribute), true))
				AddCustomAttributeExtension (module, att, "Type");
			
			// Get extensions or extension points applied to types
			
			foreach (object t in reflector.GetAssemblyTypes (asm)) {
				
				string typeFullName = reflector.GetTypeFullName (t);
				
				// Look for extensions
				
				object[] extensionAtts = reflector.GetCustomAttributes (t, typeof(ExtensionAttribute), false);
				if (extensionAtts.Length > 0) {
					Dictionary<string,ExtensionNodeDescription> nodes = new Dictionary<string, ExtensionNodeDescription> ();
					ExtensionNodeDescription uniqueNode = null;
					foreach (ExtensionAttribute eatt in extensionAtts) {
						string path;
						string nodeName = eatt.NodeName;
						
						if (eatt.TypeName.Length > 0) {
							path = "$" + eatt.TypeName;
						}
						else if (eatt.Path.Length == 0) {
							path = GetBaseTypeNameList (reflector, t);
							if (path == "$") {
								// The type does not implement any interface and has no superclass.
								// Will be reported later as an error.
								path = "$" + typeFullName;
							}
						} else {
							path = eatt.Path;
						}

						ExtensionNodeDescription elem = module.AddExtensionNode (path, nodeName);
						nodes [path] = elem;
						uniqueNode = elem;
						
						if (eatt.Id.Length > 0) {
							elem.SetAttribute ("id", eatt.Id);
							elem.SetAttribute ("type", typeFullName);
						} else {
							elem.SetAttribute ("id", typeFullName);
						}
						if (eatt.InsertAfter.Length > 0)
							elem.SetAttribute ("insertafter", eatt.InsertAfter);
						if (eatt.InsertBefore.Length > 0)
							elem.SetAttribute ("insertbefore", eatt.InsertBefore);
					}
					
					// Get the node attributes
					
					foreach (ExtensionAttributeAttribute eat in reflector.GetCustomAttributes (t, typeof(ExtensionAttributeAttribute), false)) {
						ExtensionNodeDescription node;
						if (!string.IsNullOrEmpty (eat.Path))
							nodes.TryGetValue (eat.Path, out node);
						else if (eat.TypeName.Length > 0)
							nodes.TryGetValue ("$" + eat.TypeName, out node);
						else {
							if (nodes.Count > 1)
								throw new Exception ("Missing type or extension path value in ExtensionAttribute for type '" + typeFullName + "'.");
							node = uniqueNode;
						}
						if (node == null)
							throw new Exception ("Invalid type or path value in ExtensionAttribute for type '" + typeFullName + "'.");
							
						node.SetAttribute (eat.Name ?? string.Empty, eat.Value ?? string.Empty);
					}
				}
				else {
					// Look for extension points
					
					extensionAtts = reflector.GetCustomAttributes (t, typeof(TypeExtensionPointAttribute), false);
					if (extensionAtts.Length > 0 && isMainModule) {
						foreach (TypeExtensionPointAttribute epa in extensionAtts) {
							ExtensionPoint ep;
							
							ExtensionNodeType nt = new ExtensionNodeType ();
							
							if (epa.Path.Length > 0) {
								ep = config.AddExtensionPoint (epa.Path);
							}
							else {
								ep = config.AddExtensionPoint (GetDefaultTypeExtensionPath (config, typeFullName));
								nt.ObjectTypeName = typeFullName;
							}
							nt.Id = epa.NodeName;
							nt.TypeName = epa.NodeTypeName;
							nt.ExtensionAttributeTypeName = epa.ExtensionAttributeTypeName;
							ep.NodeSet.NodeTypes.Add (nt);
							ep.Description = epa.Description;
							ep.Name = epa.Name;
							ep.RootAddin = config.AddinId;
							ep.SetExtensionsAddinId (config.AddinId);
						}
					}
					else {
						// Look for custom extension attribtues
						foreach (CustomAttribute att in reflector.GetRawCustomAttributes (t, typeof(CustomExtensionAttribute), false)) {
							ExtensionNodeDescription elem = AddCustomAttributeExtension (module, att, "Type");
							elem.SetAttribute ("type", typeFullName);
							if (string.IsNullOrEmpty (elem.GetAttribute ("id")))
								elem.SetAttribute ("id", typeFullName);
						}
					}
				}
			}
		}
		
		ExtensionNodeDescription AddCustomAttributeExtension (ModuleDescription module, CustomAttribute att, string nameName)
		{
			string path;
			if (!att.TryGetValue (CustomExtensionAttribute.PathFieldKey, out path))
				path = "%" + att.TypeName;
			ExtensionNodeDescription elem = module.AddExtensionNode (path, nameName);
			foreach (KeyValuePair<string,string> prop in att) {
				if (prop.Key != CustomExtensionAttribute.PathFieldKey)
					elem.SetAttribute (prop.Key, prop.Value);
			}
			return elem;
		}
		
		void ScanNodeSet (IAssemblyReflector reflector, AddinDescription config, ExtensionNodeSet nset, ArrayList assemblies, Hashtable internalNodeSets)
		{
			foreach (ExtensionNodeType nt in nset.NodeTypes)
				ScanNodeType (reflector, config, nt, assemblies, internalNodeSets);
		}
		
		void ScanNodeType (IAssemblyReflector reflector, AddinDescription config, ExtensionNodeType nt, ArrayList assemblies, Hashtable internalNodeSets)
		{
			if (nt.TypeName.Length == 0)
				nt.TypeName = "Mono.Addins.TypeExtensionNode";
			
			object ntype = FindAddinType (reflector, nt.TypeName, assemblies);
			if (ntype == null)
				return;
			
			// Add type information declared with attributes in the code
			ExtensionNodeAttribute nodeAtt = (ExtensionNodeAttribute) reflector.GetCustomAttribute (ntype, typeof(ExtensionNodeAttribute), true);
			if (nodeAtt != null) {
				if (nt.Id.Length == 0 && nodeAtt.NodeName.Length > 0)
					nt.Id = nodeAtt.NodeName;
				if (nt.Description.Length == 0 && nodeAtt.Description.Length > 0)
					nt.Description = nodeAtt.Description;
				if (nt.ExtensionAttributeTypeName.Length == 0 && nodeAtt.ExtensionAttributeTypeName.Length > 0)
					nt.ExtensionAttributeTypeName = nodeAtt.ExtensionAttributeTypeName;
			} else {
				// Use the node type name as default name
				if (nt.Id.Length == 0)
					nt.Id = reflector.GetTypeName (ntype);
			}
			
			// Add information about attributes
			object[] fieldAtts = reflector.GetCustomAttributes (ntype, typeof(NodeAttributeAttribute), true);
			foreach (NodeAttributeAttribute fatt in fieldAtts) {
				NodeTypeAttribute natt = new NodeTypeAttribute ();
				natt.Name = fatt.Name;
				natt.Required = fatt.Required;
				if (fatt.TypeName != null)
					natt.Type = fatt.TypeName;
				if (fatt.Description.Length > 0)
					natt.Description = fatt.Description;
				nt.Attributes.Add (natt);
			}
			
			// Check if the type has NodeAttribute attributes applied to fields.
			foreach (object field in reflector.GetFields (ntype)) {
				NodeAttributeAttribute fatt = (NodeAttributeAttribute) reflector.GetCustomAttribute (field, typeof(NodeAttributeAttribute), false);
				if (fatt != null) {
					NodeTypeAttribute natt = new NodeTypeAttribute ();
					if (fatt.Name.Length > 0)
						natt.Name = fatt.Name;
					else
						natt.Name = reflector.GetFieldName (field);
					if (fatt.Description.Length > 0)
						natt.Description = fatt.Description;
					natt.Type = reflector.GetFieldTypeFullName (field);
					natt.Required = fatt.Required;
					nt.Attributes.Add (natt);
				}
			}
			
			// Check if the extension type allows children by looking for [ExtensionNodeChild] attributes.
			// First of all, look in the internalNodeSets hashtable, which is being used as cache
			
			string childSet = (string) internalNodeSets [nt.TypeName];
			
			if (childSet == null) {
				object[] ats = reflector.GetCustomAttributes (ntype, typeof(ExtensionNodeChildAttribute), true);
				if (ats.Length > 0) {
					// Create a new node set for this type. It is necessary to create a new node set
					// instead of just adding child ExtensionNodeType objects to the this node type
					// because child types references can be recursive.
					ExtensionNodeSet internalSet = new ExtensionNodeSet ();
					internalSet.Id = reflector.GetTypeName (ntype) + "_" + Guid.NewGuid().ToString ();
					foreach (ExtensionNodeChildAttribute at in ats) {
						ExtensionNodeType internalType = new ExtensionNodeType ();
						internalType.Id = at.NodeName;
						internalType.TypeName = at.ExtensionNodeTypeName;
						internalSet.NodeTypes.Add (internalType);
					}
					config.ExtensionNodeSets.Add (internalSet);
					nt.NodeSets.Add (internalSet.Id);
					
					// Register the new set in a hashtable, to allow recursive references to the
					// same internal set.
					internalNodeSets [nt.TypeName] = internalSet.Id;
					internalNodeSets [reflector.GetTypeAssemblyQualifiedName (ntype)] = internalSet.Id;
					ScanNodeSet (reflector, config, internalSet, assemblies, internalNodeSets);
				}
			}
			else {
				if (childSet.Length == 0) {
					// The extension type does not declare children.
					return;
				}
				// The extension type can have children. The allowed children are
				// defined in this extension set.
				nt.NodeSets.Add (childSet);
				return;
			}
			
			ScanNodeSet (reflector, config, nt, assemblies, internalNodeSets);
		}
		
		string GetBaseTypeNameList (IAssemblyReflector reflector, object type)
		{
			StringBuilder sb = new StringBuilder ("$");
			foreach (string tn in reflector.GetBaseTypeFullNameList (type))
				sb.Append (tn).Append (',');
			if (sb.Length > 0)
				sb.Remove (sb.Length - 1, 1);
			return sb.ToString ();
		}
		
		object FindAddinType (IAssemblyReflector reflector, string typeName, ArrayList assemblies)
		{
			// Look in the current assembly
			object etype = reflector.GetType (coreAssemblies [reflector], typeName);
			if (etype != null)
				return etype;
			
			// Look in referenced assemblies
			foreach (object asm in assemblies) {
				etype = reflector.GetType (asm, typeName);
				if (etype != null)
					return etype;
			}
			
			Hashtable visited = new Hashtable ();
			
			// Look in indirectly referenced assemblies
			foreach (object asm in assemblies) {
				foreach (object aref in reflector.GetAssemblyReferences (asm)) {
					if (visited.Contains (aref))
						continue;
					visited.Add (aref, aref);
					object rasm = reflector.LoadAssemblyFromReference (aref);
					if (rasm != null) {
						etype = reflector.GetType (rasm, typeName);
						if (etype != null)
							return etype;
					}
				}
			}
			return null;
		}

		void RegisterTypeNode (AddinDescription config, ExtensionAttribute eatt, string path, string nodeName, string typeFullName)
		{
			ExtensionNodeDescription elem = config.MainModule.AddExtensionNode (path, nodeName);
			if (eatt.Id.Length > 0) {
				elem.SetAttribute ("id", eatt.Id);
				elem.SetAttribute ("type", typeFullName);
			} else {
				elem.SetAttribute ("id", typeFullName);
			}
			if (eatt.InsertAfter.Length > 0)
				elem.SetAttribute ("insertafter", eatt.InsertAfter);
			if (eatt.InsertBefore.Length > 0)
				elem.SetAttribute ("insertbefore", eatt.InsertBefore);
		}
		
		internal string GetDefaultTypeExtensionPath (AddinDescription desc, string typeFullName)
		{
			return "/" + Addin.GetIdName (desc.AddinId) + "/TypeExtensions/" + typeFullName;
		}
	}
}
