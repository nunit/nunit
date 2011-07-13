// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using NUnit.Engine;

namespace NUnit.ConsoleRunner
{
	/// <summary>
	/// TestEventHandler processes events from the running
    /// test for the console runner.
	/// </summary>
	public class TestEventHandler : MarshalByRefObject, ITestEventHandler
	{
        private int testRunCount;
        private int testIgnoreCount;
        private int failureCount;
        private int level;

		private ConsoleOptions options;
		private TextWriter outWriter;
		private TextWriter errorWriter;

        StringCollection messages;
		
        //private bool progress = false;
		private string currentTestName;

		private ArrayList unhandledExceptions = new ArrayList();

		public TestEventHandler( ConsoleOptions options, TextWriter outWriter, TextWriter errorWriter )
		{
            level = 0;
			this.options = options;
			this.outWriter = outWriter;
			this.errorWriter = errorWriter;
			this.currentTestName = string.Empty;

            //AppDomain.CurrentDomain.UnhandledException += 
            //    new UnhandledExceptionEventHandler(OnUnhandledException);
		}

        //public bool HasExceptions
        //{
        //    get { return unhandledExceptions.Count > 0; }
        //}

        //public void WriteExceptions()
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Unhandled exceptions:");
        //    int index = 1;
        //    foreach( string msg in unhandledExceptions )
        //        Console.WriteLine( "{0}) {1}", index++, msg );
        //}

        #region ITestEventHandler Members

        public void OnTestEvent(string report)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(report);
            XmlNode testEvent = doc.FirstChild;

            switch (testEvent.Name)
            {
                case "start":
                    XmlAttribute typeAttr = testEvent.Attributes["type"];
                    if (typeAttr != null)
                        if (typeAttr.Value == "test-case")
                            TestStarted(testEvent);
                        else
                            SuiteStarted(testEvent);
                    break;

                case "test-case":
                    TestFinished(testEvent);
                    break;

                case "test-suite":
                    SuiteFinished(testEvent);
                    break;

                case "output":
                    TestOutput(testEvent);
                    break;
            }

        }

        #endregion

        #region Individual Handlers

        private void TestStarted(XmlNode startNode)
        {
            if (options.labels)
            {
                XmlAttribute nameAttr = startNode.Attributes["fullname"];
                if (nameAttr != null)
                    outWriter.WriteLine("***** {0}", nameAttr.Value);
            }
        }

        public void SuiteStarted(XmlNode startNode)
        {
            if (level++ == 0)
            {
                messages = new StringCollection();
                testRunCount = 0;
                testIgnoreCount = 0;
                failureCount = 0;

                XmlAttribute nameAttr = startNode.Attributes["fullname"];
                string suiteName = (nameAttr != null)
                    ? nameAttr.Value
                    : "<anonymous>";

                Trace.WriteLine("################################ UNIT TESTS ################################");
                Trace.WriteLine("Running tests in '" + suiteName + "'...");
            }
        }

        public void TestFinished(XmlNode testResult)
        {
            XmlAttribute resultAttr = testResult.Attributes["result"];
            string resultState = resultAttr == null
                ? "Unknown"
                : resultAttr.Value;

            switch (resultState)
            {
                case "Failed":
                    testRunCount++;
                    failureCount++;

                    XmlAttribute nameAttr = testResult.Attributes["fullname"];
                    
                    messages.Add(string.Format("{0}) {1} :", failureCount, nameAttr.Value));
                    //messages.Add(testResult.Message.Trim(Environment.NewLine.ToCharArray()));

                    //string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
                    //if (stackTrace != null && stackTrace != string.Empty)
                    //{
                    //    string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
                    //    foreach (string s in trace)
                    //    {
                    //        if (s != string.Empty)
                    //        {
                    //            string link = Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
                    //            messages.Add(string.Format("at\n{0}", link));
                    //        }
                    //    }
                    //}
                    break;

                case "Inconclusive":
                case "Passed":
                    testRunCount++;
                    break;

                case "Skipped":
                    testIgnoreCount++;
                    break;
            }

            //currentTestName = string.Empty;
        }

        public void SuiteFinished(XmlNode suiteResult)
        {
            if (--level == 0)
            {
                XmlAttribute timeAttr = suiteResult.Attributes["time"];

                Trace.WriteLine("############################################################################");

                if (messages.Count == 0)
                {
                    Trace.WriteLine("##############                 S U C C E S S               #################");
                }
                else
                {
                    Trace.WriteLine("##############                F A I L U R E S              #################");

                    foreach (string s in messages)
                    {
                        Trace.WriteLine(s);
                    }
                }

                Trace.WriteLine("############################################################################");
                Trace.WriteLine("Executed tests       : " + testRunCount);
                Trace.WriteLine("Ignored tests        : " + testIgnoreCount);
                Trace.WriteLine("Failed tests         : " + failureCount);
                Trace.WriteLine("Unhandled exceptions : " + unhandledExceptions.Count);
                if ( timeAttr != null )
                Trace.WriteLine("Total time           : " + timeAttr.Value + " seconds");
                Trace.WriteLine("############################################################################");
            }
        }

        private void TestOutput(XmlNode outputNode)
        {
            XmlAttribute typeAttr = outputNode.Attributes["type"];
            string type = typeAttr == null
                ? "output"
                : typeAttr.Value;

            XmlNode textNode = outputNode.SelectSingleNode("text");
            string text = textNode == null
                ? string.Empty
                : textNode.InnerText;

            switch (type)
            {
                default:
                case "out":
                    outWriter.Write(text);
                    break;
                case "error":
                    errorWriter.Write(text);
                    break;
            }
        }

        #endregion

        //private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    if (e.ExceptionObject.GetType() != typeof(System.Threading.ThreadAbortException))
        //    {
        //        this.UnhandledException((Exception)e.ExceptionObject);
        //    }
        //}


        //public void UnhandledException( Exception exception )
        //{
        //    // If we do labels, we already have a newline
        //    unhandledExceptions.Add(currentTestName + " : " + exception.ToString());
        //    //if (!options.labels) outWriter.WriteLine();
        //    string msg = string.Format("##### Unhandled Exception while running {0}", currentTestName);
        //    //outWriter.WriteLine(msg);
        //    //outWriter.WriteLine(exception.ToString());

        //    Trace.WriteLine(msg);
        //    Trace.WriteLine(exception.ToString());
        //}


		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
