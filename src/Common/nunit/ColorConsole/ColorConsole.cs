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

namespace NUnit.Common.ColorConsole
{
    /// <summary>
    /// Sets the console color in the constructor and resets it in the dispose
    /// </summary>
    public class ColorConsole : IDisposable
    {
        /// <summary>
        /// Gets or sets the Enabled flag, indicating whether color is 
        /// being used. This must be set at program startup.
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorConsole"/> class.
        /// </summary>
        /// <param name="style">The color style to use.</param>
        public ColorConsole(ColorStyle style)
        {
#if !SILVERLIGHT && !NETCF
            if (ColorConsole.Enabled)
                Console.ForegroundColor = GetColor(style);
#endif
        }

#if !SILVERLIGHT && !NETCF
        /// <summary>
        /// By using styles, we can keep everything consistent
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ConsoleColor GetColor(ColorStyle style)
        {
            switch (Console.BackgroundColor)
            {
                case ConsoleColor.White:
                    switch (style)
                    {
                        case ColorStyle.Header:
                            return ConsoleColor.DarkBlue;
                        case ColorStyle.SubHeader:
                            return ConsoleColor.DarkGray;
                        case ColorStyle.SectionHeader:
                            return ConsoleColor.DarkBlue;
                        case ColorStyle.Label:
                            return ConsoleColor.DarkGreen;
                        case ColorStyle.Value:
                            return ConsoleColor.Blue;
                        case ColorStyle.Pass:
                            return ConsoleColor.Green;
                        case ColorStyle.Failure:
                            return ConsoleColor.Red;
                        case ColorStyle.Warning:
                            return ConsoleColor.Yellow;
                        case ColorStyle.Error:
                            return ConsoleColor.Red;
                        case ColorStyle.Output:
                            return ConsoleColor.DarkGray;
                        case ColorStyle.Help:
                            return ConsoleColor.DarkGray;
                        case ColorStyle.Default:
                        default:
                            return ConsoleColor.Green;
                    }

                case ConsoleColor.Gray:
                    switch (style)
                    {
                        case ColorStyle.Header:
                            return ConsoleColor.White;
                        case ColorStyle.SubHeader:
                            return ConsoleColor.DarkGray;
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
                            return ConsoleColor.Red;
                        case ColorStyle.Output:
                            return ConsoleColor.DarkGray;
                        case ColorStyle.Help:
                            return ConsoleColor.DarkGray;
                        case ColorStyle.Default:
                        default:
                            return ConsoleColor.Green;
                    }

                default:
                    switch (style)
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
                            return ConsoleColor.Red;
                        case ColorStyle.Output:
                            return ConsoleColor.Gray;
                        case ColorStyle.Help:
                            return ConsoleColor.Green;
                        case ColorStyle.Default:
                        default:
                            return ConsoleColor.Green;
                    }
            }
        }
#endif

        #region Implementation of IDisposable

        /// <summary>
        /// If color is enabled, restores the console colors to their defaults
        /// </summary>
        public void Dispose()
        {
#if !SILVERLIGHT && !NETCF
            if (ColorConsole.Enabled)
                Console.ResetColor();
#endif
        }

        #endregion
    }
}