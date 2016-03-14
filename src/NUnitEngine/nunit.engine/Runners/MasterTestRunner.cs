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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Common;
using NUnit.Engine.Internal;
using NUnit.Engine.Services;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : AbstractTestRunner, ITestRunner
    {
        private ITestEngineRunner _realRunner;
        private IRuntimeFrameworkService _runtimeService;
        private ExtensionService _extensionService;

        private TestEventDispatcher _eventDispatcher;

        public MasterTestRunner(IServiceLocator services, TestPackage package)
            : base(services, package)
        {
            _runtimeService = Services.GetService<IRuntimeFrameworkService>();
            _extensionService = Services.GetService<ExtensionService>();
        }

        #region Properties

        public bool IsTestRunning { get; private set; }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            return _realRunner.Explore(filter).Aggregate(TEST_RUN_ELEMENT, TestPackage.Name, TestPackage.FullName);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            // Last chance to catch invalid settings in package,
            // in case the client runner missed them.
            ValidatePackageSettings();

            // Some files in the top level package may be projects.
            // Expand them so that they contain subprojects for
            // each contained assembly.
            ExpandProjects();

            // Use SelectRuntimeFramework for its side effects.
            // Info will be left behind in the package about
            // each contained assembly, which will subsequently
            // be used to determine how to run the assembly.
            _runtimeService.SelectRuntimeFramework(TestPackage);

            if (TestPackage.GetSetting(PackageSettings.ProcessModel, "") == "InProcess" &&
                TestPackage.GetSetting(PackageSettings.RunAsX86, false))
            {
                throw new NUnitEngineException("Cannot run tests in process - a 32 bit process is required.");
            }

            _realRunner = TestRunnerFactory.MakeTestRunner(TestPackage);

            return _realRunner.Load().Aggregate(TEST_RUN_ELEMENT, TestPackage.Name, TestPackage.FullName);
        }

        private void ExpandProjects()
        {
            foreach (var package in TestPackage.SubPackages)
            {
                string packageName = package.FullName;

                if (File.Exists(packageName) && ProjectService.CanLoadFrom(packageName))
                        ProjectService.ExpandProjectPackage(package);
            }
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public override void UnloadPackage()
        {
            if (_realRunner != null)
                _realRunner.Unload();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        protected override int CountTests(TestFilter filter)
        {
            return _realRunner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in the loaded TestPackage and return a test result. The tests
        /// are run synchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            var eventDispatcher = new TestEventDispatcher();
            if (listener != null)
                eventDispatcher.Listeners.Add(listener);
            foreach (var extension in _extensionService.GetExtensions<ITestEventListener>())
                eventDispatcher.Listeners.Add(extension);

            IsTestRunning = true;

            eventDispatcher.OnTestEvent(string.Format("<start-run count='{0}'/>", CountTestCases(filter)));

            DateTime startTime = DateTime.UtcNow;
            long startTicks = Stopwatch.GetTimestamp();

            TestEngineResult result = _realRunner.Run(eventDispatcher, filter).Aggregate("test-run", TestPackage.Name, TestPackage.FullName);

            // These are inserted in reverse order, since each is added as the first child.
            InsertFilterElement(result.Xml, filter);
            InsertCommandLineElement(result.Xml);

            result.Xml.AddAttribute("engine-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            result.Xml.AddAttribute("clr-version", Environment.Version.ToString());

            double duration = (double)(Stopwatch.GetTimestamp() - startTicks) / Stopwatch.Frequency;
            result.Xml.AddAttribute("start-time", XmlConvert.ToString(startTime, "u"));
            result.Xml.AddAttribute("end-time", XmlConvert.ToString(DateTime.UtcNow, "u"));
            result.Xml.AddAttribute("duration", duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            IsTestRunning = false;

            eventDispatcher.OnTestEvent(result.Xml.OuterXml);

            return result;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public override void StopRun(bool force)
        {
            _realRunner.StopRun(force);
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                base.Dispose(disposing);

                if (disposing && _realRunner != null)
                    _realRunner.Dispose();
            }
        }

        #endregion

        #region ITestRunner Explicit Implementation

        // NOTE: Only those methods which differ from those in
        // ITestEngineRunner have an explicit implementation. 
        // Methods that are the same for both interfaces
        // use the class methods.

        /// <summary>
        /// Load a TestPackage for possible execution. The 
        /// explicit implementation returns an ITestEngineResult
        /// for consumption by clients.
        /// </summary>
        /// <returns>An XmlNode representing the loaded assembly.</returns>
        XmlNode ITestRunner.Load()
        {
            return Load().Xml;
        }

        /// <summary>
        /// Reload the currently loaded test jpackage.
        /// </summary>
        /// <returns>An XmlNode representing the loaded package</returns>
        /// <exception cref="InvalidOperationException">If no package has been loaded</exception>
        XmlNode ITestRunner.Reload()
        {
            return Reload().Xml;
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage. The explicit
        /// implementation returns an ITestEngineResult for use
        /// by external clients.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An XmlNode giving the result of the test execution</returns>
        XmlNode ITestRunner.Run(ITestEventListener listener, TestFilter filter)
        {
            return Run(listener, filter).Xml;
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">The listener that is notified as the run progresses</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns></returns>
        ITestRun ITestRunner.RunAsync(ITestEventListener listener, TestFilter filter)
        {
            return RunAsync(listener, filter);
        }

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An XmlNode representing the tests found.</returns>
        XmlNode ITestRunner.Explore(TestFilter filter)
        {
            return this.Explore(filter).Xml;
        }

        #endregion

        #region Helper Methods

        // Any Errors thrown from this method indicate that the client
        // runner is putting invalid values into the package.
        private void ValidatePackageSettings()
        {
            var frameworkSetting = TestPackage.GetSetting(PackageSettings.RuntimeFramework, "");
            if (frameworkSetting.Length > 0)
            {
                // Check requested framework is actually available
                var runtimeService = Services.GetService<IRuntimeFrameworkService>();
                if (!runtimeService.IsAvailable(frameworkSetting))
                    throw new NUnitEngineException(string.Format("The requested framework {0} is unknown or not available.", frameworkSetting));

                // If running in process, check requested framework is compatible
                var processModel = TestPackage.GetSetting(PackageSettings.ProcessModel, "Default");
                if (processModel.ToLower() == "single")
                {
                    var currentFramework = RuntimeFramework.CurrentFramework;
                    var requestedFramework = RuntimeFramework.Parse(frameworkSetting);
                    if (!currentFramework.Supports(requestedFramework))
                        throw new NUnitEngineException(string.Format(
                            "Cannot run {0} framework in process already running {1}.", frameworkSetting, currentFramework));
                }
            }
        }

        private static void InsertCommandLineElement(XmlNode resultNode)
        {
            var doc = resultNode.OwnerDocument;

            XmlNode cmd = doc.CreateElement("command-line");
            resultNode.InsertAfter(cmd, null);

            var cdata = doc.CreateCDataSection(Environment.CommandLine);
            cmd.AppendChild(cdata);
        }

        private static void InsertSettingsElement(XmlNode resultNode, IDictionary<string, object> settings)
        {
            var doc = resultNode.OwnerDocument;

            XmlNode settingsNode = doc.CreateElement("settings");
            resultNode.InsertAfter(settingsNode, null);

            foreach (string name in settings.Keys)
            {
                string value = settings[name].ToString();
                XmlNode settingNode = doc.CreateElement("setting");
                settingNode.AddAttribute("name", name);
                settingNode.AddAttribute("value", value);
                settingsNode.AppendChild(settingNode);
            }
        }

        private static void InsertFilterElement(XmlNode resultNode, TestFilter filter)
        {
            // Convert the filter to an XmlNode
            var tempNode = XmlHelper.CreateXmlNode(filter.Text);

            // Don't include it if it's an empty filter
            if (tempNode.ChildNodes.Count > 0)
            {
                var doc = resultNode.OwnerDocument;
                var filterElement = doc.ImportNode(tempNode, true);
                resultNode.InsertAfter(filterElement, null);
            }
        }

        #endregion
    }
}
