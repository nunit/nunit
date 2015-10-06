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

using System.IO;
using System.Reflection;

namespace NUnitLite.Runner
{
    /// <summary>
    /// The AutoRun class is used by executable test
    /// assemblies to control their own execution.
    /// </summary>
    public class AutoRun
    {
        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments.
        /// </summary>
        /// <param name="args">Execution options</param>
        public int Execute(Assembly callingAssembly, TextWriter writer, TextReader reader, string[] args)
        {
            var options = new NUnitLiteOptions(args);

            var _textUI = new TextUI(writer, reader, options);

            if (!options.NoHeader)
                _textUI.DisplayHeader();

            if (options.ShowHelp)
            {
                _textUI.DisplayHelp();
                return TextRunner.OK;
            }

            if (options.ErrorMessages.Count > 0)
            {
                _textUI.DisplayErrors(options.ErrorMessages);
                _textUI.DisplayHelp();

                return TextRunner.INVALID_ARG;
            }

            if (options.InputFiles.Count > 0)
            {
                _textUI.DisplayError("Input assemblies may not be specified when using the NUnitLite AutoRunner");
                return TextRunner.INVALID_ARG;
            }

            _textUI.DisplayTestFiles(new string[] { callingAssembly.GetName().Name });
            _textUI.DisplayRequestedOptions();

            if (options.WaitBeforeExit && options.OutFile != null)
                writer.WriteLine("Ignoring /wait option - only valid for Console");

            try
            {
                return new TextRunner(_textUI, options).Execute(callingAssembly);
            }
            finally
            {
                if (options.WaitBeforeExit)
                    _textUI.WaitForUser("Press Enter key to continue . . .");
            }
        }
    }
}
