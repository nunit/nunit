//
// Util.cs
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
using System.Reflection;
using Mono.Addins.Description;
using Mono.Addins.Serialization;
using System.Collections.Generic;

namespace Mono.Addins.Database
{
	internal class Util
	{
		static int isMono;
		static string monoVersion;
		
		public static bool IsWindows {
			get { return Path.DirectorySeparatorChar == '\\'; }
		}
		
		public static bool IsMono {
			get {
				if (isMono == 0)
					isMono = Type.GetType ("Mono.Runtime") != null ? 1 : -1;
				return isMono == 1;
			}
		}
		
		public static string MonoVersion {
			get {
				if (monoVersion == null) {
					if (!IsMono)
						throw new InvalidOperationException ();
					MethodInfo mi = Type.GetType ("Mono.Runtime").GetMethod ("GetDisplayName", BindingFlags.NonPublic|BindingFlags.Static);
					if (mi != null)
						monoVersion = (string) mi.Invoke (null, null);
					else
						monoVersion = string.Empty;
				}
				return monoVersion;
			}
		}
			
		public static void CheckWrittableFloder (string path)
		{
			string testFile = null;
			int n = 0;
			var random = new Random ();
			do {
				testFile = Path.Combine (path, random.Next ().ToString ());
				n++;
			} while (File.Exists (testFile) && n < 100);
			if (n == 100)
				throw new InvalidOperationException ("Could not create file in directory: " + path);
			
			StreamWriter w = new StreamWriter (testFile);
			w.Close ();
			File.Delete (testFile);
		}
		
		public static void AddDependencies (AddinDescription desc, AddinScanResult scanResult)
		{
			// Not implemented in AddinScanResult to avoid making AddinDescription remotable
			foreach (ModuleDescription mod in desc.AllModules) {
				foreach (Dependency dep in mod.Dependencies) {
					AddinDependency adep = dep as AddinDependency;
					if (adep == null) continue;
					string depid = Addin.GetFullId (desc.Namespace, adep.AddinId, adep.Version);
					scanResult.AddAddinToUpdateRelations (depid);
				}
			}
		}
		
		public static Assembly LoadAssemblyForReflection (string fileName)
		{
/*			if (!gotLoadMethod) {
				reflectionOnlyLoadFrom = typeof(Assembly).GetMethod ("ReflectionOnlyLoadFrom");
				gotLoadMethod = true;
				LoadAssemblyForReflection (typeof(Util).Assembly.Location);
			}
			
			if (reflectionOnlyLoadFrom != null)
				return (Assembly) reflectionOnlyLoadFrom.Invoke (null, new string [] { fileName });
			else
*/				return Assembly.LoadFile (fileName);
		}
		
		public static string NormalizePath (string path)
		{
			if (path.Length > 2 && path [0] == '[') {
				int i = path.IndexOf (']', 1);
				if (i != -1) {
					try {
						string fname = path.Substring (1, i - 1);
						Environment.SpecialFolder sf = (Environment.SpecialFolder) Enum.Parse (typeof(Environment.SpecialFolder), fname, true);
						path = Environment.GetFolderPath (sf) + path.Substring (i + 1);
					} catch {
						// Ignore
					}
				}
			}
			if (IsWindows)
				return path.Replace ('/','\\');
			else
				return path.Replace ('\\','/');
		}
		
		// A private hash calculation method is used to be able to get consistent
		// results across different .NET versions and implementations.
		public static int GetStringHashCode (string s)
		{
			int h = 0;
			int n = 0;
			for (; n < s.Length - 1; n+=2) {
				h = unchecked ((h << 5) - h + s[n]);
				h = unchecked ((h << 5) - h + s[n+1]);
			}
			if (n < s.Length)
				h = unchecked ((h << 5) - h + s[n]);
			return h;
		}
		
		public static string GetGacPath (string fullName)
		{
			string[] parts = fullName.Split (',');
			if (parts.Length != 4) return null;
			string name = parts[0].Trim ();
			
			int i = parts[1].IndexOf ('=');
			string version = i != -1 ? parts[1].Substring (i+1).Trim () : parts[1].Trim ();
			
			i = parts[2].IndexOf ('=');
			string culture = i != -1 ? parts[2].Substring (i+1).Trim () : parts[2].Trim ();
			if (culture == "neutral") culture = "";
			
			i = parts[3].IndexOf ('=');
			string token = i != -1 ? parts[3].Substring (i+1).Trim () : parts[3].Trim ();

			string versionDirName = version + "_" + culture + "_" + token;
			
			if (Util.IsMono) {
				string gacDir = typeof(Uri).Assembly.Location;
				gacDir = Path.GetDirectoryName (gacDir);
				gacDir = Path.GetDirectoryName (gacDir);
				gacDir = Path.GetDirectoryName (gacDir);
				string dir = Path.Combine (gacDir, name);
				return Path.Combine (dir, versionDirName);
			} else {
				// .NET 4.0 introduces a new GAC directory structure and location.
				// Assembly version directory names are now prefixed with the CLR version
				// Since there can be different assembly versions for different target CLR runtimes,
				// we now look for the best match, that is, the assembly with the higher CLR version
				
				var currentVersion = new Version (Environment.Version.Major, Environment.Version.Minor);
				
				foreach (var gacDir in GetDotNetGacDirectories ()) {
					var asmDir = Path.Combine (gacDir, name);
					if (!Directory.Exists (asmDir))
						continue;
					Version bestVersion = new Version (0, 0);
					string bestDir = null;
					foreach (var dir in Directory.GetDirectories (asmDir, "v*_" + versionDirName)) {
						var dirName = Path.GetFileName (dir);
						i = dirName.IndexOf ('_');
						Version av;
						if (Version.TryParse (dirName.Substring (1, i - 1), out av)) {
							if (av == currentVersion)
								return dir;
							else if (av < currentVersion && av > bestVersion) {
								bestDir = dir;
								bestVersion = av;
							}
						}
					}
					if (bestDir != null)
						return bestDir;
				}
				
				// Look in the old GAC. There are no CLR prefixes here
				
				foreach (var gacDir in GetLegacyDotNetGacDirectories ()) {
					var asmDir = Path.Combine (gacDir, name);
					asmDir = Path.Combine (asmDir, versionDirName);
					if (Directory.Exists (asmDir))
						return asmDir;
				}
				return null;
			}
		}

		static IEnumerable<string> GetLegacyDotNetGacDirectories ()
		{
			var winDir = Path.GetFullPath (Environment.SystemDirectory + "\\..");

			string gacDir = winDir + "\\assembly\\GAC";
			if (Directory.Exists (gacDir))
				yield return gacDir;
			if (Directory.Exists (gacDir + "_32"))
				yield return gacDir + "_32";
			if (Directory.Exists (gacDir + "_64"))
				yield return gacDir + "_64";
			if (Directory.Exists (gacDir + "_MSIL"))
				yield return gacDir + "_MSIL";
		}

		static IEnumerable<string> GetDotNetGacDirectories ()
		{
			var winDir = Path.GetFullPath (Environment.SystemDirectory + "\\..");
			
			string gacDir = winDir + "\\Microsoft.NET\\assembly\\GAC";
			if (Directory.Exists (gacDir))
				yield return gacDir;
			if (Directory.Exists (gacDir + "_32"))
				yield return gacDir + "_32";
			if (Directory.Exists (gacDir + "_64"))
				yield return gacDir + "_64";
			if (Directory.Exists (gacDir + "_MSIL"))
				yield return gacDir + "_MSIL";
		}

		internal static bool IsManagedAssembly (string file)
		{
			try {
				AssemblyName.GetAssemblyName (file);
				return true;
			} catch (BadImageFormatException) {
				return false;
			}
		}
	}
}
