// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// StackFilter class is used to remove internal NUnit
    /// entries from a stack trace so that the resulting
    /// trace provides better information about the test.
    /// </summary>
    public class StackFilter
    {
        private const string DEFAULT_TOP_OF_STACK_PATTERN = @" NUnit\.Framework\.(Assert|Assume|Warn|CollectionAssert|StringAssert|FileAssert|DirectoryAssert)\.";
        private const string DEFAULT_BOTTOM_OF_STACK_PATTERN = @" System\.(Reflection|RuntimeMethodHandle|Threading\.ExecutionContext)\.";

        /// <summary>
        /// Single instance of our default filter
        /// </summary>
        public static StackFilter DefaultFilter = new();

        private readonly Regex? _topOfStackRegex;
        private readonly Regex? _bottomOfStackRegex;

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <param name="topOfStackPattern">Regex pattern used to delete lines from the top of the stack</param>
        /// <param name="bottomOfStackPattern">Regex pattern used to delete lines from the bottom of the stack</param>
        public StackFilter(string? topOfStackPattern, string? bottomOfStackPattern)
        {
            if (topOfStackPattern is not null)
                _topOfStackRegex = new Regex(topOfStackPattern, RegexOptions.Compiled);
            if (bottomOfStackPattern is not null)
                _bottomOfStackRegex = new Regex(bottomOfStackPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <param name="topOfStackPattern">Regex pattern used to delete lines from the top of the stack</param>
        public StackFilter(string topOfStackPattern)
            : this(topOfStackPattern, DEFAULT_BOTTOM_OF_STACK_PATTERN) { }

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        public StackFilter()
            : this(DEFAULT_TOP_OF_STACK_PATTERN, DEFAULT_BOTTOM_OF_STACK_PATTERN) { }

        /// <summary>
        /// Filters a raw stack trace and returns the result.
        /// </summary>
        /// <param name="rawTrace">The original stack trace</param>
        /// <returns>A filtered stack trace</returns>
        public string? Filter(string? rawTrace)
        {
            if (rawTrace is null) return null;

            StringReader sr = new StringReader(rawTrace);
            StringWriter sw = new StringWriter();

            try
            {
                var line = sr.ReadLine();

                if (_topOfStackRegex is not null)
                {
                    // First, skip past any Assert, Assume or MultipleAssertBlock lines
                    while (line is not null && _topOfStackRegex.IsMatch(line))
                        line = sr.ReadLine();
                }

                // Copy lines down to the line that invoked the failing method.
                // This is actually only needed for the compact framework, but
                // we do it on all platforms for simplicity. Desktop platforms
                // won't have any System.Reflection lines.
                while (line is not null)
                {
                    if (_bottomOfStackRegex is not null && _bottomOfStackRegex.IsMatch(line))
                        break;

                    sw.WriteLine(line);
                    line = sr.ReadLine();
                }
            }
            catch (Exception)
            {
                return rawTrace;
            }

            return sw.ToString();
        }
    }
}
