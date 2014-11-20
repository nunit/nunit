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
using System.IO;
using System.Reflection;
using System.Xml;

namespace NUnit.ConsoleRunner
{
    public class OutputManager
    {
        private readonly string _workDirectory;

        public OutputManager(string workDirectory)
        {
            _workDirectory = workDirectory;
        }

        public void CheckWritability(OutputSpecification spec)
        {
            string outputPath = GetOutputPath(spec.OutputPath);
            IResultWriter outputWriter = CreateOutputWriter(spec.Format, spec.Transform);
            outputWriter.CheckWritability(outputPath);
        }

        public void WriteResultFile(XmlNode result, OutputSpecification spec)
        {
            string outputPath = GetOutputPath(spec.OutputPath);
            IResultWriter outputWriter = CreateOutputWriter(spec.Format, spec.Transform);
            outputWriter.WriteResultFile(result, outputPath);
            Console.WriteLine("Results ({0}) saved as {1}", spec.Format, outputPath);
        }

        private string GetOutputPath( string filename )
        {
            return Path.Combine( _workDirectory, filename );
        }

        private static IResultWriter CreateOutputWriter( string spec, string transform )
        {
            IResultWriter outputWriter;
            switch ( spec )
            {
                case "nunit3":
                    outputWriter = new NUnit3XmlOutputWriter( );
                    break;

                case "nunit2":
                    outputWriter = new NUnit2XmlOutputWriter( );
                    break;

                case "user":
                    Uri uri = new Uri( Assembly.GetExecutingAssembly( ).CodeBase );
                    string dir = Path.GetDirectoryName( uri.LocalPath );
                    outputWriter = new XmlTransformOutputWriter( Path.Combine( dir, transform ) );
                    break;

                default:
                    throw new ArgumentException( string.Format( "Invalid XML output format '{0}'", spec ), "spec" );
            }
            return outputWriter;
        }
    }
}
