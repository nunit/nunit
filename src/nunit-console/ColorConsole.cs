// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

namespace NUnit.ConsoleRunner
{
    public enum ColorStyle
    {
        /// <summary>
        /// Color for headers
        /// </summary>
        Header,
        /// <summary>
        /// Color for sub-headers
        /// </summary>
        SubHeader,
        /// <summary>
        /// Color for each of the section headers
        /// </summary>
        SectionHeader,
        /// <summary>
        /// The default color for items that don't fit into the other categories
        /// </summary>
        Default,
        /// <summary>
        /// Test output
        /// </summary>
        Output,
        /// <summary>
        /// Color for labels
        /// </summary>
        Label,
        /// <summary>
        /// Color for values, usually go beside labels
        /// </summary>
        Value,
        /// <summary>
        /// Color for passed tests
        /// </summary>
        Pass,
        /// <summary>
        /// Color for failed tests
        /// </summary>
        Failure,
        /// <summary>
        /// Color for warnings, ignored or skipped tests
        /// </summary>
        Warning,
        /// <summary>
        /// Color for errors and exceptions
        /// </summary>
        Error
    }

    /// <summary>
    /// Sets the console color in the constructor and resets it in the dispose
    /// </summary>
    public class ColorConsole : IDisposable
    {
        /// <summary>
        /// Gets or sets the options. This must be set at program startup
        /// </summary>
        public static ConsoleOptions Options { private get; set; }

        public ColorConsole( ColorStyle style )
        {
            if ( Options != null && Options.Color )
                Console.ForegroundColor = GetColor( style );
        }

        /// <summary>
        /// Writes the value with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public static void Write( ColorStyle style, string value )
        {
            using ( new ColorConsole( style ) )
            {
                Console.Write( value );
            }
        }

        /// <summary>
        /// Writes the value with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        public static void WriteLine( ColorStyle style, string value )
        {
            using (new ColorConsole(style))
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Writes the label and the option that goes with it and optionally writes a new line.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="option">The option.</param>
        /// <param name="writeLine">if set to <c>true</c> [write line].</param>
        public static void WriteLabel( string label, string option, bool writeLine )
        {
            Write( ColorStyle.Label, label );
            Write( ColorStyle.Value, option  );
            if (writeLine)
                Console.WriteLine();
        }

        /// <summary>
        /// By using styles, we can keep everything consistent
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ConsoleColor GetColor( ColorStyle style )
        {
            switch ( style )
            {
                case ColorStyle.Header:
                    return ConsoleColor.White;
                case ColorStyle.SubHeader:
                    return ConsoleColor.Gray;
                case ColorStyle.SectionHeader:
                    return ConsoleColor.Cyan;
                case ColorStyle.Label:
                    return ConsoleColor.Green;
                case ColorStyle.Value:
                    return ConsoleColor.White;
                case ColorStyle.Pass:
                    return ConsoleColor.Green;
                case ColorStyle.Failure:
                    return ConsoleColor.Red;
                case ColorStyle.Warning:
                    return ConsoleColor.Yellow;
                case ColorStyle.Error:
                    return ConsoleColor.DarkRed;
                case ColorStyle.Output:
                    return ConsoleColor.Gray;
                case ColorStyle.Default:
                default:
                    return ConsoleColor.Green;
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// If color is enabled, restores the console colors to their defaults
        /// </summary>
        public void Dispose()
        {
            if (Options != null && Options.Color)
                Console.ResetColor();
        }

        #endregion
    }
}