// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
//using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The TextCapture class intercepts console output and writes it
    /// to the current execution context, unless it corresponds with an
    /// ad-hoc test context. In an ad-hoc context, the output is written
    /// to a default destination, normally the original destination of the
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
        public override void Write(char value) => GetWriter().Write(value);

        /// <summary>
        /// Writes a string
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void Write(string? value) => GetWriter().Write(value);

        /// <summary>
        /// Writes a string followed by a line terminator
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void WriteLine(string? value) => GetWriter().WriteLine(value);

        private TextWriter GetWriter()
        {
            var context = TestExecutionContext.CurrentContext;

            if (context is not TestExecutionContext.AdhocContext && context.CurrentResult is not null)
                return context.CurrentResult.OutWriter;
            else
                return _defaultWriter;
        }
    }
}
