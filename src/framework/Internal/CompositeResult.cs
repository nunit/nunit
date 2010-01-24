using System;
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestSuiteResult represents the result of running a TestSuite.
    /// </summary>
    public class CompositeResult : TestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeResult"/> class.
        /// </summary>
        /// <param name="test">The test to be used</param>
        public CompositeResult(ITest test)
            : base(test) 
        {
        }

        /// <summary>
        /// Add a child result
        /// </summary>
        /// <param name="result">The child result to be added</param>
        public void AddResult(TestResult result)
        {
            this.Children.Add(result);

            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    if (this.ResultState.Status != TestStatus.Failed)
                        this.SetResult(ResultState.Failure, "Child test failed");
                    break;
                case TestStatus.Passed:
                    if (this.ResultState == ResultState.Inconclusive)
                        this.SetResult(ResultState.Success);
                    break;
            }
        }

        /// <summary>
        /// Adds the XML representation of the result as a child of the
        /// supplied parent node..
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override XmlNode AddToXml(XmlNode parent, bool recursive)
        {
            XmlNode node = XmlHelper.AddElement(parent, "test-suite");

            TestStatus status = this.ResultState.Status;

            XmlHelper.AddAttribute(node, "name", this.Name);
            XmlHelper.AddAttribute(node, "fullname", this.FullName);
            XmlHelper.AddAttribute(node, "result", status.ToString());
            XmlHelper.AddAttribute(node, "time", this.Time.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));

            if (status == TestStatus.Failed)
                AddFailureElement(node);
            else if (status == TestStatus.Skipped)
                AddReasonElement(node);

            if (recursive && HasChildren)
                foreach (TestResult child in Children)
                    child.AddToXml(node, recursive);

            return node;
        }
    }
}
