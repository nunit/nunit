// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.Text;
using System.Collections;

namespace NUnitLite.Runner
{
    public class CommandLineOptions
    {
        private string optionChars;

        bool error = false;

        ArrayList invalidOptions = new ArrayList();
        ArrayList parameters = new ArrayList();

        private bool wait = false;
        public bool Wait
        {
            get { return wait; }
        }

        private bool nologo = false;
        public bool Nologo
        {
            get { return nologo; }
        }

        private bool listprops = false;
        public bool ListProperties
        {
            get { return listprops; }
        }

        private bool verbose = false;
        public bool Verbose
        {
            get { return verbose; }
        }

        private bool dumpTree = false;
        public bool DumpTree
        {
            get { return dumpTree; }
        }

        private bool help = false;
        public bool Help
        {
            get { return help; }
        }

        private ArrayList tests = new ArrayList();
        public string[] Tests
        {
            get { return (string[])tests.ToArray(typeof(string)); }
        }

        public int TestCount
        {
            get { return tests.Count; }
        }

        public CommandLineOptions()
        {
            this.optionChars = System.IO.Path.DirectorySeparatorChar == '/' ? "-" : "/-";
        }

        public CommandLineOptions(string optionChars)
        {
            this.optionChars = optionChars;
        }

        public void Parse(params string[] args)
        {
            foreach( string arg in args )
            {
                if (optionChars.IndexOf(arg[0]) >= 0 )
                    ProcessOption(arg);
                else
                    ProcessParameter(arg);
            }
        }

        public string[] Parameters
        {
            get { return (string[])parameters.ToArray( typeof(string) ); }
        }

        private void ProcessOption(string opt)
        {
            int pos = opt.IndexOfAny( new char[] { ':', '=' } );
            string val = string.Empty;

            if (pos >= 0)
            {
                val = opt.Substring(pos + 1);
                opt = opt.Substring(0, pos);
            }

            switch (opt.Substring(1))
            {
                case "wait":
                    wait = true;
                    break;
                case "nologo":
                    nologo = true;
                    break;
                case "help":
                    help = true;
                    break;
                case "props":
                    listprops = true;
                    break;
                case "test":
                    tests.Add(val);
                    break;
                case "verbose":
                    verbose = true;
                    break;
                case "tree":
                    dumpTree = true;
                    break;
                default:
                    error = true;
                    invalidOptions.Add(opt);
                    break;
            }
        }

        private void ProcessParameter(string param)
        {
            parameters.Add(param);
        }

        public bool Error
        {
            get { return error; }
        }

        public string ErrorMessage
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                foreach (string opt in invalidOptions)
                    sb.Append( "Invalid option: " + opt + Env.NewLine );
                return sb.ToString();
            }
        }

        public string HelpText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                string NL = Env.NewLine;

#if PocketPC || WindowsCE || NETCF
                string name = "NUnitLite";
#else
                string name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
#endif

                sb.Append( NL + name + " [assemblies] [options]" + NL + NL );
                sb.Append(" Runs a set of NUnitLite tests from the console." + NL + NL );
                sb.Append("You may specify one or more test assemblies by name, without a path or" + NL);
                sb.Append("extension. They must be in the same in the same directory as the exe + NL");
                sb.Append("or on the probing path. If no assemblies are provided, tests in the + NL");
                sb.Append("executing assembly itself are run. + NL + NL");
                sb.Append("Options: + NL");
                sb.Append("  -test:testname  Provides the name of a test to run. This option may be + NL");
                sb.Append("                  repeated. If no test names are given, all tests are run. + NL + NL");
                sb.Append("  -help           Displays this help + NL + NL");
                sb.Append("  -nologo         Suppresses display of the initial message + NL + NL");
                sb.Append("  -wait           Waits for a key press before exiting + NL + NL");
                if (System.IO.Path.DirectorySeparatorChar != '/')
                    sb.Append("On Windows, options may be prefixed by a '/' character if desired + NL + NL");
                sb.Append("Options that take values may use an equal sign or a colon + NL");
                sb.Append("to separate the option from its value. + NL + NL");

                return sb.ToString();
            }
        }
    }
}
