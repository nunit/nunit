// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Reflection;
using System.IO;
using Mono.Options;
using NUnit.Framework.Internal;

namespace NUnit.Framework.TestHarness
{
    /// <summary>
    /// This class is an adoc runner used in testing the framework
    /// by running tests directly, without use of the normal infrastructure
    /// used by standard NUnit runners. It is only available in the source 
    /// code for use by developers and is not distributed as part of NUnit.
    /// </summary>
    public class Program
    {
        static int Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();

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

            if (!options.NoHeader)
                WriteHeader();

            if (options.ShowHelp)
            {
                //Console.Write(options.HelpText);
                WriteHelpText(options);
                return ConsoleRunner.OK;
            }

            if (!options.Validate())
            {
                foreach (string message in options.ErrorMessages)
                    Console.Error.WriteLine(message);

                return ConsoleRunner.INVALID_ARG;
            }

            try
            {
                return new ConsoleRunner(options).Execute();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return ConsoleRunner.FILE_NOT_FOUND;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ConsoleRunner.UNEXPECTED_ERROR;
            }
            finally
            {
                if (options.WaitBeforeExit)
                {
                    Console.Out.WriteLine("\nPress any key to continue . . .");
                    Console.ReadKey(true);
                }
            }
        }

        private static void WriteHeader()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.Version version = executingAssembly.GetName().Version;

            string programName = "NUnit Framework Test Harness";
            string copyrightText = "Copyright (C) 2011, Charlie Poole";
            string configText = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attrs.Length > 0)
                programName = ((AssemblyTitleAttribute)attrs[0]).Title;

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attrs.Length > 0)
                copyrightText = ((AssemblyCopyrightAttribute)attrs[0]).Copyright;

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
                configText = string.Format("({0})", ((AssemblyConfigurationAttribute)attrs[0]).Configuration);

            Console.WriteLine(String.Format("{0} {1} {2}", programName, version.ToString(3), configText));
            Console.WriteLine(copyrightText);
            Console.WriteLine();

            Console.WriteLine("Runtime Environment - ");
            Console.WriteLine(string.Format("        OS Version: {0}", Environment.OSVersion));
            Console.WriteLine(string.Format("       CLR Version: {0}", Environment.Version));
            Console.WriteLine(string.Format(" Runtime Framework: {0}", RuntimeFramework.CurrentFramework));

            Console.WriteLine();
        }

        private static void WriteHelpText(CommandLineOptions options)
        {
            Console.WriteLine();
            Console.WriteLine("TEST-HARNESS [assemblyname] [options]");
            Console.WriteLine();
            Console.WriteLine("Runs or displays a set of NUnit framework tests contained in an assembly.");
            Console.WriteLine("The harness is not intended for general test execution, but is a tool used");
            Console.WriteLine("in the development of the framework to avoid any dependency on higher-level");
            Console.WriteLine("test runners like nunit-console or on the NUnit test engine.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Description:");
            Console.WriteLine("      By default, this command runs the tests contained in the");
            Console.WriteLine("      assembly specified in the command line. Only one assembly");
            Console.WriteLine("      may be specified.");
            Console.WriteLine();
            Console.WriteLine("      If the --explore option is used, no tests are executed but");
            Console.WriteLine("      the XML description of the tests is displayed on the console");
            Console.WriteLine("      or saved in the file specified with the option.");
            Console.WriteLine();
            Console.WriteLine("      The --selftest option is equivalent to using test-harness.exe");
            Console.WriteLine("      as the assembly name. No assemblyname should be specified with");
            Console.WriteLine("      this option.");
            Console.WriteLine();
            Console.WriteLine("      When --help is used anything else on the command line is ignored.");
            Console.WriteLine();
        }
    }
}
