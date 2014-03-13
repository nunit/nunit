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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Options;
using System.Diagnostics;

namespace NUnit.Framework.TestHarness
{
    /// <summary>
    /// The CommandLineOptions class parses and holds the _values of
    /// any options entered at the command line.
    /// </summary>
    public class CommandLineOptions : OptionSet
    {
        #region Constructor

        public CommandLineOptions(params string[] args)
        {
            // Select Tests
            this.Add("test=", "Comma-separated list of {NAMES} of tests to run or explore. This option may be repeated.",
                v => ((List<string>)Tests).AddRange(TestNameParser.Parse(RequiredValue(v, "--test"))));

            this.Add("include=", "Test {CATEGORIES} to be included. May be a single category, a comma-separated list of categories or a category expression.",
                v => Include = RequiredValue(v, "--include"));

            this.Add("exclude=", "Test {CATEGORIES} to be excluded. May be a single category, a comma-separated list of categories or a category expression.",
                v => Exclude = RequiredValue(v, "--exclude"));

            // Where to Run Tests
            this.Add("appdomain|a", "Run tests in a separate AppDomain",
                v => RunInSeparateAppDomain = v != null);
            //this.Add("domain=", "{DOMAIN} isolation for test assemblies.\nValues: None, Single, Multiple",
            //    v => DomainUsage = RequiredValue(v, "--domain", "None", "Single", "Multiple"));

            // How to Run Tests
            this.Add("wait", "Wait for input before closing console window.",
                v => WaitBeforeExit = v != null);

            this.Add("timeout=", "Set timeout for each test case in {MILLISECONDS}.",
                v => defaultTimeout = RequiredInt(v, "--timeout"));

            this.Add("workers=", "Specify the {NUMBER} of worker threads to use in running tests. (Default: use main thread)",
                v => NumWorkers = RequiredInt(v, "--workers"));

            this.Add("seed=", "Specify the random {SEED} to be used in generating test cases.",
                v => randomSeed = RequiredInt(v, "--seed"));

            this.Add("capture", "Turn on capture of standard and error output in order to test that it works. The text should be displayed on the console either way.",
                v => CaptureText = v != null );

            // Output Control
            this.Add("work=", "{PATH} of the directory to use for output files.",
                v => WorkDirectory = RequiredValue(v, "--work"));

            this.Add("output|out=", "File {PATH} to contain text output from the tests.",
                v => OutFile = RequiredValue(v, "--output"));

            this.Add("err=", "File {PATH} to contain error output from the tests.",
                v => ErrFile = RequiredValue(v, "--err"));

            this.Add("explore:", "Display or save test info rather than running tests. Optionally provide an output {PATH} for saving the test info. This option may be repeated.",
                v =>
                {
                    Explore = true;
                    if (v != null)
                        ExploreFile = v;
                });

            this.Add("xml3=", "An output {PATH} for saving the test results in nunit3 format. (Default: TestResult.v3.xml)",
                v => v3ResultFile = RequiredValue(v, "--xml3"));

            this.Add("xml2=", "An output {PATH} for saving the test results in nunit2 format. (Default: 'TestResult.v2.xml)",
                v => v2ResultFile = RequiredValue(v, "--xml2"));

            this.Add("labels|l=", "Specify whether to write test case names to the output. Values: Off, On, All",
                v => DisplayTestLabels = RequiredValue(v, "--labels", "Off", "On", "All"));

            this.Add("trace=", "Set internal trace {LEVEL}.\nValues: Off, Error, Warning, Info, Verbose (Debug)",
                v => InternalTraceLevel = RequiredValue(v, "--trace", "Off", "Error", "Warning", "Info", "Verbose", "Debug"));

            this.Add("noheader|noh", "Suppress display of program information at start of run.",
                v => NoHeader = v != null);

            this.Add("teamcity", "Running under TeamCity: display service messages as tests are executed.",
                v => DisplayTeamCityServiceMessages = v != null);

            this.Add("help|h", "Display this message and exit.",
                v => ShowHelp = v != null);

            this.Add("selftest", "Run the test-harness self-test.",
                v => inputFiles.Add("test-harness.exe"));

            // Default
            this.Add("<>", v =>
            {
                if (v.StartsWith("-") || v.StartsWith("/") && System.IO.Path.DirectorySeparatorChar != '/')
                    ErrorMessages.Add("Invalid argument: " + v);
                else
                    inputFiles.Add(v);
            });

            if (args != null)
                this.Parse(args);
        }

        #endregion

        #region Properties

        // Action to Perform

        public bool Explore { get; private set; }

        public bool ShowHelp { get; private set; }

        // Select Tests

        private List<string> inputFiles = new List<string>();
        public string AssemblyName { get { return inputFiles.Count > 0 ? inputFiles[0] : null; } }

        private List<string> testList = new List<string>();
        public List<string> Tests { get { return testList; } }

        public string Include { get; private set; }

        public string Exclude { get; private set; }

        // Where to Run Tests

        public bool RunInSeparateAppDomain { get; private set; }

        // How to Run Tests

        public bool WaitBeforeExit { get; private set; }

        //DriverSetting
        private int defaultTimeout = -1;
        public int DefaultTimeout { get { return defaultTimeout; } }

        //DriverSetting
        public int? NumWorkers { get; private set; }

        //DriverSetting
        private int randomSeed = -1;
        public int RandomSeed { get { return randomSeed; } }

        //DriverSetting
        public bool CaptureText { get; private set; }

        // Output Control

        public bool NoHeader { get; private set; }

        public string OutFile { get; private set; }

        public string ErrFile { get; private set; }

        //DriverSetting
        public string DisplayTestLabels { get; private set; }

        //DriverSetting
        public string InternalTraceLevel { get; private set; }

        //DriverSetting
        public string WorkDirectory { get; private set; }

        private string v3ResultFile = "TestResult.v3.xml";
        public string V3ResultFile { get { return v3ResultFile; } }

        private string v2ResultFile = "TestResult.v2.xml";
        public string V2ResultFile { get { return v2ResultFile; } }

        public string ExploreFile { get; private set; }

        //DriverSetting
        public bool DisplayTeamCityServiceMessages { get; private set; }

        // Error Processing

        private List<string> errorMessages = new List<string>();
        public IList<string> ErrorMessages { get { return errorMessages; } }

        #endregion

        #region Public Methods

        private bool validated;
        public bool Validate()
        {
            if (!validated && !ShowHelp)
            {
                // Perform checks that can only be made after
                // all options are processed.
                if (inputFiles.Count == 0)
                    ErrorMessages.Add("Error: No assembly was specified");
                else if (inputFiles.Count > 1)
                    ErrorMessages.Add("Error: Only one assembly may be specified");

                if (Explore)
                {
                    // Check for options that don't make sense with explore
                }

                validated = true;
            }

            return ErrorMessages.Count == 0;
        }

        public IDictionary<string, object> CreateDriverSettings()
        {
            var settings = new Dictionary<string, object>();

            if (DefaultTimeout >= 0)
                settings["DefaultTimeout"] = DefaultTimeout;
            if (NumWorkers.HasValue)
                settings["NumberOfTestWorkers"] = NumWorkers.Value;
            if (RandomSeed >= 0)
                settings["RandomSeed"] = RandomSeed;
            if (CaptureText)
                settings["CaptureStandardOutput"] = settings["CaptureStandardError"] = true;

            settings["DisplayTestLabels"] = DisplayTestLabels;
            if (InternalTraceLevel != null)
                settings["InternalTraceLevel"] = InternalTraceLevel;
            if (WorkDirectory != null)
                settings["WorkDirectory"] = WorkDirectory;
            settings["DisplayTeamCityServiceMessages"] = DisplayTeamCityServiceMessages;

            return settings;
        }

        #endregion

        #region Helper Methods

        private string RequiredValue(string val, string option, params string[] validValues)
        {
            if (val == null || val == string.Empty)
                ErrorMessages.Add("Missing required value for option '" + option + "'.");

            bool isValid = true;

            if (validValues != null && validValues.Length > 0)
            {
                isValid = false;

                foreach (string valid in validValues)
                    if (string.Compare(valid, val, true) == 0)
                        isValid = true;

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
