// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
//using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The TextCapture class intercepts console output and writes it
    /// to the current execution context, if one is present on the thread.
    /// If no execution context is found, the output is written to a
    /// default destination, normally the original destination of the
    /// intercepted output.
    /// </summary>
    public class TextCapture : TextWriter
    {
        private readonly TextWriter _defaultWriter;

        /// <summary>
        /// Construct a TextCapture object
        /// </summary>
        /// <param name="defaultWriter">The default destination for non-intercepted output</param>
        public TextCapture(TextWriter defaultWriter)
        {
            _defaultWriter = defaultWriter;
        }

        /// <summary>
        /// Gets the Encoding in use by this TextWriter
        /// </summary>
        public override System.Text.Encoding Encoding => _defaultWriter.Encoding;

        /// <summary>
        /// Writes a single character
        /// </summary>
        /// <param name="value">The char to write</param>
        public override void Write(char value)
        {
            var context = TestExecutionContext.CurrentContext;

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.Write(value);
            else
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Writes a string
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void Write(string? value)
        {
            var context = TestExecutionContext.CurrentContext;

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.Write(value);
            else
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Writes a string followed by a line terminator
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void WriteLine(string? value)
        {
            var context = TestExecutionContext.CurrentContext;

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.WriteLine(value);
            else
                _defaultWriter.WriteLine(value);
        }
    }
}
