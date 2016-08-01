// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

#if SILVERLIGHT
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using NUnit.Common;

namespace NUnitLite.Runner.Silverlight
{
    /// <summary>
    /// TextBlockWriter is a TextWriter that sends it's 
    /// output to a Silverlight TextBlock.
    /// </summary>
    public class TextBlockWriter : ExtendedTextWriter
    {
        private TextBlock _textBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockWriter"/> class.
        /// </summary>
        /// <param name="textBlock">The text block.</param>
        public TextBlockWriter(TextBlock textBlock)
        {
            this._textBlock = textBlock;
        }

        #region TextWriter Overrides

        ///// <summary>
        ///// Writes a character to the text stream.
        ///// </summary>
        ///// <param name="value">The character to write to the text stream.</param>
        ///// <exception cref="T:System.ObjectDisposedException">
        ///// The <see cref="T:System.IO.TextWriter"/> is closed.
        ///// </exception>
        ///// <exception cref="T:System.IO.IOException">
        ///// An I/O error occurs.
        ///// </exception>
        //public override void Write(char value)
        //{
        //    textBlock.Text += value;
        //}

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
            Write(ColorStyle.Default, value);
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
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Adds a LineBreak to the TextBlock
        /// </summary>
        public override void WriteLine()
        {
            _textBlock.Inlines.Add(new LineBreak());
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
            get { return System.Text.Encoding.UTF8; }
        }

        #endregion

        #region ExtendeTextWriter Overrides

        /// <summary>
        /// Writes the value with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public override void Write(ColorStyle style, string value)
        {
            _textBlock.Inlines.Add(new Run()
            {
                Text = value,
                Foreground = GetBrush(style)
            });
        }

        /// <summary>
        /// Writes the value with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public override void WriteLine(ColorStyle style, string value)
        {
            Write(style, value);
            WriteLine();
        }

        /// <summary>
        /// Writes the label and the option that goes with it.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        public override void WriteLabel(string label, object option)
        {
            WriteLabel(label, option, ColorStyle.Value);
        }

        /// <summary>
        /// Writes the label and the option that goes with it followed by a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        public override void WriteLabelLine(string label, object option)
        {
            WriteLabelLine(label, option, ColorStyle.Value);
        }

        /// <summary>
        /// Writes the label and the option that goes with it and optionally writes a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        /// <param name="valueStyle">The color to display the value with</param>
        public override void WriteLabel(string label, object option, ColorStyle valueStyle)
        {
            Write(ColorStyle.Label, label);
            Write(valueStyle, option.ToString());
        }

        /// <summary>
        /// Writes the label and the option that goes with it followed by a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        /// <param name="valueStyle">The color to display the value with</param>
        public override void WriteLabelLine(string label, object option, ColorStyle valueStyle)
        {
            WriteLabel(label, option, valueStyle);
            WriteLine();
        }

        #endregion

        #region Helper Methods

        private SolidColorBrush GetBrush(ColorStyle style)
        {
      
            if ((int)style < 0 || (int)style > 11)
                style = ColorStyle.Default;

            return new SolidColorBrush(_colors[(int)style]);
        }

        // Colors for each ColorStyle, in same order as enum
        private static readonly Color[] _colors = new Color[] {
            Colors.White,    // Header
            Colors.LightGray,     // SubHeader
            Colors.Cyan,     // SectionHeader
            Color.FromArgb(255,0,255,0),    // Default
            Colors.LightGray,     // Output
            Color.FromArgb(255,0,255,0),    // Help
            Color.FromArgb(255,0,255,0),    // Label
            Colors.White,    // Value
            Color.FromArgb(255,0,255,0),    // Pass
            Colors.Red,      // Failure
            Colors.Yellow,   // Warning
            Colors.Red       // Error
        };

        #endregion
    }
}
#endif
