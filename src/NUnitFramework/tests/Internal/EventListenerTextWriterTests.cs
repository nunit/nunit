// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class EventListenerTextWriterTests
    {
        private static readonly string NL = Environment.NewLine;

        TestListenerHelper TestResult;
        TextWriter TestWriter;

        [SetUp]
        public void SetUp()
        {
            TestResult = new TestListenerHelper();
            TestExecutionContext.CurrentContext.Listener = TestResult;

            TestWriter = TextWriter.Synchronized(TestContext.Error);
        }

        [Test]
        public void TestWriteStringArgArray()
        {
            var format = "{0} {1} {2} {3}";
            var arg = new object[] { "Hello", 4, 2, "World" };

            TestWriter.Write(format, arg);
            TestWriter.WriteLine(format, arg);

            var expected = "Hello 4 2 World";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteStringArg1()
        {
            var format = "{0:dd MMM yyyy}";
            var arg0 = new DateTime(2017, 4, 20);

            TestWriter.Write(format, arg0);
            TestWriter.WriteLine(format, arg0);

            var expected = "20 Apr 2017";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteStringArg2()
        {
            var format = "{0:00.00} {1}";
            var arg0 = 5;
            var arg1 = "@";

            TestWriter.Write(format, arg0, arg1);
            TestWriter.WriteLine(format, arg0, arg1);

            var expected = "05.00 @";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteStringArg3()
        {
            var format = "{0} {1:#.00} {2}";
            var arg0 = "Quick";
            var arg1 = 9.0;
            var arg2 = "Fox";

            TestWriter.Write(format, arg0, arg1, arg2);
            TestWriter.WriteLine(format, arg0, arg1, arg2);

            var expected = "Quick 9.00 Fox";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));

        }

        [Test]
        public void TestWriteObject()
        {
            var obj = new { Mary = "Lamb", Sheep = "White" };

            TestWriter.Write(obj);
            TestWriter.WriteLine(obj);

            var expected = "{ Mary = Lamb, Sheep = White }";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteString()
        {
            var str = "Insert coin here";

            TestWriter.Write(str);
            TestWriter.WriteLine(str);

            var expected = "Insert coin here";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteDecimal()
        {
            decimal value = 2.731m;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "2.731";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteDouble()
        {
            double value = -1.5;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "-1.5";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteULong()
        {
            ulong value = 1234567890123456;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "1234567890123456";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteLong()
        {
            long value = -987654321;
            
            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "-987654321";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteUInt()
        {
            uint value = 0xff;
            
            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "255";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteInt()
        {
            int value = 0xff;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "255";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteBool()
        {
            bool value = true;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = Boolean.TrueString;
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteChar()
        {
            char value = 'x';

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "x";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteCharBuffer()
        {
            char[] buffer = new char[] { 'H', 'e', 'l', 'l', 'o', ' ', 'W', 'o', 'r', 'l', 'd' };

            TestWriter.Write(buffer);
            TestWriter.WriteLine(buffer);

            var expected = "Hello World";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteCharBufferSubstring()
        {
            char[] buffer = new char[] { 'L', 'i', 't', 't', 'l', 'e', ' ', 'M', 'i', 's', 's', ' ', 'M', 'u', 'f', 'f', 'e', 't' };
            int index = 6;
            int count = 7;

            TestWriter.Write(buffer, index, count);
            TestWriter.WriteLine(buffer, index, count);

            var expected = " Miss M";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteFloat()
        {
            float value = -5.0f;

            TestWriter.Write(value);
            TestWriter.WriteLine(value);

            var expected = "-5";
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(2));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
            Assert.That(TestResult.Outputs[1], Is.EqualTo(expected + NL));
        }

        [Test]
        public void TestWriteLine()
        {
            TestWriter.WriteLine();

            var expected = NL;
            Assert.That(TestResult.Outputs.Count, Is.EqualTo(1));
            Assert.That(TestResult.Outputs[0], Is.EqualTo(expected));
        }

        #region ITestListener implementation

        private class TestListenerHelper : ITestListener
        {
            public IList<string> Outputs { get; internal set; }

            public TestListenerHelper()
            {
                Outputs = new List<string>();
            }

            void ITestListener.TestStarted(ITest test)
            {
                // Intentionally empty
            }

            void ITestListener.TestFinished(ITestResult result)
            {
                // Intentionally empty
            }

            void ITestListener.TestOutput(TestOutput output)
            {
                Assert.IsNotNull(output);
                Outputs.Add(output.Text);
            }
        }

        #endregion
    }
}
