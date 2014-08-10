// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Text;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    /// <summary>
    /// The CommandLineOptions class parses and holds the values of
    /// any options entered at the command line.
    /// </summary>
    public class CommandLineOptions
    {
        private readonly string _optionChars;
        private static readonly string NL = NUnit.Env.NewLine;

        #region Constructors

        /// <summary>
        /// Construct a CommandLineOptions object using default option chars
        /// </summary>
        public CommandLineOptions()
            : this(Path.DirectorySeparatorChar == '/' ? "-" : "/-") { }

        /// <summary>
        /// Construct a CommandLineOptions object using specified option chars
        /// </summary>
        /// <param name="optionChars"></param>
        public CommandLineOptions(string optionChars)
        {
            _optionChars = optionChars;

            Tests = new List<string>();
            Parameters = new List<string>();
            _invalidOptions = new List<string>();
            InternalTraceLevel = InternalTraceLevel.Off;
        }

        #endregion

        #region Properties

        /// <summary>Indicates whether the 'wait' option was used.</summary>
        public bool Wait { get; private set; }

        /// <summary>Indicates whether the 'nologo' option was used.</summary>
        public bool NoHeader { get; private set; }

        /// <summary>Indicates whether the 'help' option was used.</summary>
        public bool ShowHelp { get; private set; }

        /// <summary>Indicates whether each test should be labeled in the output.</summary>
        public bool ShowLabels { get; private set; }

        /// <summary>Indicates whether tests should be listed rather than run.</summary>
        public bool Explore { get; private set; }

        /// <summary>Gets the name of the file to be used for listing tests.</summary>
        public string ExploreFile { get; private set; }

        /// <summary>Gets the name of the file to be used for test results.</summary>
        public string ResultFile { get; private set; }

        /// <summary>Gets the format to be used for test results.</summary>
        public string ResultFormat { get; private set; }

        /// <summary>Gets the full path of the file to be used for output.</summary>
        public string OutFile { get; private set; }

        /// <summary>Gets a list of all tests specified on the command line.</summary>
        public List<string> Tests { get; private set; }

        /// <summary>Indicates whether we should display TeamCity service messages.</summary>
        public bool DisplayTeamCityServiceMessages { get; private set; }

        /// <summary>Indicates whether a full report should be displayed.</summary>
        public bool Full { get; private set; }

        /// <summary>Gets the list of categories to include.</summary>
        public string Include { get; private set; }

        /// <summary>Gets the list of categories to exclude.</summary>
        public string Exclude { get; private set; }

        /// <summary>
        /// Set internal trace {LEVEL}.
        /// Values: Off, Error, Warning, Info, Verbose (Debug)
        /// </summary>
        public InternalTraceLevel InternalTraceLevel { get; private set; }

        private string ExpandToFullPath(string path)
        {
            if (path == null) return null;

#if NETCF
            return Path.Combine(NUnit.Env.DocumentFolder, path);
#else
            return Path.GetFullPath(path);
#endif
        }

        private int _randomSeed = -1;
        /// <summary>
        /// Gets the initial random seed. If the InitialSeed is not set, it will randomly generate a new one.
        /// </summary>
        /// <value>
        /// The initial seed.
        /// </value>
        public int InitialSeed
        {
            get
            {
                if (_randomSeed < 0)
                    _randomSeed = new Random().Next();

                return _randomSeed;
            }
        }

        /// <summary>Gets the parameters provided on the commandline</summary>
        public List<string> Parameters { get; private set; }

        /// <summary>Indicates whether there was an error in parsing the options.</summary>
        public bool Error { get; private set; }

        private readonly List<string> _invalidOptions;
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string opt in _invalidOptions)
                    sb.Append("Invalid option: " + opt + NL);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

#if PocketPC || WindowsCE || NETCF || SILVERLIGHT
                string name = "NUnitLite";
#else
                string name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
#endif

                sb.Append("Usage: " + name + " [assemblies] [options]" + NL + NL);
                sb.Append("Runs a set of NUnitLite tests from the console." + NL + NL);
                sb.Append("You may specify one or more test assemblies by name, without a path or" + NL);
                sb.Append("extension. They must be in the same in the same directory as the exe" + NL);
                sb.Append("or on the probing path. If no assemblies are provided, tests in the" + NL);
                sb.Append("executing assembly itself are run." + NL + NL);
                sb.Append("Options:" + NL);
                sb.Append("  -test:testname  The name of a test to run or explore. This option may be repeated." + NL);
                sb.Append("                  If no test names are given, all tests are run." + NL + NL);
                sb.Append("  -out:FILE       File to which output is redirected. If this option is not" + NL);
                sb.Append("                  used, output is to the Console, which means it is lost" + NL);
                sb.Append("                  on devices without a Console." + NL + NL);
                sb.Append("  -full           Prints full report of all test results." + NL + NL);
                sb.Append("  -result:FILE    File to which the xml test result is written." + NL + NL);
                sb.Append("  -format:FORMAT  Format in which the result is to be written. FORMAT must be" + NL);
                sb.Append("                  either nunit3 or nunit2. The default is nunit3." + NL + NL);
                sb.Append("  -explore:FILE  If provided, this option indicates that the tests" + NL);
                sb.Append("                  should be listed rather than executed. They are listed" + NL);
                sb.Append("                  to the specified file in XML format." + NL);
                sb.Append("  -help,-h        Displays this help" + NL + NL);
                sb.Append("  -noheader,-noh  Suppresses display of the initial message" + NL + NL);
                sb.Append("  -trace:level    Set internal trace {LEVEL}." + NL);
                sb.Append("                  Values: Off, Error, Warning, Info, Verbose" + NL + NL);
                sb.Append("  -labels         Displays the name of each test when it starts" + NL + NL);
                sb.Append("  -seed:SEED      Specify the random seed used in generating test cases." + NL + NL);
                sb.Append("  -include:CAT    List of categories to include" + NL + NL);
                sb.Append("  -exclude:CAT    List of categories to exclude" + NL + NL);
                sb.Append("  -wait           Waits for a key press before exiting" + NL + NL);
                sb.Append("Notes:" + NL);
                sb.Append(" * File names may be listed by themselves, with a relative path or " + NL);
                sb.Append("   using an absolute path. Any relative path is based on the current " + NL);
                sb.Append("   directory or on the Documents folder if running on a under the " + NL);
                sb.Append("   compact framework." + NL + NL);
                if (System.IO.Path.DirectorySeparatorChar != '/')
                    sb.Append(" * On Windows, options may be prefixed by a '/' character if desired" + NL + NL);
                sb.Append(" * Options that take values may use an equal sign or a colon" + NL);
                sb.Append("   to separate the option from its value." + NL + NL);

                return sb.ToString();
            }
        }

        #endregion

        #region Process Commandline

        /// <summary>
        /// Parse command arguments and initialize option settings accordingly
        /// </summary>
        /// <param name="args">The argument list</param>
        public void Parse(params string[] args)
        {
            foreach( string arg in args )
            {
                if (_optionChars.IndexOf(arg[0]) >= 0 )
                    ProcessOption(arg);
                else
                    ProcessParameter(arg);
            }
        }

        private void ProcessOption(string option)
        {
            string opt = option;
            int pos = opt.IndexOfAny( new char[] { ':', '=' } );
            string val = string.Empty;

            if (pos >= 0)
            {
                val = opt.Substring(pos + 1);
                opt = opt.Substring(0, pos);
            }

            opt = opt.StartsWith("--")
                ? opt.Substring(2)
                : opt.Substring(1);

            switch (opt)
            {
                case "wait":
                    Wait = true;
                    break;
                case "noheader":
                case "noh":
                    NoHeader = true;
                    break;
                case "help":
                case "h":
                    ShowHelp = true;
                    break;
                case "test":
                    Tests.Add(val);
                    break;
                case "full":
                    Full = true;
                    break;
                case "explore":
                    Explore = true;
                    if (val == null || val.Length == 0)
                        val = "tests.xml";
                    try
                    {
                        ExploreFile = ExpandToFullPath(val);
                    }
                    catch
                    {
                        InvalidOption(option);
                    }
                    break;
                case "result":
                    if (val == null || val.Length == 0)
                        val = "TestResult.xml";
                    try
                    {
                        ResultFile = ExpandToFullPath(val);
                    }
                    catch
                    {
                        InvalidOption(option);
                    }
                    break;
                case "format":
                    ResultFormat = val;
                    if (ResultFormat != "nunit3" && ResultFormat != "nunit2")
                        InvalidOption(option);
                    break;
                case "out":
                    try
                    {
                        OutFile = ExpandToFullPath(val);
                    }
                    catch
                    {
                        InvalidOption(option);
                    }
                    break;
                case "labels":
                    ShowLabels = true;
                    break;
                case "include":
                    Include = val;
                    break;
                case "exclude":
                    Exclude = val;
                    break;
                case "seed":
                    try
                    {
                        _randomSeed = int.Parse(val);
                    }
                    catch
                    {
                        InvalidOption(option);
                    }
                    break;
                case "trace":
                    try
                    {
                        InternalTraceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), val, true);
                    }
                    catch
                    {
                        InvalidOption(option);
                    }
                    break;
                default:
                    InvalidOption(option);
                    break;
            }
        }

        private void InvalidOption(string option)
        {
            Error = true;
            _invalidOptions.Add(option);
        }

        private void ProcessParameter(string param)
        {
            Parameters.Add(param);
        }

        #endregion
    }
}
