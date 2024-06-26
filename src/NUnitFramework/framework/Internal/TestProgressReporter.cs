// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestProgressReporter translates ITestListener events into
    /// the async callbacks that are used to inform the client
    /// software about the progress of a test run.
    /// </summary>
    public sealed class TestProgressReporter : ITestListener, ITestListenerExt
    {
        private static readonly Logger Log = InternalTrace.GetLogger("TestProgressReporter");

        private readonly ICallbackEventHandler _handler;

        // this reporter is being called synchronously so we can reuse shared string builder
        private readonly StringBuilder _stringBuilder = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProgressReporter"/> class.
        /// </summary>
        /// <param name="handler">The callback handler to be used for reporting progress.</param>
        public TestProgressReporter(ICallbackEventHandler handler)
        {
            _handler = handler;
        }

        #region ITestListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            var message = CreateTestStartedMessage(test);

            try
            {
                _handler.RaiseCallbackEvent(message);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {test.FullName}{Environment.NewLine}{ex}");
            }
        }

        private string CreateTestStartedMessage(ITest test)
        {
            var parent = GetParent(test);

            var stringBuilder = GetStringBuilder();

            using var stringWriter = new StringWriter(stringBuilder);
            using (var writer = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                var isSuite = test is TestSuite;
                writer.WriteStartElement(isSuite ? "start-suite" : "start-test");

                writer.WriteAttributeString("id", test.Id);
                writer.WriteAttributeString("parentId", parent is not null ? parent.Id : string.Empty);
                writer.WriteAttributeStringSafe("name", test.Name);
                writer.WriteAttributeStringSafe("fullname", test.FullName);
                writer.WriteAttributeStringSafe("type", test.TestType);

                if (isSuite)
                {
                    // Only add framework-version for the Assembly start-suite
                    if (test.TestType == "Assembly")
                    {
                        writer.WriteAttributeString("framework-version", typeof(TestProgressReporter).Assembly.GetName().Version?.ToString());
                    }
                }
                else
                {
                    writer.WriteAttributeStringSafe("classname", test.ClassName ?? string.Empty);
                    writer.WriteAttributeStringSafe("methodname", test.MethodName ?? string.Empty);
                }

                writer.WriteEndElement();
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Called when a test has finished. Sends a result summary to the callback.
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            var node = result.ToXml(false);
            var parent = GetParent(result.Test);
            node.AddAttribute("parentId", parent is not null ? parent.Id : string.Empty);

            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                node.WriteTo(xmlWriter);
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {result.FullName}{Environment.NewLine}{ex}");
            }
        }

        /// <inheritdoc/>
        public void OneTimeSetUpStarted(ITest test)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                var parent = GetParent(test);
                xmlWriter.WriteStartElement("OneTimeSetUpStarted");
                xmlWriter.WriteAttributeString("id", test.Id);
                xmlWriter.WriteAttributeString("parentId", parent is not null ? parent.Id : string.Empty);
                xmlWriter.WriteAttributeStringSafe("name", test.Name);
                xmlWriter.WriteAttributeStringSafe("fullname", test.FullName);
                xmlWriter.WriteAttributeStringSafe("type", test.TestType);
                xmlWriter.WriteEndElement();
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {ex}");
            }
        }

        /// <inheritdoc/>
        public void OneTimeSetUpFinished(ITest test)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                var parent = GetParent(test);
                xmlWriter.WriteStartElement("OneTimeSetUpFinished");
                xmlWriter.WriteAttributeString("id", test.Id);
                xmlWriter.WriteAttributeString("parentId", parent is not null ? parent.Id : string.Empty);
                xmlWriter.WriteAttributeStringSafe("name", test.Name);
                xmlWriter.WriteAttributeStringSafe("fullname", test.FullName);
                xmlWriter.WriteAttributeStringSafe("type", test.TestType);
                xmlWriter.WriteEndElement();
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {ex}");
            }
        }

        /// <inheritdoc/>
        public void OneTimeTearDownStarted(ITest test)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                var parent = GetParent(test);
                xmlWriter.WriteStartElement("OneTimeTearDownStarted");
                xmlWriter.WriteAttributeString("id", test.Id);
                xmlWriter.WriteAttributeString("parentId", parent is not null ? parent.Id : string.Empty);
                xmlWriter.WriteAttributeStringSafe("name", test.Name);
                xmlWriter.WriteAttributeStringSafe("fullname", test.FullName);
                xmlWriter.WriteAttributeStringSafe("type", test.TestType);
                xmlWriter.WriteEndElement();
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {ex}");
            }
        }

        /// <inheritdoc/>
        public void OneTimeTearDownFinished(ITest test)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                var parent = GetParent(test);
                xmlWriter.WriteStartElement("OneTimeTearDownFinished");
                xmlWriter.WriteAttributeString("id", test.Id);
                xmlWriter.WriteAttributeString("parentId", parent is not null ? parent.Id : string.Empty);
                xmlWriter.WriteAttributeStringSafe("name", test.Name);
                xmlWriter.WriteAttributeStringSafe("fullname", test.FullName);
                xmlWriter.WriteAttributeStringSafe("type", test.TestType);
                xmlWriter.WriteEndElement();
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing {ex}");
            }
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var writer = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                output.ToXml(writer);
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing TestOutput event{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A <see cref="TestMessage"/> object containing the text to send</param>
        public void SendMessage(TestMessage message)
        {
            using var stringWriter = new StringWriter(GetStringBuilder());
            using (var writer = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                message.ToXml(writer);
            }

            try
            {
                _handler.RaiseCallbackEvent(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception processing SendMessage event{Environment.NewLine}{ex}");
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
            while (true)
            {
                var parent = test?.Parent;

                if (parent is null)
                {
                    return null;
                }

                if (parent.IsSuite)
                {
                    return parent;
                }

                test = parent;
            }
        }

        private StringBuilder GetStringBuilder()
        {
            _stringBuilder.Clear();
            return _stringBuilder;
        }

        #endregion
    }
}
