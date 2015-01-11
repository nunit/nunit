//
// AddinScanResult.cs
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
using System.Reflection;

namespace Mono.Addins.Database
{
	internal class AddinScanResult: MarshalByRefObject, IAssemblyLocator
	{
		internal ArrayList AddinsToScan = new ArrayList ();
		internal List<string> AddinsToUpdateRelations = new List<string> ();
		internal List<string> AddinsToUpdate = new List<string> ();
		internal ArrayList FilesToScan = new ArrayList ();
		internal ArrayList ModifiedFolderInfos = new ArrayList ();
		internal ArrayList FilesWithScanFailure = new ArrayList ();
		internal AddinHostIndex HostIndex;
		internal List<string> RemovedAddins = new List<string> ();
		Hashtable visitedFolders = new Hashtable ();
		
		Hashtable assemblyLocations = new Hashtable ();
		Hashtable assemblyLocationsByFullName = new Hashtable (); 
		Hashtable filesToIgnore;
		
		bool regenerateRelationData;
		bool changesFound;
		
		public bool RegenerateAllData;
		public bool CheckOnly;
		public bool LocateAssembliesOnly;
		public string Domain;
		
		public bool ChangesFound {
			get { return changesFound; }
			set { changesFound = value; }
		}

		public bool RegenerateRelationData {
			get { return regenerateRelationData; }
			set {
				regenerateRelationData = value;
				if (value)
					ChangesFound = true;
			}
		}
		
		public bool VisitFolder (string folder)
		{
			if (visitedFolders.Contains (folder) || IgnorePath (folder))
				return false;
			else {
				visitedFolders.Add (folder, folder);
				return true;
			}
		}
		
		public bool IgnorePath (string file)
		{
			if (filesToIgnore == null)
				return false;
			string root = Path.GetPathRoot (file);
			while (root != file) {
				if (filesToIgnore.Contains (file))
					return true;
				file = Path.GetDirectoryName (file);
			}
			return false;
		}
		
		public void AddPathToIgnore (string path)
		{
			if (filesToIgnore == null)
				filesToIgnore = new Hashtable ();
			filesToIgnore [path] = path;
		}
		
		public void AddPathsToIgnore (IEnumerable paths)
		{
			foreach (string p in paths)
				AddPathToIgnore (p);
		}
		
		public void AddAddinToScan (string addinId)
		{
			if (!AddinsToScan.Contains (addinId))
				AddinsToScan.Add (addinId);
		}
		
		public void AddRemovedAddin (string addinId)
		{
			if (!RemovedAddins.Contains (addinId))
				RemovedAddins.Add (addinId);
		}
		
		public void AddFileToWithFailure (string file)
		{
			if (!FilesWithScanFailure.Contains (file))
				FilesWithScanFailure.Add (file);
		}
		
		public void AddFileToScan (string file, AddinScanFolderInfo folderInfo)
		{
			FileToScan di = new FileToScan ();
			di.File = file;
			di.AddinScanFolderInfo = folderInfo;
			FilesToScan.Add (di);
			RegisterModifiedFolderInfo (folderInfo);
		}
		
		public void RegisterModifiedFolderInfo (AddinScanFolderInfo folderInfo)
		{
			if (!ModifiedFolderInfos.Contains (folderInfo))
				ModifiedFolderInfos.Add (folderInfo);
		}
		
		public void AddAddinToUpdateRelations (string addinId)
		{
			if (!AddinsToUpdateRelations.Contains (addinId))
				AddinsToUpdateRelations.Add (addinId);
		}
		
		public void AddAddinToUpdate (string addinId)
		{
			if (!AddinsToUpdate.Contains (addinId))
				AddinsToUpdate.Add (addinId);
		}
		
		public void AddAssemblyLocation (string file)
		{
			string name = Path.GetFileNameWithoutExtension (file);
			ArrayList list = assemblyLocations [name] as ArrayList;
			if (list == null) {
				list = new ArrayList ();
				assemblyLocations [name] = list;
			}
			list.Add (file);
		}
		
		public string GetAssemblyLocation (string fullName)
		{
			string loc = assemblyLocationsByFullName [fullName] as String;
			if (loc != null)
				return loc;

			int i = fullName.IndexOf (',');
			string name = fullName.Substring (0,i);
			if (name == "Mono.Addins")
				return GetType ().Assembly.Location;
			ArrayList list = assemblyLocations [name] as ArrayList;
			if (list == null)
				return null;
			
			string lastAsm = null;
			foreach (string file in list.ToArray ()) {
				try {
					list.Remove (file);
					AssemblyName aname = AssemblyName.GetAssemblyName (file);
					lastAsm = file;
					assemblyLocationsByFullName [aname.FullName] = file;
					if (aname.FullName == fullName)
						return file;
				} catch {
					// Could not get the assembly name. The file either doesn't exist or it is not a valid assembly.
					// In this case, just ignore it.
				}
			}
			
			if (lastAsm != null) {
				// If an exact version is not found, just take any of them
				return lastAsm;
			}
			return null;
		}
	}
		
	class FileToScan
	{
		public string File;
		public AddinScanFolderInfo AddinScanFolderInfo;
	}
}
