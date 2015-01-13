// 
// AddinFileSystemExtension.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Reflection;

namespace Mono.Addins.Database
{
	/// <summary>
	/// An add-in file system extension.
	/// </summary>
	/// <remarks>
	/// File system extensions can override the behavior of the add-in scanner and provide custom rules for
	/// locating and scanning assemblies.
	/// </remarks>
	public class AddinFileSystemExtension
	{
		IAssemblyReflector reflector;
		
		/// <summary>
		/// Called when the add-in scan is about to start
		/// </summary>
		public virtual void ScanStarted ()
		{
		}
		
		/// <summary>
		/// Called when the add-in scan has finished
		/// </summary>
		public virtual void ScanFinished ()
		{
		}
		
		/// <summary>
		/// Checks if a directory exists
		/// </summary>
		/// <returns>
		/// 'true' if the directory exists
		/// </returns>
		/// <param name='path'>
		/// Directory path
		/// </param>
		public virtual bool DirectoryExists (string path)
		{
			return Directory.Exists (path);
		}
		
		/// <summary>
		/// Checks if a file exists
		/// </summary>
		/// <returns>
		/// 'true' if the file exists
		/// </returns>
		/// <param name='path'>
		/// Path to the file
		/// </param>
		public virtual bool FileExists (string path)
		{
			return File.Exists (path);
		}
		
		/// <summary>
		/// Gets the files in a directory
		/// </summary>
		/// <returns>
		/// The full path of the files in the directory
		/// </returns>
		/// <param name='path'>
		/// Directory path
		/// </param>
		public virtual System.Collections.Generic.IEnumerable<string> GetFiles (string path)
		{
			return Directory.GetFiles (path);
		}
		
		/// <summary>
		/// Gets the subdirectories of a directory
		/// </summary>
		/// <returns>
		/// The subdirectories.
		/// </returns>
		/// <param name='path'>
		/// The directory
		/// </param>
		public virtual System.Collections.Generic.IEnumerable<string> GetDirectories (string path)
		{
			return Directory.GetDirectories (path);
		}

		/// <summary>
		/// Gets the last write time of a file
		/// </summary>
		/// <returns>
		/// The last write time.
		/// </returns>
		/// <param name='filePath'>
		/// File path.
		/// </param>
		public virtual DateTime GetLastWriteTime (string filePath)
		{
			return File.GetLastWriteTime (filePath);
		}
		
		/// <summary>
		/// Opens a text file
		/// </summary>
		/// <returns>
		/// The text file stream
		/// </returns>
		/// <param name='path'>
		/// File path.
		/// </param>
		public virtual System.IO.StreamReader OpenTextFile (string path)
		{
			return new StreamReader (path);
		}
		
		/// <summary>
		/// Opens a file.
		/// </summary>
		/// <returns>
		/// The file stream.
		/// </returns>
		/// <param name='path'>
		/// The file path.
		/// </param>
		public virtual System.IO.Stream OpenFile (string path)
		{
			return File.OpenRead (path);
		}
		
		/// <summary>
		/// Gets an assembly reflector for a file.
		/// </summary>
		/// <returns>
		/// The reflector for the file.
		/// </returns>
		/// <param name='locator'>
		/// An assembly locator
		/// </param>
		/// <param name='path'>
		/// A file path
		/// </param>
		public virtual IAssemblyReflector GetReflectorForFile (IAssemblyLocator locator, string path)
		{
			if (reflector != null)
				return reflector;
			
			// If there is a local copy of the cecil reflector, use it instead of the one in the gac
			Type t;
			string asmFile = Path.Combine (Path.GetDirectoryName (GetType().Assembly.Location), "Mono.Addins.CecilReflector.dll");
			if (File.Exists (asmFile)) {
				Assembly asm = Assembly.LoadFrom (asmFile);
				t = asm.GetType ("Mono.Addins.CecilReflector.Reflector");
			}
			else {
				string refName = GetType().Assembly.FullName;
				int i = refName.IndexOf (',');
				refName = "Mono.Addins.CecilReflector.Reflector, Mono.Addins.CecilReflector" + refName.Substring (i);
				t = Type.GetType (refName, false);
			}
			if (t != null)
				reflector = (IAssemblyReflector) Activator.CreateInstance (t);
			else
				reflector = new DefaultAssemblyReflector ();
			
			reflector.Initialize (locator);
			return reflector;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="Mono.Addins.Database.AddinFileSystemExtension"/> needs to be isolated from the main execution process
		/// </summary>
		/// <value>
		/// <c>true</c> if requires isolation; otherwise, <c>false</c>.
		/// </value>
		public virtual bool RequiresIsolation {
			get { return true; }
		}
	}
}

