// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

        TestListenerIntercepter ListenerResult;
        TextWriter ListenerWriter;

        [SetUp]
        public void SetUp()
        {
            // Wrap the current listener, listening to events, and forwarding the original event
            ListenerResult = new TestListenerIntercepter(TestExecutionContext.CurrentContext.Listener);
            TestExecutionContext.CurrentContext.Listener = ListenerResult;
#if NETCOREAPP1_1
            ListenerWriter = new EventListenerTextWriter(STREAM_NAME, TextWriter.Null);
#else
            ListenerWriter = TextWriter.Synchronized(new EventListenerTextWriter(STREAM_NAME, TextWriter.Null));
#endif
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));

        }

        [Test]
        public void TestWriteObject()
        {
            var obj = new { Mary = "Lamb", Sheep = "White" };

            ListenerWriter.Write(obj);
            ListenerWriter.WriteLine(obj);

            var expected = "{ Mary = Lamb, Sheep = White }";
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteBool()
        {
            bool value = true;

            ListenerWriter.Write(value);
            ListenerWriter.WriteLine(value);

            var expected = Boolean.TrueString;
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
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
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(ListenerResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(ListenerResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteLine()
        {
            ListenerWriter.WriteLine();

            var expected = NL;
            Assert.That(ListenerResult.Outputs.Count, Is.EqualTo(1));
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
                if (DefaultListener != null)
                    DefaultListener.TestStarted(test);
            }

            void ITestListener.TestFinished(ITestResult result)
            {
                if (DefaultListener != null)
                    DefaultListener.TestFinished(result);
            }

            void ITestListener.TestOutput(TestOutput output)
            {
                Assert.IsNotNull(output);
                Outputs.Add(output.Text);

                if (DefaultListener != null)
                    DefaultListener.TestOutput(output);
            }
        }

#endregion
    }
}
