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
using System.Reflection;
using System.IO;

namespace NUnit.DirectRunner
{
    /// <summary>
    /// This class is an adoc runner used in testing the framework
    /// by running tests directly, without use of the normal infrastructure
    /// used by standard NUnit runners. It is only available in the source 
    /// code for use by developers and is not distributed as part of NUnit.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            options.Parse(args);

            if (!options.Nologo)
                WriteCopyright();

            if (options.Help)
                Console.Write(options.HelpText);
            else if (options.Error)
            {
                Console.WriteLine(options.ErrorMessage);
                Console.WriteLine("Try test-runner -help for more info");
                Console.WriteLine();
            }
            else if (options.Parameters.Length == 0)
                Console.WriteLine("No Test assembly was specified");
            else
                new ConsoleRunner(options).Execute();
        }

        private static void WriteCopyright()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.Version version = executingAssembly.GetName().Version;
            string copyright = "Copyright (C) 2011, Charlie Poole";
            string title = "NUnit Framework Test Harness";
            string build = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyTitleAttribute titleAttr = (AssemblyTitleAttribute)attrs[0];
                title = titleAttr.Title;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyCopyrightAttribute copyrightAttr = (AssemblyCopyrightAttribute)attrs[0];
                copyright = copyrightAttr.Copyright;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyConfigurationAttribute configAttr = (AssemblyConfigurationAttribute)attrs[0];
                build = string.Format("({0})", configAttr.Configuration);
            }

            Console.WriteLine(String.Format("{0} {1} {2}", title, version.ToString(3), build));
            Console.WriteLine(copyright);
            Console.WriteLine();
        }
    }
}
