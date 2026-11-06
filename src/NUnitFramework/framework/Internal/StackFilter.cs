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
        private const string DEFAULT_NUNIT_FRAMEWORK_STACK_PATTERN = @" (NUnit\.Framework\.|NUnitLite\.)(?!Tests\.(?!TestUtilities\.))";
        private const string DEFAULT_SYSTEM_STACK_PATTERN = @" InvokeStub_| System\.(Reflection|Runtime|RuntimeMethodHandle|Threading)\.| System\.Environment\.[A-Za-z_]*StackTrace|--- .* ---";

#if NETFRAMEWORK
        private const string AsyncStateMachineMethodPattern = @"\.<(.*)>d__[0-9]+(`[0-9]+)?\.MoveNext\(\)";
        private const string AsyncStateMachineMethodReplacementPattern = @".$1()";

        private static readonly Regex AsyncStateMachineMethodRegex = new(AsyncStateMachineMethodPattern, RegexOptions.Compiled);
#endif

        /// <summary>
        /// Single instance of our default filter
        /// </summary>
        /// <remarks>
        /// Kept as non-readonly for backwards compatibility. Althought it should not be written to, some users may have done so.
        /// If we make it readonly, they will get a runtime error.
        /// </remarks>
        public static StackFilter DefaultFilter = new();

        private readonly Regex? _nunitStackRegex;
        private readonly Regex? _systemStackRegex;

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <remarks>
        /// Parameter names retained for backwards compatibility.
        /// The patterns are no longer restricted to top and bottom of the stack, but are used to filter any lines in the stack trace.
        /// </remarks>
        /// <param name="topOfStackPattern">Regex pattern used to delete matching lines from the stack trace.</param>
        /// <param name="bottomOfStackPattern">Additional regex pattern used to delete matching lines from the stack trace.</param>
        public StackFilter(string? topOfStackPattern, string? bottomOfStackPattern)
        {
            if (topOfStackPattern is not null)
                _nunitStackRegex = new Regex(topOfStackPattern, RegexOptions.Compiled);
            if (bottomOfStackPattern is not null)
                _systemStackRegex = new Regex(bottomOfStackPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <remarks>
        /// Constructor overload and parameter names retained for backwards compatibility.
        /// The patterns are no longer restricted to top and bottom of the stack, but are used to filter any lines in the stack trace.
        /// </remarks>
        /// <param name="topOfStackPattern">Regex pattern used to delete matching lines from the stack trace.</param>
        public StackFilter(string topOfStackPattern)
            : this(topOfStackPattern, DEFAULT_SYSTEM_STACK_PATTERN)
        {
        }

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        public StackFilter()
            : this(DEFAULT_NUNIT_FRAMEWORK_STACK_PATTERN, DEFAULT_SYSTEM_STACK_PATTERN)
        {
        }

        /// <summary>
        /// Filters a raw stack trace and returns the result.
        /// </summary>
        /// <param name="rawTrace">The original stack trace</param>
        /// <returns>A filtered stack trace</returns>
        public string? Filter(string? rawTrace)
        {
            if (rawTrace is null)
                return null;

            var sr = new StringReader(rawTrace);
            var sw = new StringWriter();

            try
            {
                for (var line = sr.ReadLine(); line is not null; line = sr.ReadLine())
                {
                    if (_nunitStackRegex?.IsMatch(line) == true)
                        continue;

                    if (_systemStackRegex?.IsMatch(line) == true)
                        continue;

#if NETFRAMEWORK
                    line = AsyncStateMachineMethodRegex.Replace(line, AsyncStateMachineMethodReplacementPattern);
#endif

                    sw.WriteLine(line);
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
