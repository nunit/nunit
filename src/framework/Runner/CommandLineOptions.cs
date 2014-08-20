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
        private static readonly string NL = NUnit.Env.NewLine;

        #region Constructor

        /// <summary>
        /// Construct a CommandLineOptions object using specified arguments
        /// </summary>
        public CommandLineOptions(params string[] args)
        {
            Tests = new List<string>();
            InputFiles = new List<string>();
            ErrorMessages = new List<string>();

            InternalTraceLevel = "Off";
            DisplayTestLabels = "Off";
            DefaultTimeout = -1;

            this.Parse(args);
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
        public string DisplayTestLabels { get; private set; }

        /// <summary>Indicates whether tests should be listed rather than run.</summary>
        public bool Explore { get; private set; }

        /// <summary>Gets the name of the file to be used for listing tests.</summary>
        public string ExploreFile { get; private set; }

        /// <summary>Gets the name of the file to be used for test results.</summary>
        public string ResultFile { get; private set; }

        /// <summary>Gets the format to be used for test results.</summary>
        public string ResultFormat { get; private set; }

        /// <summary>Gets the full path of the file to be used for standard output.</summary>
        public string OutFile { get; private set; }

        /// <summary>Gets the full path of the file to be used for error output.</summary>
        public string ErrFile { get; private set; }

        /// <summary>Gets the full path of the directory used for output files./// </summary>
        public string WorkDirectory { get; private set; }

        /// <summary>Gets a list of all tests specified on the command line.</summary>
        public List<string> Tests { get; private set; }

        /// <summary>The default timeout value to be used for test cases.</summary>
        public int DefaultTimeout { get; private set; }

        /// <summary>Indicates whether we should display TeamCity service messages.</summary>
        public bool DisplayTeamCityServiceMessages { get; private set; }

        /// <summary>Indicates whether a full report should be displayed.</summary>
        public bool Full { get; private set; }

        /// <summary>Gets the list of categories to include.</summary>
        public string Include { get; private set; }

        /// <summary>Gets the list of categories to exclude.</summary>
        public string Exclude { get; private set; }

        /// <summary>Gets the internal trace level.</summary>
        public string InternalTraceLevel { get; private set; }

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
        public List<string> InputFiles { get; private set; }

        /// <summary>Our list of error messages.</summary>
        public List<string> ErrorMessages { get; private set; }

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
                sb.Append("  -work:PATH      PATH of the directory to use for output files." + NL + NL);
                sb.Append("  -output:FILE,   File to which standard output is redirected. If this option" + NL);
                sb.Append("  -out:FILE       is not used, output is to the Console, which means it is" + NL);
                sb.Append("                  lost on devices without a Console." + NL + NL);
                sb.Append("  -err:FILE       File to which error output is redirected. If this option" + NL);
                sb.Append("                  is not used, output is to the Console, which means it is" + NL);
                sb.Append("                  lost on devices without a Console." + NL + NL);
                sb.Append("  -full           Prints full report of all test results." + NL + NL);
                sb.Append("  -result:FILE    File to which the xml test result is written." + NL + NL);
                sb.Append("  -format:FORMAT  Format in which the result is to be written. FORMAT must be" + NL);
                sb.Append("                  either nunit3 or nunit2. The default is nunit3." + NL + NL);
                sb.Append("  -explore:FILE  If provided, this option indicates that the tests" + NL);
                sb.Append("                  should be listed rather than executed. They are listed" + NL);
                sb.Append("                  to the specified file in XML format." + NL);
                sb.Append("  -help,-h        Displays this help" + NL + NL);
                sb.Append("  -noheader,-noh  Suppresses display of the initial message" + NL + NL);
                sb.Append("  -trace:LEVEL    Set internal trace {LEVEL}." + NL);
                sb.Append("                  Values: Off, Error, Warning, Info, Verbose" + NL + NL);
                sb.Append("  -labels:VAL,    Specify whether to write test case names to the output." + NL);
                sb.Append("  -l:VAL          Values: Off, On, All" + NL + NL);
                sb.Append("  -teamcity       Running under TeamCity: display service messages as tests are executed" + NL + NL);
                sb.Append("  -seed:SEED      Specify the random seed used in generating test cases." + NL + NL);
                sb.Append("  -timeout:VAL     Set timeout for each test case in milliseconds." + NL + NL);
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
        private void Parse(params string[] args)
        {
            foreach( string arg in args )
            {
                if (arg[0] == '-' || arg[0] == '/')
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
                case "teamcity":
                    DisplayTeamCityServiceMessages = true;
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
                case "work":
                    WorkDirectory = RequiredValue(val, option);
                    break;
                case "output":
                case "out":
                    OutFile = RequiredValue(val, option);
                    break;
                case "err":
                    ErrFile = RequiredValue(val, option);
                    break;
                case "labels":
                case "l":
                    DisplayTestLabels = RequiredValue(val, option, "Off", "On", "All");
                    break;
                case "include":
                    Include = val;
                    break;
                case "exclude":
                    Exclude = val;
                    break;
                case "seed":
                    _randomSeed = RequiredInt(val, option);
                    break;
                case "timeout":
                    DefaultTimeout = RequiredInt(val, option);
                    break;
                case "trace":
                    InternalTraceLevel = RequiredValue(val, option, "Off", "Error", "Warning", "Info", "Debug", "Verbose");
                    break;
                default:
                    InvalidOption(option);
                    break;
            }
        }

        private void InvalidOption(string option)
        {
            ErrorMessages.Add("Invalid option: " + option);
        }

        private void ProcessParameter(string param)
        {
            InputFiles.Add(param);
        }

        #endregion

        #region Helper Methods

        private string RequiredValue(string val, string option, params string[] validValues)
        {
            if (val == null || val == string.Empty)
            {
                ErrorMessages.Add("Missing required value for option '" + option + "'.");
                return val;
            }


            bool isValid = true;

            if (validValues != null && validValues.Length > 0)
            {
                isValid = false;

                foreach (string valid in validValues)
                    if (StringUtil.Compare(valid, val, true) == 0)
                        isValid = true;

            }

            if (!isValid)
            {
                ErrorMessages.Add(string.Format("The value '{0}' is not valid for option '{1}'.", val, option));
            }

            return val;
        }

        private int RequiredInt(string val, string option)
        {
            // We have to return something even though the value will 
            // be ignored if an error is reported. The -1 value seems
            // like a safe bet in case it isn't ignored due to a bug.
            int result = -1;

            if (val == null || val == string.Empty)
                ErrorMessages.Add("Missing required value for option '" + option + "'.");
            else
            {
                int r;
                if (int.TryParse(val, out r))
                    result = r;
                else
                    ErrorMessages.Add("An int value was exprected for option '{0}' but a value of '{1}' was used");
            }

            return result;
        }

        #endregion
    }
}
