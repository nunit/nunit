// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Common;

namespace NUnitLite
{
    /// <summary>
    /// The AutoRun class is used by executable test
    /// assemblies to control their own execution.
    ///
    /// Call it from your executable test like this:
    ///    new AutoRun().Execute(args);
    /// The arguments can be those passed into your exe
    /// or constructed for the purpose in your code.
    ///
    /// If the tests are in a dll, you can write a stub
    /// executable that runs them like this:
    ///    new Autorun().Execute(testAssembly, args);
    ///
    /// When running tests compiled against the .NET Standard
    /// framework, the methods above are not available.
    /// Run your tests like this:
    ///    new AutoRun().Execute(testAssembly, args, output, input);
    /// Where output is an ExtendedTextWriter (normally a
    /// ColorConsoleWriter) and input is usually Console.In
    /// and is used by the --wait option.
    /// </summary>
    public class AutoRun
    {
        private readonly Assembly _testAssembly;

        /// <summary>
        /// Constructor for use where GetCallingAssembly is not
        /// available, requiring the assembly to be passed.
        /// </summary>
        /// <param name="testAssembly">The test assembly</param>
        public AutoRun(Assembly testAssembly)
        {
            _testAssembly = testAssembly;
        }

        /// <summary>
        /// Default Constructor, only used where GetCallingAssembly is available
        /// </summary>
        public AutoRun() : this(Assembly.GetCallingAssembly()) { }

        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments.
        /// </summary>
        /// <param name="args">arguments for NUnitLite to use</param>
        public int Execute(string[] args)
        {
            return new TextRunner(_testAssembly).Execute(args);
        }

        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments, a test assembly a writer
        /// and a reader. For use in builds for runtimes
        /// that don't support Assembly.GetCallingAssembly().
        /// </summary>
        /// <param name="args">Arguments passed to NUnitLite</param>
        /// <param name="writer">An ExtendedTextWriter to which output will be written</param>
        /// <param name="reader">A TextReader used when waiting for input</param>
        public int Execute(string[] args, ExtendedTextWriter writer, TextReader reader)
        {
            return new TextRunner(_testAssembly).Execute(writer, reader, args);
        }
    }
}
