// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Listeners
{
    // Note: Setting mimimum engine version in this case is
    // purely documentary since engines prior to 3.4 do not
    // check the EngineVersion property and will try to
    // load this extension anyway.
    [Extension(Enabled = false, EngineVersion = "3.4")]
    public class TeamCityEventListener : ITestEventListener
    {
        private readonly TextWriter _outWriter;
        private readonly Dictionary<string, string> _refs = new Dictionary<string, string>();
        private int _blockCounter;
        private string _rootFlowId;

        public TeamCityEventListener() : this(Console.Out) { }

        public TeamCityEventListener(TextWriter outWriter)
        {
            if (outWriter == null)
            {
                throw new ArgumentNullException("outWriter");
            }

            _outWriter = outWriter;
        }

        #region ITestEventListener Implementation

        public void OnTestEvent(string report)
        {
            var doc = new XmlDocument();
            doc.LoadXml(report);

            var testEvent = doc.FirstChild;
            RegisterMessage(testEvent);
        }

        #endregion

        public void RegisterMessage(XmlNode testEvent)
        {
            if (testEvent == null)
            {
                throw new ArgumentNullException("message");
            }

            var messageName = testEvent.Name;
            if (string.IsNullOrEmpty(messageName))
            {
                return;
            }

            messageName = messageName.ToLowerInvariant();
            if (messageName == "start-run")
            {
                _refs.Clear();
                return;
            }

            var fullName = testEvent.GetAttribute("fullname");
            if (string.IsNullOrEmpty(fullName))
            {
                return;
            }

            var id = testEvent.GetAttribute("id");
            var parentId = testEvent.GetAttribute("parentId");
            string flowId;
            if (parentId != null)
            {
                // NUnit 3 case
                string rootId;
                flowId = TryFindRootId(parentId, out rootId) ? rootId : id;
            }
            else
            {
                // NUnit 2 case
                flowId = _rootFlowId;
            }

            string testFlowId;
            if (id != flowId && parentId != null)
            {
                testFlowId = id;
            }
            else
            {
                testFlowId = flowId;
                if (testFlowId == null)
                {
                    testFlowId = id;
                }
            }

            switch (messageName.ToLowerInvariant())
            {
                case "start-suite":
                    _refs[id] = parentId;
                    StartSuiteCase(id, parentId, flowId, fullName);
                    break;

                case "test-suite":
                    _refs.Remove(id);
                    TestSuiteCase(id, parentId, flowId, fullName);
                    break;

                case "start-test":
                    _refs[id] = parentId;
                    CaseStartTest(id, flowId, parentId, testFlowId, fullName);
                    break;

                case "test-case":
                    try
                    {
                        if (!_refs.Remove(id))
                        {
                            // When test without starting
                            CaseStartTest(id, flowId, parentId, testFlowId, fullName);
                        }

                        var result = testEvent.GetAttribute("result");
                        if (string.IsNullOrEmpty(result))
                        {
                            break;
                        }

                        switch (result.ToLowerInvariant())
                        {
                            case "passed":
                                OnTestFinished(testFlowId, testEvent, fullName);
                                break;

                            case "inconclusive":
                                OnTestInconclusive(testFlowId, testEvent, fullName);
                                break;

                            case "skipped":
                                OnTestSkipped(testFlowId, testEvent, fullName);
                                break;

                            case "failed":
                                OnTestFailed(testFlowId, testEvent, fullName);
                                break;
                        }
                    }
                    finally
                    {
                        if (id != flowId && parentId != null)
                        {
                            OnFlowFinished(id);
                        }
                    }

                    break;
            }
        }

        private void CaseStartTest(string id, string flowId, string parentId, string testFlowId, string fullName)
        {
            if (id != flowId && parentId != null)
            {
                OnFlowStarted(id, flowId);
            }

            OnTestStart(testFlowId, fullName);
        }

        private void TestSuiteCase(string id, string parentId, string flowId, string fullName)
        {
            // NUnit 3 case
            if (parentId == string.Empty)
            {
                OnRootSuiteFinish(flowId, fullName);
            }

            // NUnit 2 case
            if (parentId == null)
            {
                if (--_blockCounter == 0)
                {
                    _rootFlowId = null;
                    OnRootSuiteFinish(id, fullName);
                }
            }
        }

        private void StartSuiteCase(string id, string parentId, string flowId, string fullName)
        {
            // NUnit 3 case
            if (parentId == string.Empty)
            {
                OnRootSuiteStart(flowId, fullName);
            }

            // NUnit 2 case
            if (parentId == null)
            {
                if (_blockCounter++ == 0)
                {
                    _rootFlowId = id;
                    OnRootSuiteStart(id, fullName);
                }
            }
        }

        private bool TryFindParentId(string id, out string parentId)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return _refs.TryGetValue(id, out parentId) && !string.IsNullOrEmpty(parentId);
        }

        private bool TryFindRootId(string id, out string rootId)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            while (TryFindParentId(id, out rootId) && id != rootId)
            {
                id = rootId;
            }

            rootId = id;
            return !string.IsNullOrEmpty(id);
        }

        private void TrySendOutput(string flowId, XmlNode message, string fullName)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var output = message.SelectSingleNode("output");
            if (output == null)
            {
                return;
            }

            var outputStr = output.InnerText;
            if (string.IsNullOrEmpty(outputStr))
            {
                return;
            }

            WriteLine("##teamcity[testStdOut name='{0}' out='{1}' flowId='{2}']", fullName, outputStr, flowId);
        }

        private void OnRootSuiteStart(string flowId, string assemblyName)
        {
            assemblyName = Path.GetFileName(assemblyName);
            WriteLine("##teamcity[testSuiteStarted name='{0}' flowId='{1}']", assemblyName, flowId);
        }

        private void OnRootSuiteFinish(string flowId, string assemblyName)
        {
            assemblyName = Path.GetFileName(assemblyName);
            WriteLine("##teamcity[testSuiteFinished name='{0}' flowId='{1}']", assemblyName, flowId);
        }

        private void OnFlowStarted(string flowId, string parentFlowId)
        {
            WriteLine("##teamcity[flowStarted flowId='{0}' parent='{1}']", flowId, parentFlowId);
        }

        private void OnFlowFinished(string flowId)
        {
            WriteLine("##teamcity[flowFinished flowId='{0}']", flowId);
        }

        private void OnTestStart(string flowId, string fullName)
        {
            WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='false' flowId='{1}']", fullName, flowId);
        }

        private void OnTestFinished(string flowId, XmlNode message, string fullName)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var durationStr = message.GetAttribute("duration");
            double durationDecimal;
            int durationMilliseconds = 0;
            if (durationStr != null && double.TryParse(durationStr, NumberStyles.Any, CultureInfo.InvariantCulture, out durationDecimal))
            {
                durationMilliseconds = (int)(durationDecimal * 1000d);
            }

            TrySendOutput(flowId, message, fullName);
            WriteLine(
                "##teamcity[testFinished name='{0}' duration='{1}' flowId='{2}']",
                fullName,
                durationMilliseconds.ToString(),
                flowId);
        }

        private void OnTestFailed(string flowId, XmlNode message, string fullName)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var errorMmessage = message.SelectSingleNode("failure/message");
            var stackTrace = message.SelectSingleNode("failure/stack-trace");
            WriteLine(
                "##teamcity[testFailed name='{0}' message='{1}' details='{2}' flowId='{3}']",
                fullName,
                errorMmessage == null ? string.Empty : errorMmessage.InnerText,
                stackTrace == null ? string.Empty : stackTrace.InnerText,
                flowId);

            OnTestFinished(flowId, message, fullName);
        }

        private void OnTestSkipped(string flowId, XmlNode message, string fullName)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            TrySendOutput(flowId, message, fullName);
            var reason = message.SelectSingleNode("reason/message");
            WriteLine(
                "##teamcity[testIgnored name='{0}' message='{1}' flowId='{2}']",
                fullName,
                reason == null ? string.Empty : reason.InnerText,
                flowId);
        }

        private void OnTestInconclusive(string flowId, XmlNode message, string fullName)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            TrySendOutput(flowId, message, fullName);
            WriteLine(
                "##teamcity[testIgnored name='{0}' message='{1}' flowId='{2}']",
                fullName,
                "Inconclusive",
                flowId);
        }

        private void WriteLine(string format, params string[] arg)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }

            var argObjects = new object[arg.Length];
            for (var i = 0; i < arg.Length; i++)
            {
                var str = arg[i];
                if (str != null)
                {
                    str = Escape(str);
                }

                argObjects[i] = str;
            }

            var message = string.Format(format, argObjects);
            _outWriter.WriteLine(message);
        }

        private static string Escape(string input)
        {
            return input != null
                ? input.Replace("|", "||")
                       .Replace("'", "|'")
                       .Replace("\n", "|n")
                       .Replace("\r", "|r")
                       .Replace(char.ConvertFromUtf32(int.Parse("0086", NumberStyles.HexNumber)), "|x")
                       .Replace(char.ConvertFromUtf32(int.Parse("2028", NumberStyles.HexNumber)), "|l")
                       .Replace(char.ConvertFromUtf32(int.Parse("2029", NumberStyles.HexNumber)), "|p")
                       .Replace("[", "|[")
                       .Replace("]", "|]")
                : null;
        }
    }
}
