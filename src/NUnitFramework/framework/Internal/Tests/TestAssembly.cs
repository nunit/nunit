// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestAssembly is a TestSuite that represents the execution
    /// of tests in a managed assembly.
    /// </summary>
    public class TestAssembly : TestSuite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAssembly"/> class
        /// specifying the Assembly and the suite name.
        /// </summary>
        /// <param name="assembly">The assembly this test represents.</param>
        /// <param name="assemblyNameOrPath">
        /// This becomes the full name of the suite and the filename part is used as the suite name.
        /// </param>
        public TestAssembly(Assembly assembly, string assemblyNameOrPath)
            : this(assemblyNameOrPath)
        {
            Assembly = assembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAssembly"/> class
        /// specifying the suite name for an assembly that could not be loaded.
        /// </summary>
        /// <param name="assemblyNameOrPath">
        /// This becomes the full name of the suite and the filename part is used as the suite name.
        /// </param>
        public TestAssembly(string assemblyNameOrPath) : base(assemblyNameOrPath)
        {
            Name = Path.GetFileName(assemblyNameOrPath);
        }

        /// <summary>
        /// Creates a copy of the given assembly with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="assembly">The <see cref="TestAssembly"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public TestAssembly(TestAssembly assembly, ITestFilter filter)
            : base(assembly, filter)
        {
            Name     = assembly.Name;
            Assembly = assembly.Assembly;
        }

        /// <summary>
        /// Gets the Assembly represented by this instance.
        /// </summary>
        public Assembly? Assembly { get; }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string TestType => "Assembly";

        /// <summary>
        /// Get custom attributes specified on the assembly
        /// </summary>
        public override TAttr[] GetCustomAttributes<TAttr>(bool inherit)
        {
            return Assembly is not null
                ? Assembly.GetAttributes<TAttr>()
                : Array.Empty<TAttr>();
        }

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new TestAssembly(this, filter);
        }
    }
}

