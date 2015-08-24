// ***********************************************************************
// Copyright (c) 2011-2014 Charlie Poole
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
using Mono.Options;

namespace NUnit.Common
{
    /// <summary>
    /// ConsoleOptions encapsulates the option settings for
    /// the nunit-console program. It inherits from the Mono
    /// Options OptionSet class and provides a central location
    /// for defining and parsing options.
    /// </summary>
    public class ConsoleOptions : OptionSet
    {
        private bool validated;
        private bool noresult;

        #region Constructor

        internal ConsoleOptions(IDefaultOptionsProvider defaultOptionsProvider, params string[] args)
        {
            // Apply default oprions
            if (defaultOptionsProvider == null) throw new ArgumentNullException("defaultOptionsProvider");
            TeamCity = defaultOptionsProvider.TeamCity;
            
            ConfigureOptions();            
            if (args != null)
                Parse(args);
        }

        public ConsoleOptions(params string[] args)
        {
            ConfigureOptions();
            if (args != null)
                Parse(args);
        }
        
        #endregion

        #region Properties

        // Action to Perform

        public bool Explore { get; private set; }

        public bool ShowHelp { get; private set; }

        // Select tests

        private List<string> inputFiles = new List<string>();
        public IList<string> InputFiles { get { return inputFiles; } }

        private List<string> testList = new List<string>();
        public IList<string> TestList { get { return testList; } }

        public string Include { get; private set; }

        public string Exclude { get; private set; }

        public string ActiveConfig { get; private set; }

        // Where to Run Tests

        public string ProcessModel { get; private set; }

        public string DomainUsage { get; private set; }

        // How to Run Tests

        public string Framework { get; private set; }

        public bool RunAsX86 { get; private set; }

        public bool DisposeRunners { get; private set; }

        public bool ShadowCopyFiles { get; private set; }

        private int defaultTimeout = -1;
        public int DefaultTimeout { get { return defaultTimeout; } }

        private int randomSeed = -1;
        public int RandomSeed { get { return randomSeed; } }

        private int numWorkers = -1;
        public int NumWorkers { get { return numWorkers; } }

        public bool StopOnError { get; private set; }

        public bool WaitBeforeExit { get; private set; }

        public bool PauseBeforeRun { get; private set; }

        // Output Control

        public bool NoHeader { get; private set; }

        public bool NoColor { get; private set; }

        public bool Verbose { get; private set; }

        public bool TeamCity { get; private set; }

        public string OutFile { get; private set; }

        public string ErrFile { get; private set; }

        public string DisplayTestLabels { get; private set; }

        private string workDirectory = NUnit.Env.DefaultWorkDirectory;
        public string WorkDirectory 
        {
            get { return workDirectory; }
        }

        public string InternalTraceLevel { get; private set; }

        /// <summary>Indicates whether a full report should be displayed.</summary>
        public bool Full { get; private set; }

        private List<OutputSpecification> resultOutputSpecifications = new List<OutputSpecification>();
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

        private List<OutputSpecification> exploreOutputSpecifications = new List<OutputSpecification>();
        public IList<OutputSpecification> ExploreOutputSpecifications { get { return exploreOutputSpecifications; } }

        // Error Processing

        public List<string> errorMessages = new List<string>();
        public IList<string> ErrorMessages { get { return errorMessages; } }

        #endregion

        #region Public Methods

        public bool Validate()
        {
            if (!validated)
            {
                // Additional Checks here

                validated = true;
            }

            return ErrorMessages.Count == 0;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Case is ignored when val is compared to validValues. When a match is found, the
        /// returned value will be in the canonical case from validValues.
        /// </summary>
        private string RequiredValue(string val, string option, params string[] validValues)
        {
            if (string.IsNullOrEmpty(val))
                ErrorMessages.Add("Missing required value for option '" + option + "'.");

            bool isValid = true;

            if (validValues != null && validValues.Length > 0)
            {
                isValid = false;

                foreach (string valid in validValues)
                    if (string.Compare(valid, val, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return valid;

            }

            if (!isValid)
                ErrorMessages.Add(string.Format("The value '{0}' is not valid for option '{1}'.", val, option));

            return val;
        }

        private int RequiredInt(string val, string option)
        {
            // We have to return something even though the value will
            // be ignored if an error is reported. The -1 value seems
            // like a safe bet in case it isn't ignored due to a bug.
            int result = -1;

            if (string.IsNullOrEmpty(val))
                ErrorMessages.Add("Missing required value for option '" + option + "'.");
            else
            {
#if NETCF   // NETCF: Create compatibility method for TryParse
                try
                {
                    result = int.Parse(val);
                }
                catch (Exception)
                {
                    ErrorMessages.Add("An int value was expected for option '{0}' but a value of '{1}' was used");
                }
#else
                int r;
                if (int.TryParse(val, out r))
                    result = r;
                else
                    ErrorMessages.Add("An int value was expected for option '{0}' but a value of '{1}' was used");
#endif
            }

            return result;
        }

        private string ExpandToFullPath(string path)
        {
            if (path == null) return null;

#if NETCF
            return Path.Combine(NUnit.Env.DocumentFolder, path);
#else
            return Path.GetFullPath(path);
#endif
        }

        private void ConfigureOptions()
        {
            // NOTE: The order in which patterns are added
            // determines the display order for the help.

            // Old Options no longer supported:
            //   fixture
            //   xmlConsole
            //   noshadow
            //   nothread
            //   nodots
           
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
                            using (var rdr = new StreamReader(fullTestListPath))
                            {
                                while (!rdr.EndOfStream)
                                {
                                    var line = rdr.ReadLine().Trim();

                                    if (line[0] != '#')
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

            this.Add("include=", "Test {CATEGORIES} to be included. May be a single category, a comma-separated list of categories or a category expression.",
                v => Include = RequiredValue(v, "--include"));

            this.Add("exclude=", "Test {CATEGORIES} to be excluded. May be a single category, a comma-separated list of categories or a category expression.",
                v => Exclude = RequiredValue(v, "--exclude"));

#if !NUNITLITE
            this.Add("config=", "{NAME} of a project configuration to load (e.g.: Debug).",
                v => ActiveConfig = RequiredValue(v, "--config"));

            // Where to Run Tests
            this.Add("process=", "{PROCESS} isolation for test assemblies.\nValues: Single, Separate, Multiple, Parallel. If not specified, defaults to Separate for a single assembly or Multiple for more than one.",
                v => ProcessModel = RequiredValue(v, "--process", "Single", "Separate", "Multiple", "Parallel"));

            this.Add("domain=", "{DOMAIN} isolation for test assemblies.\nValues: None, Single, Multiple. If not specified, defaults to Single for a single assembly or Multiple for more than one.",
                v => DomainUsage = RequiredValue(v, "--domain", "None", "Single", "Multiple"));

            // How to Run Tests
            this.Add("framework=", "{FRAMEWORK} type/version to use for tests.\nExamples: mono, net-3.5, v4.0, 2.0, mono-4.0. If not specified, tests will run under the framework they are compiled with.",
                v => Framework = RequiredValue(v, "--framework"));

            this.Add("x86", "Run tests in an x86 process on 64 bit systems",
                v => RunAsX86 = v != null);

            this.Add("dispose-runners", "Dispose each test runner after it has finished running its tests.",
                v => DisposeRunners = v != null);

            this.Add("shadowcopy", "Shadow copy test files",
                v => ShadowCopyFiles = v != null);
#endif

            this.Add("timeout=", "Set timeout for each test case in {MILLISECONDS}.",
                v => defaultTimeout = RequiredInt(v, "--timeout"));

            this.Add("seed=", "Set the random {SEED} used to generate test cases.",
                v => randomSeed = RequiredInt(v, "--seed"));

            this.Add("workers=", "Specify the {NUMBER} of worker threads to be used in running tests. If not specified, defaults to 2 or the number of processors, whichever is greater.",
                v => numWorkers = RequiredInt(v, "--workers"));

            this.Add("stoponerror", "Stop run immediately upon any test failure or error.",
                v => StopOnError = v != null);

            this.Add("wait", "Wait for input before closing console window.",
                v => WaitBeforeExit = v != null);

            this.Add("pause", "Pause before run to allow debugging.",
                v => PauseBeforeRun = v != null);

            // Output Control
            this.Add("work=", "{PATH} of the directory to use for output files. If not specified, defaults to the current directory.",
                v => workDirectory = RequiredValue(v, "--work"));

            this.Add("output|out=", "File {PATH} to contain text output from the tests.",
                v => OutFile = RequiredValue(v, "--output"));

            this.Add("err=", "File {PATH} to contain error output from the tests.",
                v => ErrFile = RequiredValue(v, "--err"));

            this.Add("full", "Prints full report of all test results.",
                v => Full = v != null);

            this.Add("result=", "An output {SPEC} for saving the test results.\nThis option may be repeated.",
                v => resultOutputSpecifications.Add(new OutputSpecification(RequiredValue(v, "--resultxml"))));

            this.Add("explore:", "Display or save test info rather than running tests. Optionally provide an output {SPEC} for saving the test info. This option may be repeated.", v =>
            {
                Explore = true;
                if (v != null)
                    ExploreOutputSpecifications.Add(new OutputSpecification(v));
            });

            this.Add("noresult", "Don't save any test results.",
                v => noresult = v != null);

            this.Add("labels=", "Specify whether to write test case names to the output. Values: Off, On, All",
                v => DisplayTestLabels = RequiredValue(v, "--labels", "Off", "On", "All"));

            this.Add("trace=", "Set internal trace {LEVEL}.\nValues: Off, Error, Warning, Info, Verbose (Debug)",
                v => InternalTraceLevel = RequiredValue(v, "--trace", "Off", "Error", "Warning", "Info", "Verbose", "Debug"));

#if !NETCF
            this.Add("teamcity", "Turns on use of TeamCity service messages.",
                v => TeamCity = v != null);
#endif

            this.Add("noheader|noh", "Suppress display of program information at start of run.",
                v => NoHeader = v != null);

            this.Add("nocolor|noc", "Displays console output without color.",
                v => NoColor = v != null);

            this.Add("verbose|v", "Display additional information as the test runs.",
                v => Verbose = v != null);

            this.Add("help|h", "Display this message and exit.",
                v => ShowHelp = v != null);

            // Default
            this.Add("<>", v =>
            {
                if (v.StartsWith("-") || v.StartsWith("/") && Path.DirectorySeparatorChar != '/')
                    ErrorMessages.Add("Invalid argument: " + v);
                else
                    InputFiles.Add(v);
            });
        }

        #endregion
    }
}
