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
using NUnit.Engine;

namespace NUnit.ConsoleRunner
{
    /// <summary>
    /// ConsoleOptions encapsulates the option settings for
    /// the nunit-console program. It inherits from the Mono
    /// Options OptionSet class and provides a central location
    /// for defining and parsing options.
    /// </summary>
	public class ConsoleOptions : NDesk.Options.OptionSet
	{
        public string activeConfig;
        public bool noxml;
        public string outputPath;
        public string errorPath;
        public string workDir;
        public bool labels;
        public InternalTraceLevel internalTraceLevel;
        public string include;
        public string exclude;
        public string framework;
        public ProcessModel processModel;
        public DomainUsage domainUsage;
        public int defaultTimeout = -1;
        public bool wait;
        public bool noheader;
        public bool help;

        private List<string> inputFiles = new List<string>();
        private List<string> runList = new List<string>();
        private List<string> errorMessages = new List<string>();
        private List<XmlOutputSpecification> xmlOutputSpecifications = new List<XmlOutputSpecification>();

        private bool validated;

        public ConsoleOptions(params string[] args)
        {
            // NOTE: The order in which patterns are added 
            // determines the display order for the help.

            // fixture

            this.Add("run=", "(NYI)Name of a tests to run. This option may be repeated.",
                v => runList.Add(RequiredValue(v, "--run")));

            this.Add("config=", "Project configuration (e.g.: Debug) to load", 
                v => activeConfig = RequiredValue(v, "--config"));

            this.Add("xml=", "Name and optional format of an XML output file.\nFormats:\n  --xml:filepath\n  --xml:filepath;format=[nunit3|nunit2]\n  --xml:filepath;transform=xsltfile\nThe default format is nunit3. If no option is specified, --xml:TestResult.xml is assumed.\nThis option may be repeated.", 
                v => xmlOutputSpecifications.Add(new XmlOutputSpecification(RequiredValue(v, "--xml"))));

            // xmlConsole

            this.Add("noxml", "Suppress all XML output, ignoring any --xml options", 
                v => noxml = v != null);

            this.Add("output|out=", "File to receive output sent to stdout by the test",
                v => outputPath = RequiredValue(v, "--output"));

            this.Add("err=", "File to receive output sent to stderr by the test", 
                v => errorPath = RequiredValue(v, "--err"));

            this.Add("work=", "Work directory for output files", 
                v => workDir = RequiredValue(v, "--work"));

            this.Add("labels", "(NYI) Label each test by name in the output", 
                v => labels = v != null);

            this.Add("trace=", "(NYI) Set internal trace level\nValues: Off, Error, Warning, Info, Verbose",
                (InternalTraceLevel v) => internalTraceLevel = v);

            this.Add("include=", "(NYI) Comma-separated list of categories to include", 
                v => include = RequiredValue(v, "--include"));

            this.Add("exclude=", "(NYI) Comma-separated list of categories to exclude", 
                v => exclude = RequiredValue(v, "--exclude"));

            this.Add("framework=", "(NYI) Framework version to be used for tests",
                v => framework = v);

            this.Add("process=", "Process model for tests\nValues: Single, Separate, Multiple",
                (ProcessModel v) => processModel = v);

            this.Add("domain=", "AppDomain usage for tests\nValues: None, Single, Multiple",
                (DomainUsage v) => domainUsage = v);

            // noshadow
            // nothread

            this.Add("timeout=", "(NYI) Set timeout for each test case in milliseconds",
                (int v) => defaultTimeout = v);
            
            this.Add("wait", "Wait for input before closing console window", 
                v => wait = v != null);

            this.Add("noheader|noh", "Suppress display of program information at start of run.",
                v => noheader = v != null);

            // nodots

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

        //public ConsoleOptions( bool allowForwardSlash, params string[] args )
        //{
        //}

        #region Properties

        public string WorkDirectory
        {
            get { return workDir == null ? Environment.CurrentDirectory : workDir; }
        }

        public string[] InputFiles
        {
            get { return inputFiles.ToArray(); }
        }

        public string[] RunList
        {
            get { return runList.ToArray(); }
        }

        public IList<string> ErrorMessages
        {
            get { return errorMessages; }
        }

        public IList<XmlOutputSpecification> XmlOutputSpecifications
        {
            get 
            {
                if (xmlOutputSpecifications.Count == 0)
                    xmlOutputSpecifications.Add(new XmlOutputSpecification("TestResult.xml"));

                return xmlOutputSpecifications; 
            }
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

        private string RequiredValue(string val, string option)
        {
            if (val == null || val == string.Empty)
                errorMessages.Add("Missing required value for option '" + option + "'.");
            return val;
        }

        //private void RequiredValueError(string option)
        //{
        //    if (v == null || v == string.Empty)
        //        errorMessages.Add("Missing required value for option '" + option + "'.");
        //    return v;
        //}

        //private void RequiredValueError(string option)
        //{
        //    errorMessages.Add("Missing required value for option '" + option + "'.");
        //}

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