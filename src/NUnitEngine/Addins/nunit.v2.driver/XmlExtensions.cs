// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Globalization;
using System.Text;
using System.Xml;
using NUnit.Core;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

namespace NUnit.Engine.Drivers
{
    // NOTE: The following elements and attributes are not available in 
    // NUnit v2 and therefore are not included in the XML created by this class.
    // All of these are optional as far as the TestEngine is concerned
    // but runners that use them need to allow for their absence:
    //   1. seed attribute
    //   2. start-time attribute
    //   3. end-time attribute
    //   4. output element
    //   5. totals are all set to 0 in the test-suite element
    // The last item can be corrected if we decide to, but it will require
    // decending the tree to calculate the totals at each node.

    public static class XmlExtensions
    {
        #region ITest Extensions

        /// <summary>
        /// Returns an XmlNode representing a test
        /// </summary>
        public static XmlNode ToXml(this ITest test, bool recursive)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<dummy/>");
            var topNode = doc.FirstChild;

            topNode.AddTest(test, recursive);

            return topNode.FirstChild;
        }

        #endregion

        #region TestResult Extensions

        /// <summary>
        /// Returns an XmlNode representing a TestResult
        /// </summary>
        public static XmlNode ToXml(this TestResult result, bool recursive)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<dummy/>");
            var topNode = doc.FirstChild;

            topNode.AddResult(result, recursive);

            return topNode.FirstChild;
        }

        #endregion

        #region XmlNode Extensions

        // Adds a test-suite or test-case element, without result info
        private static XmlNode AddTest(this XmlNode parent, ITest test, bool recursive)
        {
            var thisNode = parent.AddElement(test.IsSuite ? "test-suite" : "test-case");

            if (test.IsSuite)
                thisNode.AddAttribute("type", test.TestType);
            thisNode.AddAttribute("id", test.TestName.TestID.ToString());
            thisNode.AddAttribute("name", Escape(test.TestName.Name));
            thisNode.AddAttribute("fullname", Escape(test.TestName.FullName));
            thisNode.AddAttribute("runstate", test.RunState.ToString());

            if (test.IsSuite)
                thisNode.AddAttribute("testcasecount", test.TestCount.ToString());

            if (test.Properties.Keys.Count > 0)
                thisNode.AddProperties(test);

            if (recursive && test.IsSuite)
                foreach (ITest child in test.Tests)
                    thisNode.AddTest(child, recursive);

            return thisNode;
        }

        // Adds a test-suite or test-case element, with result info
        private static void AddResult(this XmlNode parent, TestResult result, bool recursive)
        {
            var thisNode = parent.AddTest(result.Test, false);

            string status = GetTranslatedResultState(result.ResultState);

            var colon = status.IndexOf(':');
            string label = null;
            if (colon > 0)
            {
                label = status.Substring(colon + 1);
                status = status.Substring(0, colon);
            }

            thisNode.AddAttribute("result", status);
            if (label != null)
                thisNode.AddAttribute("label", label);

            // TODO: Check values are the same between V2 and V3
            if (result.FailureSite != FailureSite.Test)
                parent.AddAttribute("site", result.FailureSite.ToString());

            //start-time not available
            //end-time not available
            thisNode.AddAttribute("duration", result.Time.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            if (result.Test.IsSuite)
            {
                // TODO: These values all should be calculated
                thisNode.AddAttribute("total", result.Test.TestCount.ToString());
                thisNode.AddAttribute("passed", "0");
                thisNode.AddAttribute("failed", "0");
                thisNode.AddAttribute("inconclusive", "0");
                thisNode.AddAttribute("skipped", "0");
            }
            thisNode.AddAttribute("asserts", result.AssertCount.ToString());

            if (status == "Failed")
                thisNode.AddFailureElement(result);
            else if (result.Message != null)
                thisNode.AddReasonElement(result);

            //OutputElement not available

            if (recursive && result.HasResults)
                foreach (TestResult child in result.Results)
                    thisNode.AddResult(child, recursive);
        }

        //Adds a properties element and its contents
        private static void AddProperties(this XmlNode parent, ITest test)
        {
            var properties = parent.AddElement("properties");

            foreach (string key in test.Properties.Keys)
            {
                object propValue = test.Properties[key];

                // NOTE: This depends on categories being the only multi-valued 
                // property. We can count on this because NUnit V2 is not 
                // being developed any further.
                if (key == "_CATEGORIES")
                    foreach (string category in (IList)propValue)
                        properties.AddProperty(key, category);
                else
                    properties.AddProperty(key, propValue);
            }
        }

        // Adds a property element and its contents
        private static void AddProperty(this XmlNode parent, string key, object val)
        {
            var node = parent.AddElement("property");
            node.AddAttribute("name", key);
            node.AddAttribute("value", val.ToString());
        }

        private static void AddFailureElement(this XmlNode parent, TestResult result)
        {
            var thisNode = parent.AddElement("failure");

            if (result.Message != null)
                thisNode.AddElementWithCDataSection("message", result.Message);

            if (result.StackTrace != null)
                thisNode.AddElementWithCDataSection("stack-trace", result.StackTrace);
        }

        private static void AddReasonElement(this XmlNode parent, TestResult result)
        {
            parent.AddElement("reason").AddElementWithCDataSection("message", result.Message);
        }

        /// <summary>
        /// Adds an attribute with a specified name and value to an existing XmlNode.
        /// </summary>
        /// <param name="node">The node to which the attribute should be added.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        private static void AddAttribute(this XmlNode node, string name, string value)
        {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

        /// <summary>
        /// Adds a new element as a child of an existing XmlNode and returns it.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <returns>The newly created child element</returns>
        private static XmlNode AddElement(this XmlNode node, string name)
        {
            XmlNode childNode = node.OwnerDocument.CreateElement(name);
            node.AppendChild(childNode);
            return childNode;
        }

        /// <summary>
        /// Adds the a new element as a child of an existing node and returns it.
        /// A CDataSection is added to the new element using the data provided.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <param name="data">The data for the CDataSection.</param>
        /// <returns></returns>
        private static XmlNode AddElementWithCDataSection(this XmlNode node, string name, string data)
        {
            XmlNode childNode = node.AddElement(name);
            childNode.AppendChild(node.OwnerDocument.CreateCDataSection(data));
            return childNode;
        }

        #endregion

        #region Helper Methods

        // Escapes a string for use in the XML
        private static string Escape(string original)
        {
            return original
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        // Returns ResultState translated to a v3 string representation
        private static string GetTranslatedResultState(ResultState resultState)
        {
            switch (resultState)
            {
                case ResultState.Inconclusive:
                    return "Inconclusive";
                case ResultState.NotRunnable:
                    return "Failed:Invalid";
                case ResultState.Skipped:
                    return "Skipped";
                case ResultState.Ignored:
                    return "Skipped:Ignored";
                case ResultState.Success:
                    return "Passed";
                case ResultState.Failure:
                    return "Failed";
                case ResultState.Error:
                    return "Failed:Error";
                case ResultState.Cancelled:
                    return "Failed:Cancelled";
            }

            return resultState.ToString();
        }

        #endregion
    }
}
