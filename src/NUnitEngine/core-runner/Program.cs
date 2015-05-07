// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Text;
using System.Xml;
using NUnit.Common;
using NUnit.Engine;

namespace NUnit.CoreRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Error("No arguments provided");
            else if (args.Length > 1)
                Error("Too many arguments");
            else
                RunTests(args[0]);
        }

        private static void RunTests(string assemblyPath)
        {
            var package = new TestPackage(assemblyPath);
            var engine = new TestEngine();
            var runner = engine.GetRunner(package);

            XmlNode result = runner.Run(new TestListener(), TestFilter.Empty);
            string overallResult = result.GetAttribute("result");

            Console.WriteLine("Test Run for {0} {1}", assemblyPath, result.GetAttribute("result"));
            Console.WriteLine("    Passing Tests:      {0}", result.GetAttribute("passed"));
            Console.WriteLine("    Failing Tests:      {0}", result.GetAttribute("failed"));
            Console.WriteLine("    Inconclusive Tests: {0}", result.GetAttribute("inconclusive"));
            Console.WriteLine("    Skipped Tests:      {0}", result.GetAttribute("skipped"));
            Console.WriteLine();
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: CORE-RUNNER <assembly_path>");
            Console.WriteLine();
        }

        private static void Error(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            Usage();
        }

        // Listener currently does nothing
        class TestListener : MarshalByRefObject, ITestEventListener
        {
            public void OnTestEvent(string report)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(report);
                XmlNode testEvent = doc.FirstChild;

                switch (testEvent.Name)
                {
                    case "start-test":
                        break;

                    case "test-case":
                        break;

                    case "start-suite":
                        break;

                    case "test-suite":
                        break;

                    case "start-run":
                        break;
                }
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}
