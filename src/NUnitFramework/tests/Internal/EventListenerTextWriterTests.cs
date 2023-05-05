// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class EventListenerTextWriterTests
    {
        private static readonly string STREAM_NAME = "EventListenerTextWriterTestsStream";
        private static readonly string NL = Environment.NewLine;

#pragma warning disable IDE1006 // Naming Styles
        private TestListenerIntercepter ListenerResult;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
        private TextWriter ListenerWriter;
#pragma warning restore IDE1006 // Naming Styles

        [SetUp]
        public void SetUp()
        {
            // Wrap the current listener, listening to events, and forwarding the original event
            ListenerResult = new TestListenerIntercepter(TestExecutionContext.CurrentContext.Listener);
            TestExecutionContext.CurrentContext.Listener = ListenerResult;
            ListenerWriter = TextWriter.Synchronized(new EventListenerTextWriter(STREAM_NAME, TextWriter.Null));
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original listener
            TestExecutionContext.CurrentContext.Listener = ListenerResult.DefaultListener;
        }

        [Test]
        public void TestWriteStringArgArray()
        {
            var format = "{0} {1} {2} {3}";
            var arg = new object[] { "Hello", 4, 2, "World" };

            ListenerWriter.Write(format, arg);
            ListenerWriter.WriteLine(format, arg);

            var expected = "Hello 4 2 World";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteStringArg1()
        {
            var format = "{0:dd MMM yyyy}";
            var arg0 = new DateTime(2017, 4, 20);

            ListenerWriter.Write(format, arg0);
            ListenerWriter.WriteLine(format, arg0);

            var expected = $"{arg0:dd MMM yyyy}";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
                Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
            });
        }

        [Test]
        public void TestWriteStringArg2()
        {
            var format = "{0:00.00} {1}";
            var arg0 = 5;
            var arg1 = "@";

            ListenerWriter.Write(format, arg0, arg1);
            ListenerWriter.WriteLine(format, arg0, arg1);

            var expected = $"{5:00.00} @";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
                Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
            });
        }

        [Test]
        public void TestWriteStringArg3()
        {
            var format = "{0} {1:#.00} {2}";
            var arg0 = "Quick";
            var arg1 = 9.0;
            var arg2 = "Fox";

            ListenerWriter.Write(format, arg0, arg1, arg2);
            ListenerWriter.WriteLine(format, arg0, arg1, arg2);

            var expected = $"Quick {9:#.00} Fox";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
                Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
            });
        }

        [Test]
        public void TestWriteObject()
        {
            var obj = new { Mary = "Lamb", Sheep = "White" };

            ListenerWriter.Write(obj);
            ListenerWriter.WriteLine(obj);

            var expected = "{ Mary = Lamb, Sheep = White }";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteString()
        {
            var str = "Insert coin here";

            ListenerWriter.Write(str);
            ListenerWriter.WriteLine(str);

            var expected = "Insert coin here";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteDecimal()
        {
            decimal value = 2.731m;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = $"{2.731:0.000}";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteDouble()
        {
            double value = -1.5;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = $"{-1.5:0.0}";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteULong()
        {
            ulong value = 1234567890123456;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "1234567890123456";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteLong()
        {
            long value = -987654321;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "-987654321";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteUInt()
        {
            uint value = 0xff;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "255";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteInt()
        {
            int value = 0xff;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "255";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteBool()
        {
            bool value = true;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = bool.TrueString;
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteChar()
        {
            char value = 'x';

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "x";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteCharBuffer()
        {
            char[] buffer = new char[] { 'H', 'e', 'l', 'l', 'o', ' ', 'W', 'o', 'r', 'l', 'd' };

            ListenerWriter.Write(buffer);
            ListenerWriter.WriteLine(buffer);

            var expected = "Hello World";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteCharBufferSubstring()
        {
            char[] buffer = new char[] { 'L', 'i', 't', 't', 'l', 'e', ' ', 'M', 'i', 's', 's', ' ', 'M', 'u', 'f', 'f', 'e', 't' };
            int index = 6;
            int count = 7;

            ListenerWriter.Write(buffer, index, count);
            ListenerWriter.WriteLine(buffer, index, count);

            var expected = " Miss M";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteFloat()
        {
            float value = -5.0f;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = "-5";
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteLine()
        {
            ListenerWriter.WriteLine();

            var expected = NL;
            Assert.That(ListenerResult.Outputs, Has.Count.EqualTo(1));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
        }

#region ITestListener implementation

        private class TestListenerIntercepter : ITestListener
        {
            public IList<string> Outputs { get; }
            public ITestListener DefaultListener { get; }

            public TestListenerIntercepter(ITestListener defaultListener)
            {
                DefaultListener = defaultListener;
                Outputs = new List<string>();
            }

            void ITestListener.TestStarted(ITest test)
            {
                DefaultListener?.TestStarted(test);
            }

            void ITestListener.TestFinished(ITestResult result)
            {
                DefaultListener?.TestFinished(result);
            }

            void ITestListener.TestOutput(TestOutput output)
            {
                Assert.That(output, Is.Not.Null);
                Outputs.Add(output.Text);

                DefaultListener?.TestOutput(output);
            }

            void ITestListener.SendMessage(TestMessage message)
            {

            }
        }

#endregion
    }
}
