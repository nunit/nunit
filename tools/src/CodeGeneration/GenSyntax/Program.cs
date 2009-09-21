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
using System.Collections.Generic;
using NDesk.Options;

namespace NUnit.Framework.CodeGeneration
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	class Program
	{
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            bool showHelp = false;
            List<string> targets = new List<string>();
            string syntaxFile = null;
            string templateDir = null;
            StreamReader inputReader;

            OptionSet options = new OptionSet() {
                { "f|syntax=", "The {PATH} to the syntax definition file.\nDefault: SyntaxElements.txt.", v => syntaxFile = v },
                { "t|templates=", "The {PATH} to the template directory.\nDefault: ./Templates", v => templateDir = v },
                { "h|?|help", "Display this message and exit.", v => showHelp = v != null },
                { "<>", v => {
                    if (v.StartsWith("-") || v.StartsWith("/") && Path.DirectorySeparatorChar != '/')
                        Error("Invalid option: " + v);
                    else
                        targets.Add(v); }
                }
            };

            try
            {
                options.Parse(args);
            }
            catch(OptionException ex)
            {
                Error(ex.Message);
                return;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (syntaxFile == null) 
                syntaxFile = "SyntaxElements.txt";
            if (templateDir == null)
                templateDir = "Templates";

            if (targets.Count == 0)
            {
                Error("At least one target must be specified");
                return;
            }

            try
            {
                inputReader = new StreamReader(syntaxFile);

                SyntaxInfo.Instance.Load(inputReader);

                foreach (string targetName in targets)
                {
                    string className = Path.GetFileNameWithoutExtension(targetName);
                    string templateName = Path.Combine("Templates", className + ".template.cs");

                    CodeGenerator generator = new CodeGenerator(className, templateName);

                    CodeWriter writer = new IndentedTextWriter(new StreamWriter(targetName));

                    Console.WriteLine("Generating " + targetName);

                    generator.GenerateClass(writer);
                }
            }
            catch (FileNotFoundException ex)
            {
                Error(ex.Message);
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        static void ShowHelp(OptionSet options)
        {
            Console.Error.WriteLine("Usage: GenSyntax [OPTION...] TARGET...");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Generates C# code for one or more target classes. The content of each");
            Console.Error.WriteLine("output file is controlled by an individual template and a common syntax");
            Console.Error.WriteLine("definition file.");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Templates are stored in a separate templates directory and are named");
            Console.Error.WriteLine("according the target file for which they are used. For example, the");
            Console.Error.WriteLine("template for file CLASS.cs is CLASS.template.cs.");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Error);
        }

        static void Error(string message)
        {
            Console.Error.WriteLine("GenSyntax: {0}", message);
            Console.Error.WriteLine("Try 'GenSyntax --help' for more information");
        }
	}
}
