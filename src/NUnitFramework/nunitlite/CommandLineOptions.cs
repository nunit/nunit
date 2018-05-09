// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Options;
using System.Text;

namespace NUnit.Common
{
    /// <summary>
    /// CommandLineOptions is the base class the specific option classes
    /// used for nunit3-console and nunitlite. It encapsulates all common
    /// settings and features of both. This is done to ensure that common
    /// features remain common and for the convenience of having the code
    /// in a common location. The class inherits from the Mono
    /// Options OptionSet class and provides a central location
    /// for defining and parsing options.
    /// </summary>
    public class CommandLineOptions : OptionSet
    {
        private static readonly string DEFAULT_WORK_DIRECTORY =
            Directory.GetCurrentDirectory();

        private bool validated;
        private bool noresult;

        #region Constructors

        // Currently used only by tests
        internal CommandLineOptions(IDefaultOptionsProvider defaultOptionsProvider, bool requireInputFile, params string[] args)
        {
            // Apply default options
            if (defaultOptionsProvider == null) throw new ArgumentNullException(nameof(defaultOptionsProvider));

            TeamCity = defaultOptionsProvider.TeamCity;

            ConfigureOptions(requireInputFile);
            if (args != null)
                Parse(PreParse(args));
        }

        public CommandLineOptions(bool requireInputFile, params string[] args)
        {
            ConfigureOptions(requireInputFile);
            if (args != null)
                Parse(PreParse(args));
        }

        private int _nesting = 0;

        internal IEnumerable<string> PreParse(IEnumerable<string> args)
        {
            if (++_nesting > 3)
            {
                ErrorMessages.Add("@ nesting exceeds maximum depth of 3.");
                --_nesting;
                return args;
            }

            var listArgs = new List<string>();

            foreach (var arg in args)
            {
                if (arg.Length == 0 || arg[0] != '@')
                {
                    listArgs.Add(arg);
                    continue;
                }

                if (arg.Length == 1)
                {
                    ErrorMessages.Add("You must include a file name after @.");
                    continue;
                }

                var filename = arg.Substring(1);

                if (!File.Exists(filename))
                {
                    ErrorMessages.Add("The file \"" + filename + "\" was not found.");
                    continue;
                }

                try
                {
                    listArgs.AddRange(PreParse(GetArgsFromFile(filename)));
                }
                catch (IOException ex)
                {
                    ErrorMessages.Add("Error reading \"" + filename + "\": " + ex.Message);
                }
            }

            --_nesting;
            return listArgs;
        }

        private static readonly Regex ArgsRegex = new Regex(@"\G(""((""""|[^""])+)""|(\S+)) *", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // Get args from a string of args
        internal static IEnumerable<string> GetArgs(string commandLine)
        {
            foreach (Match m in ArgsRegex.Matches(commandLine))
                yield return Regex.Replace(m.Groups[2].Success ? m.Groups[2].Value : m.Groups[4].Value, @"""""", @"""");
        }

        // Get args from an included file
        private static IEnumerable<string> GetArgsFromFile(string filename)
        {
            var sb = new StringBuilder();

            foreach (var line in File.ReadAllLines(filename))
            {
                if (!string.IsNullOrEmpty(line) && line[0] != '#' && line.Trim().Length > 0)
                {
                    if (sb.Length > 0)
                        sb.Append(' ');
                    sb.Append(line);
                }
            }

            return GetArgs(sb.ToString());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether an input file is required on the command-line.
        /// Note that multiple input files are never allowed.
        /// </summary>
        public bool InputFileRequired { get; set; }

        // Action to Perform

        public bool Explore { get; private set; }

        public bool ShowHelp { get; private set; }

        public bool ShowVersion { get; private set; }

        // Select tests

        public string InputFile { get; private set; }

        public IList<string> TestList { get; } = new List<string>();

        public IDictionary<string, string> TestParameters { get; } = new Dictionary<string, string>();

        public string WhereClause { get; private set; }
        public bool WhereClauseSpecified { get { return WhereClause != null; } }

        public int DefaultTimeout { get; private set; } = -1;
        public bool DefaultTimeoutSpecified { get { return DefaultTimeout >= 0; } }

        public int RandomSeed { get; private set; } = -1;
        public bool RandomSeedSpecified { get { return RandomSeed >= 0; } }

        public string DefaultTestNamePattern { get; private set; }

        public int NumberOfTestWorkers { get; private set; } = -1;
        public bool NumberOfTestWorkersSpecified { get { return NumberOfTestWorkers >= 0; } }

        public bool StopOnError { get; private set; }

        public bool WaitBeforeExit { get; private set; }

        // Output Control
        public bool NoHeader { get; private set; }

        public bool NoColor { get; private set; }

        public bool TeamCity { get; private set; }

        public string OutFile { get; private set; }
        public bool OutFileSpecified { get { return OutFile != null; } }

        public string ErrFile { get; private set; }
        public bool ErrFileSpecified { get { return ErrFile != null; } }

        public string DisplayTestLabels { get; private set; }

        private string workDirectory = null;
        public string WorkDirectory
        {
            get { return workDirectory ?? DEFAULT_WORK_DIRECTORY; }
        }
        public bool WorkDirectorySpecified { get { return workDirectory != null; } }

        public string InternalTraceLevel { get; private set; }
        public bool InternalTraceLevelSpecified { get { return InternalTraceLevel != null; } }

        private readonly List<OutputSpecification> resultOutputSpecifications = new List<OutputSpecification>();
        public IList<OutputSpecification> ResultOutputSpecifications
        {
            get
            {
                if (noresult)
                    return new OutputSpecification[0];

                if (resultOutputSpecifications.Count == 0)
                    resultOutputSpecifications.Add(new OutputSpecification("TestResult.xml"));

                return resultOutputSpecifications;
            }
        }

        public IList<OutputSpecification> ExploreOutputSpecifications { get; } = new List<OutputSpecification>();

        // Error Processing

        public IList<string> ErrorMessages { get; } = new List<string>();

        #endregion

        #region Public Methods

        public bool Validate()
        {
            if (!validated)
            {
                CheckOptionCombinations();

                validated = true;
            }

            return ErrorMessages.Count == 0;
        }

        #endregion

        #region Helper Methods

        protected virtual void CheckOptionCombinations()
        {

        }

        /// <summary>
        /// Case is ignored when val is compared to validValues. When a match is found, the
        /// returned value will be in the canonical case from validValues.
        /// </summary>
        protected string RequiredValue(string val, string option, params string[] validValues)
        {
            if (string.IsNullOrEmpty(val))
                ErrorMessages.Add("Missing required value for option '" + option + "'.");

            bool isValid = true;

            if (validValues != null && validValues.Length > 0)
            {
                isValid = false;

                foreach (string valid in validValues)
                    if (string.Compare(valid, val, StringComparison.OrdinalIgnoreCase) == 0)
                        return valid;

            }

            if (!isValid)
                ErrorMessages.Add(string.Format("The value '{0}' is not valid for option '{1}'.", val, option));

            return val;
        }

        protected int RequiredInt(string val, string option)
        {
            int result;
            if (int.TryParse(val, out result)) return result;

            ErrorMessages.Add(string.IsNullOrEmpty(val)
                ? "Missing required value for option '" + option + "'."
                : "An int value was expected for option '{0}' but a value of '{1}' was used");

            // We have to return something even though the value will
            // be ignored if an error is reported. The -1 value seems
            // like a safe bet in case it isn't ignored due to a bug.
            return -1;
        }

        private string ExpandToFullPath(string path)
        {
            if (path == null) return null;

            return Path.GetFullPath(path);
        }

        protected virtual void ConfigureOptions(bool allowInputFile)
        {
            InputFileRequired = allowInputFile;

            // NOTE: The order in which patterns are added
            // determines the display order for the help.

            // Select Tests
            this.Add("test=", "Comma-separated list of {NAMES} of tests to run or explore. This option may be repeated.",
                v => ((List<string>)TestList).AddRange(TestNameParser.Parse(RequiredValue(v, "--test"))));

            this.Add("testlist=", "File {PATH} containing a list of tests to run, one per line. This option may be repeated.",
                v =>
                {
                    string testListFile = RequiredValue(v, "--testlist");

                    var fullTestListPath = ExpandToFullPath(testListFile);

                    if (!File.Exists(fullTestListPath))
                        ErrorMessages.Add("Unable to locate file: " + testListFile);
                    else
                    {
                        try
                        {
                            using (var str = new FileStream(fullTestListPath, FileMode.Open))
                            using (var rdr = new StreamReader(str))
                            {
                                while (!rdr.EndOfStream)
                                {
                                    var line = rdr.ReadLine().Trim();

                                    if (!string.IsNullOrEmpty(line) && line[0] != '#')
                                        ((List<string>)TestList).Add(line);
                                }
                            }
                        }
                        catch (IOException)
                        {
                            ErrorMessages.Add("Unable to read file: " + testListFile);
                        }
                    }
                });

            this.Add("where=", "Test selection {EXPRESSION} indicating what tests will be run. See description below.",
                v => WhereClause = RequiredValue(v, "--where"));

            this.Add("params|p=", "Define a test parameter.",
                v =>
                {
                    string parameters = RequiredValue(v, "--params");

                    // This can be changed without breaking backwards compatibility with frameworks.
                    foreach (string param in parameters.Split(new[] { ';' }))
                    {
                        int eq = param.IndexOf("=");
                        if (eq == -1 || eq == param.Length - 1)
                        {
                            ErrorMessages.Add("Invalid format for test parameter. Use NAME=VALUE.");
                        }
                        else
                        {
                            string name = param.Substring(0, eq);
                            string val = param.Substring(eq + 1);

                            TestParameters[name] = val;
                        }
                    }
                });
#if !NETSTANDARD1_6
            this.Add("timeout=", "Set timeout for each test case in {MILLISECONDS}.",
                v => DefaultTimeout = RequiredInt(v, "--timeout"));
#endif
            this.Add("seed=", "Set the random {SEED} used to generate test cases.",
                v => RandomSeed = RequiredInt(v, "--seed"));

            this.Add("workers=", "Specify the {NUMBER} of worker threads to be used in running tests. If not specified, defaults to 2 or the number of processors, whichever is greater.",
                v => NumberOfTestWorkers = RequiredInt(v, "--workers"));

            this.Add("stoponerror", "Stop run immediately upon any test failure or error.",
                v => StopOnError = v != null);

            this.Add("wait", "Wait for input before closing console window.",
                v => WaitBeforeExit = v != null);

            // Output Control
            this.Add("work=", "{PATH} of the directory to use for output files. If not specified, defaults to the current directory.",
                v => workDirectory = RequiredValue(v, "--work"));

            this.Add("output|out=", "File {PATH} to contain text output from the tests.",
                v => OutFile = RequiredValue(v, "--output"));

            this.Add("err=", "File {PATH} to contain error output from the tests.",
                v => ErrFile = RequiredValue(v, "--err"));

            this.Add("result=", "An output {SPEC} for saving the test results. This option may be repeated.",
                v => ResolveOutputSpecification(RequiredValue(v, "--resultxml"), resultOutputSpecifications));

            this.Add("explore:", "Display or save test info rather than running tests. Optionally provide an output {SPEC} for saving the test info. This option may be repeated.", v =>
            {
                Explore = true;
                ResolveOutputSpecification(v, ExploreOutputSpecifications);
            });

            this.Add("noresult", "Don't save any test results.",
                v => noresult = v != null);

            this.Add("labels=", "Specify whether to write test case names to the output. Values: Off, On, All",
                v => DisplayTestLabels = RequiredValue(v, "--labels", "Off", "On", "Before", "After", "All"));

            this.Add("test-name-format=", "Non-standard naming pattern to use in generating test names.",
                v => DefaultTestNamePattern = RequiredValue(v, "--test-name-format"));

            this.Add("teamcity", "Turns on use of TeamCity service messages.",
                v => TeamCity = v != null);

            this.Add("trace=", "Set internal trace {LEVEL}.\nValues: Off, Error, Warning, Info, Verbose (Debug)",
                v => InternalTraceLevel = RequiredValue(v, "--trace", "Off", "Error", "Warning", "Info", "Verbose", "Debug"));

            this.Add("noheader|noh", "Suppress display of program information at start of run.",
                v => NoHeader = v != null);

            this.Add("nocolor|noc", "Displays console output without color.",
                v => NoColor = v != null);

            this.Add("help|h", "Display this message and exit.",
                v => ShowHelp = v != null);

            this.Add("version|V", "Display the header and exit.",
                v => ShowVersion = v != null);

            // Default
            this.Add("<>", v =>
            {
                if (LooksLikeAnOption(v))
                    ErrorMessages.Add("Invalid argument: " + v);
                else if (InputFileRequired)
                    if (InputFile == null)
                        InputFile = v;
                    else
                        ErrorMessages.Add("Multiple file names are not allowed on the command-line.\n    Invalid entry: " + v);
                else
                    ErrorMessages.Add("Do not provide a file name when running a self-executing test.\n    Invalid entry: " + v);
            });
        }

        private bool LooksLikeAnOption(string v)
        {
            return v.StartsWith("-") || v.StartsWith("/") && Path.DirectorySeparatorChar != '/';
        }

        private void ResolveOutputSpecification(string value, IList<OutputSpecification> outputSpecifications)
        {
            if (value == null)
                return;

            try
            {
                var spec = new OutputSpecification(value);
                outputSpecifications.Add(spec);
            }
            catch (ArgumentException e)
            {
                ErrorMessages.Add(e.Message);
            }
        }

        #endregion
    }
}
