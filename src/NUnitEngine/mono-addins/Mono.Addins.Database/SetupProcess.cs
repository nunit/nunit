//
// SetupProcess.cs
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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Reflection.Emit;

namespace Mono.Addins.Database
{
	class SetupProcess: ISetupHandler
	{
		public void Scan (IProgressStatus monitor, AddinRegistry registry, string scanFolder, string[] filesToIgnore)
		{
			ExecuteCommand (monitor, registry.RegistryPath, registry.StartupDirectory, registry.DefaultAddinsFolder, registry.AddinCachePath, "scan", scanFolder, filesToIgnore);
		}
		
		public void GetAddinDescription (IProgressStatus monitor, AddinRegistry registry, string file, string outFile)
		{
			ExecuteCommand (monitor, registry.RegistryPath, registry.StartupDirectory, registry.DefaultAddinsFolder, registry.AddinCachePath, "get-desc", file, outFile);
		}
		
		internal static void ExecuteCommand (IProgressStatus monitor, string registryPath, string startupDir, string addinsDir, string databaseDir, string name, string arg1, params string[] args)
		{
			string verboseParam = monitor.LogLevel.ToString ();
			
			// Arguments string
			StringBuilder sb = new StringBuilder ();
			sb.Append (verboseParam).Append (' ').Append (name);
			sb.Append (" \"").Append (arg1).Append ("\"");
			foreach (string arg in args)
				sb.Append (" \"").Append (arg).Append ("\"");
			
			Process process = new Process ();
			
			string asm = null;
			try {
				asm = CreateHostExe ();
				if (!Util.IsMono)
					process.StartInfo = new ProcessStartInfo (asm, sb.ToString ());
				else {
					asm = asm.Replace(" ", @"\ ");
					process.StartInfo = new ProcessStartInfo ("mono", "--debug " + asm + " " + sb.ToString ());
				}
				process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.EnableRaisingEvents = true;
				process.Start ();
			
				process.StandardInput.WriteLine (registryPath);
				process.StandardInput.WriteLine (startupDir);
				process.StandardInput.WriteLine (addinsDir);
				process.StandardInput.WriteLine (databaseDir);
				process.StandardInput.Flush ();
	
	//			string rr = process.StandardOutput.ReadToEnd ();
	//			Console.WriteLine (rr);
				
				StringCollection progessLog = new StringCollection ();
				ProcessProgressStatus.MonitorProcessStatus (monitor, process.StandardOutput, progessLog);
				process.WaitForExit ();
				if (process.ExitCode != 0)
					throw new ProcessFailedException (progessLog);
				
			} catch (Exception ex) {
				Console.WriteLine (ex);
				throw;
			} finally {
				if (asm != null) {
					try {
						File.Delete (asm);
					} catch { }
				}
			}
		}
		
		public static int Main (string[] args)
		{
			ProcessProgressStatus monitor = new ProcessProgressStatus (int.Parse (args[0]));
			
			try {
				string registryPath = Console.In.ReadLine ();
				string startupDir = Console.In.ReadLine ();
				string addinsDir = Console.In.ReadLine ();
				string databaseDir = Console.In.ReadLine ();
				
				AddinDatabase.RunningSetupProcess = true;
				AddinRegistry reg = new AddinRegistry (registryPath, startupDir, addinsDir, databaseDir);
			
				switch (args [1]) {
				case "scan":
					string folder = args.Length > 2 ? args [2] : null;
					if (folder.Length == 0) folder = null;
					StringCollection filesToIgnore = new StringCollection ();
					for (int n=3; n<args.Length; n++)
						filesToIgnore.Add (args[n]);
					reg.ScanFolders (monitor, folder, filesToIgnore);
					break;
				case "get-desc":
					reg.ParseAddin (monitor, args[2], args[3]);
					break;
				}
			} catch (Exception ex) {
				monitor.ReportError ("Unexpected error in setup process", ex);
				return 1;
			}
			return 0;
		}
		
		static string CreateHostExe ()
		{
			string file;
			string id;
			string fullFile;
			do {
				id = Guid.NewGuid ().ToString ().Replace ('-','_');
				file = id + ".exe";
				fullFile = Path.Combine (Path.GetTempPath (), file);
			} while (File.Exists (fullFile));

			AssemblyName aname = new AssemblyName ();
			aname.Name = id;
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly (aname, AssemblyBuilderAccess.Save, Path.GetTempPath ());
			ModuleBuilder mb = ab.DefineDynamicModule (aname.Name, file);
			TypeBuilder tb = mb.DefineType ("App", TypeAttributes.Public|TypeAttributes.Class);
			
			MethodBuilder fb = tb.DefineMethod("Main",
			                                   MethodAttributes.Public |
			                                   MethodAttributes.Static,
			                                   typeof(int), new Type[] { typeof(string[]) });
			
			MethodInfo mi = typeof(SetupProcess).GetMethod ("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
			ILGenerator ilg = fb.GetILGenerator();
			ilg.Emit (OpCodes.Ldarg_0);
			ilg.EmitCall (OpCodes.Call, mi, null);
			ilg.Emit (OpCodes.Ret);
			
			tb.CreateType();
			ab.SetEntryPoint (fb, PEFileKinds.WindowApplication);
			ab.Save (file);
			return fullFile;
		}
	}
	
	class ProcessFailedException: Exception
	{
		StringCollection progessLog;
		
		public ProcessFailedException (StringCollection progessLog): this (progessLog, null)
		{
		}
		
		public ProcessFailedException (StringCollection progessLog, Exception ex): base ("Setup process failed.", ex)
		{
			this.progessLog = progessLog;
		}
		
		public StringCollection ProgessLog {
			get { return progessLog; }
		}
		
		public string LastLog {
			get { return progessLog.Count > 0 ? progessLog [progessLog.Count - 1] : ""; }
		}
	}
}
