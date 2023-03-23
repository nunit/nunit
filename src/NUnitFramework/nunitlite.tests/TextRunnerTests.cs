// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace NUnitLite.Tests
{
    [TestFixture]
    public class TextRunnerTests
    {
        [Test]
        public void CommandLineTests()
        {
            Console.SetOut(new WriteToStreamAndConsole(Console.Out, new MemoryStream()));
            
            var textRunner = new TextRunner();
            textRunner.Execute(new[] { "--noresult" });
            Console.Out.WriteLine("Broken");
        }
    }

    public class WriteToStreamAndConsole : TextWriter
    {
        private readonly TextWriter _writer;
        private readonly StreamWriter _streamWriter;
        
        public WriteToStreamAndConsole(TextWriter writer, Stream stream)
        {
            _writer = writer;
            _streamWriter = new StreamWriter(stream);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer.Dispose();
                _streamWriter.Dispose();
            }
            
            base.Dispose(disposing);
        }

        // Only overriden WriteLine for simplicity sake
        public override void WriteLine(string value)
        {
            _writer.WriteLine(value);
            _streamWriter.WriteLine(value);
            
            base.WriteLine(value);
        }

        public override Encoding Encoding => _writer.Encoding;
    }
}
