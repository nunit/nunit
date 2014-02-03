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
using Mono.Options;
using NUnit.Engine;

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

		    ColorSetter.Options = options;

            // Create SettingsService early so we know the trace level right at the start
            //SettingsService settingsService = new SettingsService();
            //InternalTraceLevel level = (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default);
            //if (options.trace != InternalTraceLevel.Default)
            //    level = options.trace;

            //InternalTrace.Initialize("nunit-console_%p.log", level);
            
            //log.Info("NUnit-console.exe starting");
		    try
            {
                if (options.PauseBeforeRun)
                {
                    ColorSetter.WriteLine(ColorStyle.Warning, "Press any key to continue . . .");
                    Console.ReadKey(true);
                }

                if (!options.NoHeader)
                    WriteHeader();

                if (options.ShowHelp)
                {
                    WriteHelpText(options);
                    return ConsoleRunner.OK;
                }

                if (!options.Validate())
                {
                    using (new ColorSetter(ColorStyle.Error))
                    {
                        foreach (string message in options.ErrorMessages)
                            Console.Error.WriteLine(message);
                    }

                    return ConsoleRunner.INVALID_ARG;
                }

                if (options.InputFiles.Count == 0)
                {
                    using (new ColorSetter(ColorStyle.Error))
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
                        ColorSetter.WriteLine(ColorStyle.Warning, "File type not known: " + file);
                        return ConsoleRunner.INVALID_ARG;
                    }
                }

                string workDirectory = options.WorkDirectory ?? Environment.CurrentDirectory;
                var traceLevel = options.InternalTraceLevel != null
                    ? (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), options.InternalTraceLevel)
                    : InternalTraceLevel.Off;

                using (ITestEngine engine = TestEngineActivator.CreateInstance(workDirectory, traceLevel))
                {
                    try
                    {
                        return new ConsoleRunner(engine, options).Execute();
                    }
                    catch (NUnitEngineException ex)
                    {
                        ColorSetter.WriteLine(ColorStyle.Error, ex.Message);
                        return ConsoleRunner.INVALID_ARG;
                    }
                    catch (FileNotFoundException ex)
                    {
                        ColorSetter.WriteLine(ColorStyle.Error, ex.Message);
#if DEBUG
                        ColorSetter.WriteLine(ColorStyle.Error, ex.StackTrace);
#endif
                        return ConsoleRunner.FILE_NOT_FOUND;
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        ColorSetter.WriteLine(ColorStyle.Error, ex.Message);
                        return ConsoleRunner.FILE_NOT_FOUND;
                    }
                    catch (Exception ex)
                    {
                        ColorSetter.WriteLine(ColorStyle.Error, ex.Message);
                        return ConsoleRunner.UNEXPECTED_ERROR;
                    }
                    finally
                    {
                        if (options.WaitBeforeExit)
                        {
                            using (new ColorSetter(ColorStyle.Warning))
                            {
                                Console.Out.WriteLine("\nPress any key to continue . . .");
                                Console.ReadKey(true);
                            }
                        }

                        //    log.Info( "NUnit-console.exe terminating" );
                    }
                }
            }
            finally
            {
                Console.ResetColor();
		    }
		}

		private static void WriteHeader()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string versionText = executingAssembly.GetName().Version.ToString(3);

            string programName = "NUnit Console Runner";
            string copyrightText = "Copyright (C) 2011 Charlie Poole.\r\nAll Rights Reserved.";
            string configText = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attrs.Length > 0)
                programName = ((AssemblyTitleAttribute)attrs[0]).Title;

			attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			if ( attrs.Length > 0 )
				copyrightText = ((AssemblyCopyrightAttribute)attrs[0]).Copyright;

			attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
                configText = string.Format("({0})", ((AssemblyConfigurationAttribute)attrs[0]).Configuration);

            ColorSetter.WriteLine(ColorStyle.Header, string.Format( "{0} {1} {2}", programName, versionText, configText ));
            ColorSetter.WriteLine(ColorStyle.SubHeader, copyrightText);
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.SectionHeader, "Runtime Environment - ");
            ColorSetter.WriteLabel("   OS Version: ", Environment.OSVersion.ToString(), true);
            ColorSetter.WriteLabel("  CLR Version: ", Environment.Version.ToString(), true);
            Console.WriteLine();
        }

        private static void WriteHelpText(ConsoleOptions options)
        {
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.Header, "NUNIT-CONSOLE [inputfiles] [options]");
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.Default, "Runs a set of NUnit tests from the console.");
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.SectionHeader, "InputFiles:");
            ColorSetter.WriteLine(ColorStyle.Default, "      One or more assemblies or test projects of a recognized type.");
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.SectionHeader, "Options:");
            using (new ColorSetter(ColorStyle.Default))
            {
                options.WriteOptionDescriptions(Console.Out);
            }
            Console.WriteLine();
            ColorSetter.WriteLine(ColorStyle.SectionHeader, "Description:");
            using (new ColorSetter(ColorStyle.Default))
            {
                Console.WriteLine("      By default, this command runs the tests contained in the");
                Console.WriteLine("      assemblies and projects specified. If the --explore option");
                Console.WriteLine("      is used, no tests are executed but a description of the tests");
                Console.WriteLine("      is saved in the specified or default format.");
                Console.WriteLine();
                Console.WriteLine("      Several options that specify processing of XML output take");
                Console.WriteLine("      an output specification as a value. A SPEC may take one of");
                Console.WriteLine("      the following forms:");
                Console.WriteLine("          --OPTION:filename");
                Console.WriteLine("          --OPTION:filename;format=formatname");
                Console.WriteLine("          --OPTION:filename;transform=xsltfile");
                Console.WriteLine();
                Console.WriteLine("      The --result option may use any of the following formats:");
                Console.WriteLine("          nunit3 - the native XML format for NUnit 3.0");
                Console.WriteLine("          nunit2 - legacy XML format used by earlier releases of NUnit");
                Console.WriteLine();
                Console.WriteLine("      The --explore option may use any of the following formats:");
                Console.WriteLine("          nunit3 - the native XML format for NUnit 3.0");
                Console.WriteLine("          cases  - a text file listing the full names of all test cases.");
                Console.WriteLine("      If --explore is used without any specification following, a list of");
                Console.WriteLine("      test cases is output to the console.");
                Console.WriteLine();
                Console.WriteLine("      If none of the options {--result, --explore, --noxml} is used,");
                Console.WriteLine("      NUnit saves the results to TestResult.xml in nunit3 format");
                Console.WriteLine();
                Console.WriteLine("      Any transforms provided must handle input in the native nunit3 format.");
                Console.WriteLine();
                //Console.WriteLine("Options that take values may use an equal sign, a colon");
                //Console.WriteLine("or a space to separate the option from its value.");
                //Console.WriteLine();
            }
        }
	}
}
