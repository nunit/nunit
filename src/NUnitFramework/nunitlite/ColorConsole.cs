// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Common
{
    /// <summary>
    /// Sets the console color in the constructor and resets it in the dispose
    /// </summary>
    public class ColorConsole : IDisposable
    {
        private readonly ConsoleColor _originalColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorConsole"/> class.
        /// </summary>
        /// <param name="style">The color style to use.</param>
        public ColorConsole(ColorStyle style)
        {
            _originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColor(style);
        }

        /// <summary>
        /// By using styles, we can keep everything consistent
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ConsoleColor GetColor(ColorStyle style)
        {
            ConsoleColor color = GetColorForStyle(style);
            ConsoleColor bg = Console.BackgroundColor;

            if (color == bg || color == ConsoleColor.Red && bg == ConsoleColor.Magenta)
            {
                return bg == ConsoleColor.Black
                    ? ConsoleColor.White
                    : ConsoleColor.Black;
            }

            return color;
        }

        private static ConsoleColor GetColorForStyle(ColorStyle style)
        {
            switch (Console.BackgroundColor)
            {
                case ConsoleColor.White:
                    switch (style)
                    {
                        case ColorStyle.Header:
                            return ConsoleColor.Black;
                        case ColorStyle.SubHeader:
                            return ConsoleColor.Black;
                        case ColorStyle.SectionHeader:
                            return ConsoleColor.Blue;
                        case ColorStyle.Label:
                            return ConsoleColor.Black;
                        case ColorStyle.Value:
                            return ConsoleColor.Blue;
                        case ColorStyle.Pass:
                            return ConsoleColor.Green;
                        case ColorStyle.Failure:
                            return ConsoleColor.Red;
                        case ColorStyle.Warning:
                            return ConsoleColor.Black;
                        case ColorStyle.Error:
                            return ConsoleColor.Red;
                        case ColorStyle.Output:
                            return ConsoleColor.Black;
                        case ColorStyle.Help:
                            return ConsoleColor.Black;
                        case ColorStyle.Default:
                        default:
                            return ConsoleColor.Black;
                    }

                case ConsoleColor.Cyan:
                case ConsoleColor.Green:
                case ConsoleColor.Red:
                case ConsoleColor.Magenta:
                case ConsoleColor.Yellow:
                    switch (style)
                    {
                        case ColorStyle.Header:
                            return ConsoleColor.Black;
                        case ColorStyle.SubHeader:
                            return ConsoleColor.Black;
                        case ColorStyle.SectionHeader:
                            return ConsoleColor.Blue;
                        case ColorStyle.Label:
                            return ConsoleColor.Black;
                        case ColorStyle.Value:
                            return ConsoleColor.Black;
                        case ColorStyle.Pass:
                            return ConsoleColor.Black;
                        case ColorStyle.Failure:
                            return ConsoleColor.Red;
                        case ColorStyle.Warning:
                            return ConsoleColor.Yellow;
                        case ColorStyle.Error:
                            return ConsoleColor.Red;
                        case ColorStyle.Output:
                            return ConsoleColor.Black;
                        case ColorStyle.Help:
                            return ConsoleColor.Black;
                        case ColorStyle.Default:
                        default:
                            return ConsoleColor.Black;
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

        #region Implementation of IDisposable

        /// <summary>
        /// If color is enabled, restores the console colors to their defaults
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = _originalColor;
        }

        #endregion
    }
}