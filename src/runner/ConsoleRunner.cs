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
using System.IO;
using NUnit.Core;

namespace NUnit.AdhocTestRunner
{
    public class ConsoleRunner
    {
        private static string frameworkAssembly = "nunit.framework";
        private static string runnerTypeName = "NUnit.Core.RemoteTestRunner";

        AppDomain testDomain;

        private CommandLineOptions options;

        public ConsoleRunner(CommandLineOptions options)
        {
            this.options = options;
        }

        public void Execute()
        {
            WriteRunInfo();

            try
            {
                this.testDomain = options.UseAppDomain
                    ? CreateDomain(Path.GetDirectoryName(Path.GetFullPath(options.Parameters[0])))
                    : AppDomain.CurrentDomain;

                TestRunner runner = (TestRunner)CreateObject( runnerTypeName );
                
                TestPackage package = new TestPackage("Top Level Suite", options.Parameters);
                package.Settings["AutoNamespaceSuites"] = false;
                package.Settings["MergeAssemblies"] = true;

                if (!runner.Load(package))
                    Console.WriteLine("Unable to load package");
                else
                {
                    TextWriter savedOut = Console.Out;
                    TextWriter savedError = Console.Error;

                    TestEventListener listener = new TestEventListener(options, Console.Out);

                    TestResult result = runner.Run(listener, TestFilter.Empty);

                    Console.SetOut(savedOut);
                    Console.SetError(savedError);

                    new ResultReporter(result).ReportResults();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (options.Wait)
                {
                    Console.WriteLine("Press Enter key to continue . . .");
                    Console.ReadLine();
                }
            }
        }

        private void WriteRunInfo()
        {
            string clrPlatform = Type.GetType("Mono.Runtime", false) == null ? ".NET" : "Mono";
            Console.WriteLine("Runtime Environment -");
            Console.WriteLine("    OS Version: {0}", Environment.OSVersion);
            Console.WriteLine("  {0} Version: {1}", clrPlatform, Environment.Version);
            Console.WriteLine();

            Console.WriteLine("Options -");
            Console.WriteLine( options.UseAppDomain
                ? "    Use Separate AppDomain"
                : "    Use Same AppDomain" );
            Console.WriteLine();
        }

        #region Helper Methods

        private static AppDomain CreateDomain(string appBase)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = appBase;
            AppDomain domain = AppDomain.CreateDomain("test-domain", null, setup);
            return domain;
        }

        private object CreateObject(string typeName, params object[] args)
        {
            return this.testDomain.CreateInstanceAndUnwrap(
                frameworkAssembly,
                typeName,
                false,
                0,
                null,
                args,
                null,
                null,
                null);
        }

        #endregion
    }
}
