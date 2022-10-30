// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestProgressReporter translates ITestListener events into
    /// the async callbacks that are used to inform the client
    /// software about the progress of a test run.
    /// </summary>
    public class TestProgressReporter : ITestListener
    {
        static readonly Logger log = InternalTrace.GetLogger("TestProgressReporter");

        private readonly ICallbackEventHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProgressReporter"/> class.
        /// </summary>
        /// <param name="handler">The callback handler to be used for reporting progress.</param>
        public TestProgressReporter(ICallbackEventHandler handler)
        {
            this.handler = handler;
        }

        #region ITestListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            var parent = GetParent(test);
            try
            {
                string report;
                if (test is TestSuite)
                {
                    // Only add framework-version for the Assembly start-suite
                    string version = test.TestType == "Assembly" ? $"framework-version=\"{typeof(TestProgressReporter).Assembly.GetName().Version}\" " : "";
                    report = $"<start-suite id=\"{test.Id}\" parentId=\"{(parent != null ? parent.Id : string.Empty)}\" name=\"{FormatAttributeValue(test.Name)}\" fullname=\"{FormatAttributeValue(test.FullName)}\" type=\"{test.TestType}\" {version}/>";
                }
                else
                {
                    report = $"<start-test id=\"{test.Id}\" parentId=\"{(parent != null ? parent.Id : string.Empty)}\" name=\"{FormatAttributeValue(test.Name)}\" fullname=\"{FormatAttributeValue(test.FullName)}\" type=\"{test.TestType}\" classname=\"{FormatAttributeValue(test.ClassName ?? "")}\" methodname=\"{FormatAttributeValue(test.MethodName ?? "")}\"/>";
                }

                handler.RaiseCallbackEvent(report);
            }
            catch (Exception ex)
            {
                log.Error($"Exception processing {test.FullName}{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Called when a test has finished. Sends a result summary to the callback.
        /// to 
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            try
            {
                var node = result.ToXml(false);
                var parent = GetParent(result.Test);
                node.Attributes.Add("parentId", parent != null ? parent.Id : string.Empty);
                handler.RaiseCallbackEvent(node.OuterXml);
            }
            catch (Exception ex)
            {
                log.Error($"Exception processing {result.FullName}{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
            try
            {
                handler.RaiseCallbackEvent(output.ToXml());
            }
            catch (Exception ex)
            {
                log.Error($"Exception processing TestOutput event{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A <see cref="TestMessage"/> object containing the text to send</param>
        public void SendMessage(TestMessage message)
        {
            try
            {
                handler.RaiseCallbackEvent(message.ToXml());
            }
            catch (Exception ex)
            {
                log.Error($"Exception processing SendMessage event{Environment.NewLine}{ex}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the parent test item for the target test item if it exists
        /// </summary>
        /// <param name="test"></param>
        /// <returns>parent test item</returns>
        private static ITest? GetParent(ITest test)
        {
            if (test == null || test.Parent == null)
            {
                return null;
            }

            return test.Parent.IsSuite ? test.Parent : GetParent(test.Parent);
        }

        /// <summary>
        /// Makes a string safe for use as an attribute, replacing
        /// characters that can't be used with their
        /// corresponding XML representations.
        /// </summary>
        /// <param name="original">The string to be used</param>
        /// <returns>A new string with the values replaced</returns>
        private static string FormatAttributeValue(string original)
        {
            return original
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        #endregion
    }
}
