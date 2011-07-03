// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NDesk.Options;

namespace NUnit.ConsoleRunner
{
	/// <summary>
	/// This class provides the entry point for the console runner.
	/// </summary>
	public class Program
	{
        //static Logger log = InternalTrace.GetLogger(typeof(Runner));

		[STAThread]
		public static int Main(string[] args)
		{
            ConsoleOptions options = new ConsoleOptions();

            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                WriteHeader();
                Console.WriteLine(ex.Message, ex.OptionName);
                return ConsoleRunner.INVALID_ARG;
            }

            // Create SettingsService early so we know the trace level right at the start
            //SettingsService settingsService = new SettingsService();
            //InternalTraceLevel level = (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default);
            //if (options.trace != InternalTraceLevel.Default)
            //    level = options.trace;

            //InternalTrace.Initialize("nunit-console_%p.log", level);
            
            //log.Info("NUnit-console.exe starting");

			if(!options.noheader)
				WriteHeader();

            if (options.help)
            {
                WriteHelpText(options);
                return ConsoleRunner.OK;
            }
            
            if (!options.Validate())
            {
                foreach (string message in options.ErrorMessages)
                    Console.Error.WriteLine(message);

                return ConsoleRunner.INVALID_ARG;
            }
            
            if (options.InputFiles.Length == 0)
            {
                Console.Error.WriteLine("Error: no inputs specified");
                return ConsoleRunner.OK;
            }

            // TODO: Move this to engine
            foreach (string file in options.InputFiles)
            {
                //if (!Services.ProjectService.CanLoadProject(file) && !PathUtils.IsAssemblyFileType(file))
                string ext = Path.GetExtension(file);
                if (ext != ".dll" && ext != ".exe" && ext != ".nunit")
                {
                    Console.WriteLine("File type not known: {0}", file);
                    return ConsoleRunner.INVALID_ARG;
                }
            }

            try
            {
                return new ConsoleRunner(options).Execute();
            }
            catch( FileNotFoundException ex )
            {
                Console.WriteLine( ex.Message );
                return ConsoleRunner.FILE_NOT_FOUND;
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Unhandled Exception:\n{0}", ex.ToString() );
                return ConsoleRunner.UNEXPECTED_ERROR;
            }
            finally
            {
                if(options.wait)
                {
                    Console.Out.WriteLine("\nHit <enter> key to continue");
                    Console.ReadLine();
                }

            //    log.Info( "NUnit-console.exe terminating" );
            }
		}

		private static void WriteHeader()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string versionText = executingAssembly.GetName().Version.ToString();

            string productName = "NUnit-Console";
            string copyrightText = "Copyright (C) 2002-2011 Charlie Poole.\r\nCopyright (C) 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov.\r\nCopyright (C) 2000-2002 Philip Craig.\r\nAll Rights Reserved.";

            //object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            //if ( objectAttrs.Length > 0 )
            //    productName = ((AssemblyProductAttribute)objectAttrs[0]).Product;

			object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			if ( objectAttrs.Length > 0 )
				copyrightText = ((AssemblyCopyrightAttribute)objectAttrs[0]).Copyright;

			objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (objectAttrs.Length > 0)
            {
                string configText = ((AssemblyConfigurationAttribute)objectAttrs[0]).Configuration;
                if (configText != "")
                    versionText += string.Format(" ({0})", configText);
            }

			Console.WriteLine(String.Format("{0} version {1}", productName, versionText));
			Console.WriteLine(copyrightText);
			Console.WriteLine();

            Console.WriteLine("Runtime Environment - ");
            //RuntimeFramework framework = RuntimeFramework.CurrentFramework;
            Console.WriteLine(string.Format("   OS Version: {0}", Environment.OSVersion));
            //Console.WriteLine(string.Format("  CLR Version: {0} ( {1} )",
            //    Environment.Version, framework.DisplayName));
            Console.WriteLine(string.Format("  CLR Version: {0}",
                Environment.Version));

            Console.WriteLine();
        }

        private static void WriteHelpText(ConsoleOptions options)
        {
            Console.WriteLine();
            Console.WriteLine("NUNIT-CONSOLE [inputfiles] [options]");
            Console.WriteLine();
            Console.WriteLine("Runs a set of NUnit tests from the console.");
            Console.WriteLine();
            //Console.WriteLine("You may specify one or more assemblies or a single");
            //Console.WriteLine("project file of type .nunit.");
            //Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            //Console.WriteLine("Options that take values may use an equal sign, a colon");
            //Console.WriteLine("or a space to separate the option from its value.");
            //Console.WriteLine();
        }
	}
}
