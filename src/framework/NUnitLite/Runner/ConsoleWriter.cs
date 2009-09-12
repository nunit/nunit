using System;
using System.IO;

namespace NUnitLite.Runner
{
    /// <summary>
    /// Provide an alternative to Console.Out for 
    /// version 1.0 of the compact framework.
    /// </summary>
    public class ConsoleWriter : TextWriter
    {
        private static TextWriter writer;

        /// <summary>
        /// Gets the underlying TextWriter, creating it if it does not already exist.
        /// </summary>
        /// <value>The underlying TextWriter.</value>
        public static TextWriter Out
        {
            get
            {
                if ( writer == null )
                    writer = new ConsoleWriter();

                return writer; 
            }
        }

        /// <summary>
        /// Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextWriter"/> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Write(char value)
        {
            Console.Write(value);
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextWriter"/> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Write(string value)
        {
            Console.Write(value);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value"/> is null, only the line termination characters are written.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextWriter"/> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> in which the output is written.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The Encoding in which the output is written.
        /// </returns>
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
