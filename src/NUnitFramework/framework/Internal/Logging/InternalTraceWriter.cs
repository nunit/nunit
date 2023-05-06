// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A trace listener that writes to a separate file per domain
    /// and process using it.
    /// </summary>
    public class InternalTraceWriter : TextWriter
    {
        private TextWriter writer;
        private readonly object myLock = new object();

        /// <summary>
        /// Construct an InternalTraceWriter that writes to a file.
        /// </summary>
        /// <param name="logPath">Path to the file to use</param>
        public InternalTraceWriter(string logPath)
        {
            var streamWriter = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write));
            streamWriter.AutoFlush = true;
            this.writer = streamWriter;
        }

        /// <summary>
        /// Construct an InternalTraceWriter that writes to a
        /// TextWriter provided by the caller.
        /// </summary>
        /// <param name="writer"></param>
        public InternalTraceWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Returns the character encoding in which the output is written.
        /// </summary>
        /// <returns>The character encoding in which the output is written.</returns>
        public override System.Text.Encoding Encoding => writer.Encoding;

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            lock (myLock)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string? value)
        {
            lock (myLock)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(string? value)
        {
            lock (myLock)
            {
                writer.WriteLine(value);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && writer is not null)
            {
                writer.Flush();
                writer.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            writer.Flush();
        }
    }
}
