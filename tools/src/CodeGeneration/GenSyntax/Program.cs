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

namespace NUnit.Framework.CodeGeneration
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	class Program
	{
        static string InputFile;
        static List<string> GenOptions = new List<string>();

        static StreamReader InputReader;
        static List<StreamWriter> OutputWriters = new List<StreamWriter>();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            try
            {
                if (ProcessArgs(args))
                {
                    SyntaxInfo.Instance.Load(InputReader);

                    if (GenOptions.Count == 0)
                        GenOptions = SyntaxInfo.Instance.Defaults;

                    foreach (string option in GenOptions)
                    {
                        string className, targetName;
                        int eq = option.IndexOf('=');
                        if (eq > 0)
                        {
                            className = option.Substring(0, eq);
                            targetName = option.Substring(eq + 1);
                        }
                        else
                        {
                            className = option;
                            targetName = className + ".cs";
                        }
                        
                        CodeGenerator generator = new CodeGenerator( className, targetName );

                        CodeWriter writer = new IndentedTextWriter(new StreamWriter(targetName));

                        Console.WriteLine("Generating " + targetName);

                        generator.GenerateClass(writer);
                    }
                }
                else
                    Usage();
            }
            catch (CommandLineError ex)
            {
                Error(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                Error(ex.Message);
            }
		}

        static bool ProcessArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "-help")
                    return false;
                else if (arg.StartsWith("-gen:"))
                    GenOptions.Add(arg.Substring(5));
                else if (InputFile == null)
                    InputFile = arg;
                else
                    throw new CommandLineError(string.Format("Unknown option: {0}", arg));
            }

            if (InputFile == null) throw new CommandLineError("No input file provided");

            InputReader = new StreamReader(InputFile);

            return true;
        }

        static void Help()
        {
            Console.Error.WriteLine("Generates C# code for NUnit syntax elements");
            Console.Error.WriteLine();
            Usage();
        }

        static void Error(string message)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
            Usage();
        }

        static void Usage()
        {
            Console.Error.WriteLine("Usage: GenSyntax <input_file> [ [ [-gen:<class_name>[=<file_name>] ] ...]");
            Console.Error.WriteLine();
            Console.Error.WriteLine("The <input_file> is required. If any -gen options are given, only the code");
            Console.Error.WriteLine("for the specified classes are generated. If <file_name> is not specified,");
            Console.Error.WriteLine("it defaults to the <class_name> with a .cs extension. If no -gen options");
            Console.Error.WriteLine("are used, Default entries specified in the input file are generated.");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Syntax for entries in the input file are described in the file");
            Console.Error.WriteLine("SyntaxElements.txt, which is distributed with the NUnit source.");
        }
	}

    class CommandLineError : Exception
    {
        public CommandLineError(string message) : base(message) { }
    }
}
