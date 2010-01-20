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
using System.Collections;
using System.Xml;
using System.IO;

namespace NUnit.AdhocTestRunner
{
    public class ConsoleRunner
    {
        private CommandLineOptions commandlineOptions;

        public ConsoleRunner(CommandLineOptions commandlineOptions)
        {
            this.commandlineOptions = commandlineOptions;
        }

        public void Execute()
        {
            WriteRunInfo();

            try
            {
                IDictionary loadOptions = new Hashtable();
                if (commandlineOptions.Load.Count > 0)
                    loadOptions["LOAD"] = commandlineOptions.Load;

                AppDomain testDomain = AppDomain.CurrentDomain;
                if (commandlineOptions.UseAppDomain)
                    testDomain = CreateDomain(
                        Path.GetDirectoryName(Path.GetFullPath(commandlineOptions.Parameters[0])));

                FrameworkController driver = new FrameworkController(testDomain);

                string assemblyFilename = commandlineOptions.Parameters[0];

                if (!driver.Load(assemblyFilename, loadOptions))
                {
                    Console.WriteLine( 
                        commandlineOptions.Load.Count > 0
                            ? "Specifed tests not found in assembly {0}"
                            : "No tests found in assembly {0}", 
                        assemblyFilename);
                }
                else
                {
                    TextWriter savedOut = Console.Out;
                    TextWriter savedError = Console.Error;

                    //TestEventListener listener = new TestEventListener(options, Console.Out);

                    XmlNode result = driver.Run();

                    Console.SetOut(savedOut);
                    Console.SetError(savedError);

                    XmlNode xmlResult = result as XmlNode;
                    XmlTextWriter writer = new XmlTextWriter("TestResult.Xml", System.Text.Encoding.UTF8);
                    xmlResult.WriteTo(writer);
                    writer.Close();

                    new ResultReporter(result).ReportResults();
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;

                if (ex is FileNotFoundException)
                    Console.WriteLine(ex.Message);
                else
                    Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (commandlineOptions.Wait)
                {
                    Console.WriteLine("Press Enter key to continue . . .");
                    Console.ReadLine();
                }
            }
        }

        private static AppDomain CreateDomain(string appBase)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = appBase;
            AppDomain domain = AppDomain.CreateDomain("test-domain", null, setup);
            return domain;
        }

        private void WriteRunInfo()
        {
            string clrPlatform = Type.GetType("Mono.Runtime", false) == null ? ".NET" : "Mono";
            Console.WriteLine("Runtime Environment -");
            Console.WriteLine("    OS Version: {0}", Environment.OSVersion);
            Console.WriteLine("  {0} Version: {1}", clrPlatform, Environment.Version);
            Console.WriteLine();

            Console.WriteLine("Options -");
            Console.WriteLine( commandlineOptions.UseAppDomain
                ? "    Use Separate AppDomain"
                : "    Use Same AppDomain" );
            Console.WriteLine();
        }
    }
}
