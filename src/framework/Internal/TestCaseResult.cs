using System;
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestCaseResult represents the result of running a
    /// single test case.
    /// </summary>
    public class TestCaseResult : TestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseResult"/> class.
        /// </summary>
        /// <param name="test">The test to be used</param>
        public TestCaseResult(ITest test)
            : base(test)
        {
        }

        /// <summary>
        /// Adds the XML representation of the result as a child of the
        /// supplied parent node..
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            XmlNode node = XmlHelper.AddElement(parentNode, "test-case");

            TestStatus status = this.ResultState.Status;

            XmlHelper.AddAttribute(node, "name", this.Name);
            XmlHelper.AddAttribute(node, "fullname", this.FullName);
            XmlHelper.AddAttribute(node, "result", status.ToString());
            XmlHelper.AddAttribute(node, "time", this.Time.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));

            if (status == TestStatus.Failed)
                AddFailureElement(node);
            else if (status == TestStatus.Skipped)
                AddReasonElement(node);

            return node;
        }
    }
}
