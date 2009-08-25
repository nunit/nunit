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
using System.Reflection;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace NUnit.Framework.CodeGeneration
{
    /// <summary>
    /// Generates code for a single class
    /// </summary>
    class CodeGenerator
    {
        private string className;
        private string fileName;
        private StreamReader template;
        private bool isStatic;

        public CodeGenerator(string option)
        {
            int eq = option.IndexOf('=');
            if (eq > 0)
            {
                this.className = option.Substring(0, eq);
                this.fileName = option.Substring(eq + 1);
            }
            else
            {
                this.className = option;
                this.fileName = className + ".cs";
            }

            Assembly assembly = GetType().Assembly;

            Stream stream = assembly.GetManifestResourceStream("NUnit.Framework.CodeGeneration.Templates." + className + ".template.cs");
            if (stream == null)
                stream = assembly.GetManifestResourceStream("NUnit.Framework.CodeGeneration.Templates.Default.template.cs");

            this.template = new StreamReader(stream);
        }

        public void GenerateClass()
        {
            IndentedTextWriter writer = new IndentedTextWriter(new StreamWriter(this.fileName));

            Console.WriteLine("Generating " + this.fileName);

            WriteFileHeader(writer);

            foreach (CodeGenSpec stanza in SyntaxInfo.Instance)
            {
                stanza.Generate(writer, this.className, isStatic);
            }

            WriteFileTrailer(writer);

            writer.Close();
        }

        private void WriteFileHeader(IndentedTextWriter writer)
        {
            string[] argList = Environment.GetCommandLineArgs();
            argList[0] = Path.GetFileName(argList[0]);
            string commandLine = string.Join(" ", argList);

            string line = template.ReadLine();
            while (line != null && line.IndexOf("$$GENERATE$$") < 0)
            {
                line = line.Replace("__CLASSNAME__", this.className)
                           .Replace("__COMMANDLINE__", commandLine);
                writer.WriteLine(line);
                line = template.ReadLine();
            }

            if( line != null && line.IndexOf("$$STATIC$$") >= 0)
                this.isStatic = true;

            writer.Indent += 2;
        }

        private void WriteFileTrailer(IndentedTextWriter writer)
        {
            writer.Indent -= 2;

            while (!template.EndOfStream)
                writer.WriteLine(template.ReadLine());
        }
    }
}
