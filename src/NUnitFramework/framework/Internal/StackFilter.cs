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
        private const string DEFAULT_NUNIT_FRAMEWORK_STACK_PATTERN = @"^ +\w+:? (?:NUnit\.Framework|NUnitLite)\.";

        // Infrastructure frames: reflection invocation, async/await machinery and thread (pool) start-up.
        // Deliberately restricted to known plumbing types, so frames of BCL APIs called by user code are kept.
        // Mono wrapper frames are runtime plumbing, except dynamic-method (compiled code such as expression lambdas)
        // and synchronized (wraps a [MethodImpl(MethodImplOptions.Synchronized)] user method).
        // Wrapper frames where an exception was thrown are kept, as are all system frames at the top of the stack trace.
        private const string DEFAULT_SYSTEM_STACK_PATTERN =
            @"^ +\w+:? \(wrapper (?!dynamic-method\)|synchronized\))[\w-]+\) " +
            @"|^ +\w+:? (?:" +
                @"InvokeStub_" +
                @"|System\.RuntimeMethodHandle\." +
                @"|System\.Reflection\.(?:Runtime(?:Method|Constructor)Info|MethodBase|\w*Invoker\w*)\b" +
                @"|System\.Runtime\.ExceptionServices\.ExceptionDispatchInfo\." +
                @"|System\.Runtime\.CompilerServices\.(?:\w*Awaiter|\w*Awaitable|Async\w*MethodBuilder\w*)\b" +
                @"|System\.Runtime\.Remoting\." +
                @"|System\.Threading\.(?:ExecutionContext|\w*SynchronizationContext|Thread|ThreadHelper|\w*ThreadPool\w*|QueueUserWorkItemCallback\w*)\b" +
                @"|System\.Threading\.Tasks\.(?:Task|TaskScheduler|ThreadPoolTaskScheduler|\w*TaskContinuation)\b" +
            @")|^--- .+ ---$";

        // The frames of the NUnit test execution pipeline. Nothing below these is user code.
        private const string NUNIT_RUNNER_STACK_PATTERN = @"^ +\w+:? NUnit\.Framework\.Internal\.Commands\.";

#if NETFRAMEWORK
        // .NET Framework writes the state machine as a nested type with '.', Mono with '+' and its generic parameters,
        // e.g. "Fixture.<Method>d__8`1.MoveNext()" and "Fixture+<Method>d__8`1[T].MoveNext ()".
        // The space before the parentheses is kept, so the frame keeps the runtime's formatting.
        private const string AsyncStateMachineMethodName = "MoveNext";
        private const string AsyncStateMachineMethodPattern = $@"[.+]<(.*)>d__[0-9]+(?:`[0-9]+)?(?:\[[^\]]*\])?\.{AsyncStateMachineMethodName}( *)\(\)";
        private const string AsyncStateMachineMethodReplacementPattern = @".$1$2()";

        private static readonly Regex AsyncStateMachineMethodRegex = new(AsyncStateMachineMethodPattern, RegexOptions.Compiled);
#endif

        /// <summary>
        /// Single instance of our default filter
        /// </summary>
        /// <remarks>
        /// Kept as non-readonly field for backwards compatibility.
        /// If we make it readonly or convert to a property, they would get a runtime error.
        /// </remarks>
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static StackFilter DefaultFilter = new();
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private readonly Regex? _nunitStackRegex;
        private readonly Regex? _systemStackRegex;
        private readonly Regex? _runnerStackRegex;

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>StackFilter.Create(topOfStackPattern, null, bottomOfStackPattern)</c>.
        /// Despite its name, <paramref name="topOfStackPattern"/> is not restricted to the top of the stack trace.
        /// </remarks>
        /// <param name="topOfStackPattern">Regex pattern used to delete matching lines anywhere in the stack trace.</param>
        /// <param name="bottomOfStackPattern">
        /// Regex pattern for the first line at which the stack trace is cut off,
        /// or <see langword="null"/> to not cut off the stack trace.
        /// </param>
        [Obsolete("Use StackFilter.Create(nunitStackPattern: topOfStackPattern, systemStackPattern: null, runnerStackPattern: bottomOfStackPattern) instead.")]
        public StackFilter(string? topOfStackPattern, string? bottomOfStackPattern)
            : this(topOfStackPattern, null, bottomOfStackPattern)
        {
        }

        /// <summary>
        /// Construct a stack filter instance
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>StackFilter.Create(topOfStackPattern)</c>.
        /// Despite its name, the pattern is not restricted to the top of the stack trace.
        /// </remarks>
        /// <param name="topOfStackPattern">Regex pattern used to delete matching lines anywhere in the stack trace.</param>
        [Obsolete("Use StackFilter.Create(nunitStackPattern) instead.")]
        public StackFilter(string topOfStackPattern)
            : this(topOfStackPattern, DEFAULT_SYSTEM_STACK_PATTERN, NUNIT_RUNNER_STACK_PATTERN)
        {
        }

        /// <summary>
        /// Construct a stack filter instance that removes NUnit frames and infrastructure frames,
        /// and cuts off the stack trace at the NUnit test execution pipeline.
        /// </summary>
        public StackFilter()
            : this(DEFAULT_NUNIT_FRAMEWORK_STACK_PATTERN, DEFAULT_SYSTEM_STACK_PATTERN, NUNIT_RUNNER_STACK_PATTERN)
        {
        }

        private StackFilter(string? nunitStackPattern, string? systemStackPattern, string? runnerStackPattern)
        {
            if (nunitStackPattern is not null)
                _nunitStackRegex = new Regex(nunitStackPattern, RegexOptions.Compiled);
            if (systemStackPattern is not null)
                _systemStackRegex = new Regex(systemStackPattern, RegexOptions.Compiled);
            if (runnerStackPattern is not null)
                _runnerStackRegex = new Regex(runnerStackPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates a stack filter that uses a custom pattern for NUnit frames,
        /// combined with NUnit's default patterns for infrastructure frames and the test execution pipeline.
        /// </summary>
        /// <param name="nunitStackPattern">
        /// Regex pattern used to delete matching lines anywhere in the stack trace,
        /// or <see langword="null"/> to keep all NUnit frames.
        /// </param>
        /// <returns>A new <see cref="StackFilter"/>.</returns>
        public static StackFilter Create(string? nunitStackPattern)
            => new(nunitStackPattern, DEFAULT_SYSTEM_STACK_PATTERN, NUNIT_RUNNER_STACK_PATTERN);

        /// <summary>
        /// Creates a stack filter with custom patterns.
        /// </summary>
        /// <remarks>
        /// Lines are processed from the top of the stack trace down:
        /// <list type="number">
        /// <item>A line matching <paramref name="runnerStackPattern"/> ends the trace; it and all lines below it are removed.</item>
        /// <item>A line matching <paramref name="nunitStackPattern"/> is removed.</item>
        /// <item>A line matching <paramref name="systemStackPattern"/> is removed, unless all lines above it also match
        /// <paramref name="systemStackPattern"/>, as those lines show where the exception was thrown.</item>
        /// </list>
        /// The result ends with a newline.
        /// If no lines remain, the original stack trace is returned, with a newline appended if it does not end with one.
        /// </remarks>
        /// <param name="nunitStackPattern">
        /// Regex pattern used to delete matching lines anywhere in the stack trace,
        /// or <see langword="null"/> to not delete any lines with it.
        /// </param>
        /// <param name="systemStackPattern">
        /// Regex pattern used to delete matching infrastructure lines,
        /// or <see langword="null"/> to not delete any lines with it.
        /// </param>
        /// <param name="runnerStackPattern">
        /// Regex pattern for the first line at which the stack trace is cut off,
        /// or <see langword="null"/> to not cut off the stack trace.
        /// </param>
        /// <returns>A new <see cref="StackFilter"/>.</returns>
        public static StackFilter Create(string? nunitStackPattern, string? systemStackPattern, string? runnerStackPattern)
            => new(nunitStackPattern, systemStackPattern, runnerStackPattern);

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
            bool atTopOfStack = true;
            bool anyLineWritten = false;

            try
            {
                for (var line = sr.ReadLine(); line is not null; line = sr.ReadLine())
                {
                    // Everything below the NUnit execution pipeline is runner/host code.
                    if (_runnerStackRegex?.IsMatch(line) == true)
                        break;

                    if (_nunitStackRegex?.IsMatch(line) == true)
                    {
                        atTopOfStack = false;
                        continue;
                    }

                    if (_systemStackRegex?.IsMatch(line) == true)
                    {
                        // Keep system frames at the top: they show where the exception was thrown.
                        if (!atTopOfStack)
                            continue;
                    }
                    else
                    {
                        atTopOfStack = false;
                    }

#if NETFRAMEWORK
                    if (line.Contains(AsyncStateMachineMethodName))
                        line = AsyncStateMachineMethodRegex.Replace(line, AsyncStateMachineMethodReplacementPattern);
#endif

                    sw.WriteLine(line);
                    anyLineWritten = true;
                }
            }
            catch (Exception)
            {
                return WithTrailingNewLine(rawTrace);
            }

            // If nothing is left, the exception originated inside NUnit itself. Show the full trace so it can be diagnosed.
            if (!anyLineWritten)
                return WithTrailingNewLine(rawTrace);

            return sw.ToString();
        }

        // A filtered stack trace always ends with a newline, so the unfiltered fallback does too.
        private static string WithTrailingNewLine(string trace)
        {
            return trace.Length == 0 || trace[trace.Length - 1] == '\n'
                ? trace
                : trace + Environment.NewLine;
        }
    }
}
