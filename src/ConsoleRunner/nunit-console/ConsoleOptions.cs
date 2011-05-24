// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

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
        public string xmlPath;
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
        private bool validated;

        public ConsoleOptions(params string[] args)
        {
            // NOTE: The order in which patterns are added 
            // determines the display order for the help.

            // fixture
            this.Add("run=", "Names of the tests to run",
                v => runList.Add(v));
            this.Add("config=", "Project configuration (e.g.: Debug) to load",
                v => activeConfig = v);
            this.Add("xml=", "Name of XML output file (Default: TestResult.xml)",
                v => xmlPath = v);
            // xmlConsole
            this.Add("noxml", "Suppress XML output", 
                v => noxml = v != null);
            this.Add("output|out=", "File to receive test output",
                v => outputPath = v);
            this.Add("err=", "File to receive test error output", 
                v => errorPath = v);
            this.Add("work=", "Work directory for output files", 
                v => workDir = v);
            this.Add("labels", "Label each test by name in the output", 
                v => labels = v != null);
            this.Add("trace=", "Set internal trace level (NYI)\nValues: Off, Error, Warning, Info, Verbose",
                (InternalTraceLevel v) => internalTraceLevel = v);
            this.Add("include=", "Comma-separated list of categories to include", 
                v => include = v);
            this.Add("exclude=", "Comma-separated list of categories to exclude", 
                v => exclude = v);
            this.Add("framework=", "Framework version to be used for tests",
                v => framework = v);
            this.Add("process=", "Process model for tests\nValues: Single, Separate, Multiple",
                (ProcessModel v) => processModel = v);
            this.Add("domain=", "AppDomain usage for tests\nValues: None, Single, Multiple",
                (DomainUsage v) => domainUsage = v);
            // noshadow
            // nothread
            this.Add("timeout=", "Set timeout for each test case in milliseconds",
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

        public IList<string> ErrorMessages
        {
            get { return errorMessages; }
        }

        #endregion

        #region Public Methods

        public bool Validate()
        {
            if (!validated)
            {
                if (activeConfig == "")
                    RequiredValueError("--config");
                if (xmlPath == "") 
                    RequiredValueError("--xml");
                if (outputPath == "")
                    RequiredValueError("--output");
                if (errorPath == "")
                    RequiredValueError("--err");
                if (include == "")
                    RequiredValueError("--include");
                if (exclude == "")
                    RequiredValueError("--exclude");
                if (workDir == "")
                    RequiredValueError("--work");
                //if (timeout == "")
                //    RequiredValueError("--timeout");
                //else if (timeout != null && !int.TryParse(timeout, out Timeout))
                //    RequiredIntError("--timeout");
                //if (trace == "")
                //    RequiredValueError("--trace");

                validated = true;
            }

            return errorMessages.Count == 0;
        }

        #endregion

        #region Helper Methods

        private void RequiredValueError(string option)
        {
            errorMessages.Add("Missing required value for option '" + option + "'.");
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