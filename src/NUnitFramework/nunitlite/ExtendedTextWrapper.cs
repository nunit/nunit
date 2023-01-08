// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
using System.Text;

namespace NUnit.Common
{
    /// <summary>
    /// ExtendedTextWrapper wraps a TextWriter and makes it
    /// look like an ExtendedTextWriter. All style indications
    /// are ignored. It's used when text is being written
    /// to a file.
    /// </summary>
    public class ExtendedTextWrapper : ExtendedTextWriter
    {
        private readonly TextWriter _writer;

        public ExtendedTextWrapper(TextWriter writer)
        {
            _writer = writer;
        }

        #region TextWriter Overrides

        /// <summary>
        /// Write a single char value
        /// </summary>
        public override void Write(char value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Write a string value
        /// </summary>
        public override void Write(string value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Write a string value followed by a NewLine
        /// </summary>
        public override void WriteLine(string value)
        {
            _writer.WriteLine(value);
        }

        /// <summary>
        /// Gets the encoding for this ExtendedTextWriter
        /// </summary>
        public override Encoding Encoding => _writer.Encoding;

        /// <summary>
        /// Dispose the Extended TextWriter
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            _writer.Dispose();
        }

        #endregion

        #region Extended Methods

        /// <summary>
        /// Writes the value with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public override void Write(ColorStyle style, string value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes the value with the specified style
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public override void WriteLine(ColorStyle style, string value)
        {
            WriteLine(value);
        }

        /// <summary>
        /// Writes the label and the option that goes with it.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        public override void WriteLabel(string label, object option)
        {
            Write(label);
            Write(option.ToString());
        }

        /// <summary>
        /// Writes the label and the option that goes with it.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        /// <param name="valueStyle">The color to display the value with</param>
        public override void WriteLabel(string label, object option, ColorStyle valueStyle)
        {
            WriteLabel(label, option);
        }

        /// <summary>
        /// Writes the label and the option that goes with it followed by a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        public override void WriteLabelLine(string label, object option)
        {
            WriteLabel(label, option);
            WriteLine();
        }

        /// <summary>
        /// Writes the label and the option that goes with it followed by a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        /// <param name="valueStyle">The color to display the value with</param>
        public override void WriteLabelLine(string label, object option, ColorStyle valueStyle)
        {
            WriteLabelLine(label, option);
        }

        #endregion
    }
}
