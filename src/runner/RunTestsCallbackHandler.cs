// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace NUnit.AdhocTestRunner
{
    public class RunTestsCallbackHandler : CallbackHandler
    {
        private TextWriter output;

        public RunTestsCallbackHandler()
        {
            this.output = Console.Out;
        }

        public override void ReportProgress(object state)
        {
            XmlNode topNode = (XmlNode)state;
            switch (topNode.Name)
            {
                case "start":
                    OnTestStart(topNode);
                    break;
                case "test-case":
                    OnTestCaseFinished(topNode);
                    break;
                case "suite":
                    OnSuiteFinished(topNode);
                    break;
                case "output":
                    OnOutput(topNode);
                    break;
            }
        }

        private void OnTestStart(XmlNode startNode)
        {
            XmlAttribute id = startNode.Attributes["id"];
            XmlAttribute name = startNode.Attributes["name"];
            //XmlAttribute fullname = startNode.Attributes["fullname"];
            XmlAttribute testcase = startNode.Attributes["testcase"];

            Debug.Assert(id != null);
            Debug.Assert(name != null);
            //Debug.Assert(fullname != null);

            //output.WriteLine(name);
        }

        private void OnTestCaseFinished(XmlNode testNode)
        {
            //int id = int.Parse(testNode.Attributes["id"].Value);
            XmlAttribute name = testNode.Attributes["name"];
            //XmlAttribute fullname = testNode.Attributes["fullname"];
            XmlAttribute result = testNode.Attributes["result"];

            Debug.Assert(name != null);
            //Debug.Assert(fullname != null);
            Debug.Assert(result != null);
        }

        private void OnSuiteFinished(XmlNode suiteNode)
        {
            //int id = int.Parse(suiteNode.Attributes["id"].Value);
            XmlAttribute name = suiteNode.Attributes["name"];
            //XmlAttribute fullname = suiteNode.Attributes["fullname"];
            XmlAttribute result = suiteNode.Attributes["result"];

            Debug.Assert(name != null);
            //Debug.Assert(fullname != null);
            Debug.Assert(result != null);
        }

        private void OnOutput(XmlNode outputNode)
        {
            XmlAttribute type = outputNode.Attributes["type"];
            XmlNode textNode = outputNode.SelectSingleNode("text");

            Debug.Assert(type != null);
            Debug.Assert(textNode != null);

            output.Write(textNode.InnerText);
        }
    }
}
