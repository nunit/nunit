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
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using NUnit.Engine;

namespace NUnit.ConsoleRunner
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

        public ConsoleOptions(params string[] args)
        {
            // NOTE: The order in which patterns are added 
            // determines the display order for the help.

            // Old Options no longer supported:
            //   fixture
            //   xmlConsole
            //   noshadow
            //   nothread
            //   nodots

            this.Add("test=", "(NYI)Comma-separated list of {NAMES} of tests to run or explore. This option may be repeated.",
                v => testList.AddRange(TestNameParser.Parse(RequiredValue(v, "--test"))));

            this.Add("include=", "(NYI) Comma-separated list of test {CATEGORIES} to be included.",
                v => include = RequiredValue(v, "--include"));

            this.Add("exclude=", "(NYI) Comma-separated list of test {CATEGORIES} to be excluded.",
                v => exclude = RequiredValue(v, "--exclude"));

            this.Add("config=", "{NAME} of a project configuration to load (e.g.: Debug).", 
                v => activeConfig = RequiredValue(v, "--config"));

            this.Add("work=", "{PATH} of the directory to use for output files.",
                v => workDir = RequiredValue(v, "--work"));

            this.Add("output|out=", "File {PATH} to contain text output from the tests.",
                v => outputPath = RequiredValue(v, "--output"));

            this.Add("err=", "File {PATH} to contain error output from the tests.",
                v => errorPath = RequiredValue(v, "--err"));

            this.Add("result=", "An output {SPEC} for saving the test results.\nThis option may be repeated.", 
                v => resultOutputSpecifications.Add(new OutputSpecification(RequiredValue(v, "--resultxml"))));

            this.Add("explore:", "Display or save test info rather than running tests. Optionally provide an output {SPEC} for saving the test info. This option may be repeated.", v => 
                {
                    explore = true;
                    if (v != null)
                        exploreOutputSpecifications.Add(new OutputSpecification(v));
                });

            this.Add("noresult", "Don't save any test results.", 
                v => noresult = v != null);

            this.Add("labels=", "Specify whether to write test case names to the output. Values: Off, On, All", 
                v => labels = RequiredValue(v, "--labels", "Off", "On", "All"));

            this.Add("trace=", "Set internal trace {LEVEL}.\nValues: Off, Error, Warning, Info, Verbose (Debug)",
                v => internalTraceLevel = RequiredValue(v, "--trace", "Off", "Error", "Warning", "Info", "Verbose", "Debug"));

            this.Add("framework=", "{FRAMEWORK} type/version to use for tests.\nExamples: mono, net-3.5, v4.0, 2.0, mono-4.0",
                v => framework = v);

            this.Add("process=", "{PROCESS} isolation for test assemblies.\nValues: Single, Separate, Multiple",
                v => processModel = RequiredValue(v, "--process", "Single", "Separate", "Multiple"));

            this.Add("domain=", "{DOMAIN} isolation for test assemblies.\nValues: None, Single, Multiple",
                v => domainUsage = RequiredValue(v, "--domain", "None", "Single", "Multiple"));

            this.Add("timeout=", "Set timeout for each test case in {MILLISECONDS}.",
                v => defaultTimeout = RequiredInt(v, "--timeout"));
            
            this.Add("wait", "Wait for input before closing console window.", 
                v => wait = v != null);

            this.Add("noheader|noh", "Suppress display of program information at start of run.",
                v => noheader = v != null);

            this.Add("help|h", "Display this message and exit.", 
                v => help = v != null);

            // Default
            this.Add("<>", v =>
            {
                if (v.StartsWith("-") || v.StartsWith("/") && Path.DirectorySeparatorChar != '/')
                    errorMessages.Add("Invalid argument: " + v);
                else
                    inputFiles.Add(v);
            });

            if (args != null)
                this.Parse(args);
        }

        #endregion

        #region Properties

        private string activeConfig;
        public string ActiveConfig
        {
            get { return activeConfig; }
        }

        private string outputPath;
        public string OutputPath
        {
            get { return outputPath; }
        }

        private string errorPath;
        public string ErrorPath
        {
            get { return errorPath; }
        }

        private string workDir;
        public string WorkDirectory
        {
            get { return workDir == null ? Environment.CurrentDirectory : workDir; }
        }

        private string include;
        public string Include
        {
            get { return include; }
        }

        private string exclude;
        public string Exclude
        {
            get { return exclude; }
        }

        private string framework;
        public string Framework
        {
            get { return framework; }
        }

        private string processModel;
        public string ProcessModel
        {
            get { return processModel; }
        }

        private string domainUsage;
        public string DomainUsage
        {
            get { return domainUsage; }
        }

        private string internalTraceLevel;
        public string InternalTraceLevel
        {
            get { return internalTraceLevel; }
        }

        private int defaultTimeout = -1;
        public int DefaultTimeout
        {
            get { return defaultTimeout; }
        }   

        private string labels;
        public string Labels
        {
            get { return labels; }
        }

        private bool explore;
        public bool Explore
        {
            get { return explore; }
        }

        private bool wait;
        public bool Wait
        {
            get { return wait; }
        }

        private bool noheader;
        public bool NoHeader
        {
            get { return noheader; }
        }

        private bool help;
        public bool ShowHelp
        {
            get { return help; }
        }

        private List<string> inputFiles = new List<string>();
        public string[] InputFiles
        {
            get { return inputFiles.ToArray(); }
        }

        private List<string> testList = new List<string>();
        public string[] TestList
        {
            get { return testList.ToArray(); }
        }

        private List<string> errorMessages = new List<string>();
        public IList<string> ErrorMessages
        {
            get { return errorMessages; }
        }

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
        public IList<OutputSpecification> ExploreOutputSpecifications
        {
            get { return exploreOutputSpecifications; }
        }

        #endregion

        #region Public Methods

        public bool Validate()
        {
            if (!validated)
            {
                // Additional Checks here

                validated = true;
            }

            return errorMessages.Count == 0;
        }

        #endregion

        #region Helper Methods

        private string RequiredValue(string val, string option, params string[] validValues)
        {
            if (val == null || val == string.Empty)
                errorMessages.Add("Missing required value for option '" + option + "'.");

            bool isValid = true;

            if (validValues != null && validValues.Length > 0)
            {
                isValid = false;

                foreach (string valid in validValues)
                    if (string.Compare(valid, val, true) == 0)
                        isValid = true;

            }

            if (!isValid)
                errorMessages.Add(string.Format("The value '{0}' is not valid for option '{1}'.", val, option));

            return val;
        }

        private int RequiredInt(string val, string option)
        {
            // We have to return something even though the value will 
            // be ignored if an error is reported. The -1 value seems
            // like a safe bet in case it isn't ignored due to a bug.
            int result = -1;

            if (val == null || val == string.Empty)
                errorMessages.Add("Missing required value for option '" + option + "'.");
            else 
            {
                int r;
                if (int.TryParse(val, out r))
                    result = r;
                else
                    errorMessages.Add("An int value was exprected for option '{0}' but a value of '{1}' was used");
            }
                
            return result;
        }

        private void RequiredIntError(string option)
        {
            errorMessages.Add("An int value is required for option '" + option + "'.");
        }

        private void ProcessIntOption(string v, ref int field)
        {
            if (!int.TryParse(v, out field))
                errorMessages.Add("Invalid argument value: " + v);
        }

        private void ProcessEnumOption<T>(string v, ref T field)
        {
            if (Enum.IsDefined(typeof(T), v))
                field = (T)Enum.Parse(typeof(T), v);
            else
                errorMessages.Add("Invalid argument value: " + v);
        }

        private object ParseEnumOption(Type enumType, string value)
        {
            foreach (string name in Enum.GetNames(enumType))
                if (value.ToLower() == name.ToLower())
                    return Enum.Parse(enumType, value);

            this.errorMessages.Add(value);

            return null;
        }

        #endregion
    }
}