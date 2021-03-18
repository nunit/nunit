// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

#nullable enable

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
            this.Assembly = assembly;
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
            this.Name = Path.GetFileName(assemblyNameOrPath);
        }

        /// <summary>
        /// Creates a copy of the given assembly with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="assembly">The <see cref="TestAssembly"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public TestAssembly(TestAssembly assembly, ITestFilter filter)
            : base(assembly as TestSuite, filter)
        {
            this.Name     = assembly.Name;
            this.Assembly = assembly.Assembly;
        }

        /// <summary>
        /// Gets the Assembly represented by this instance.
        /// </summary>
        public Assembly? Assembly { get; }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string TestType
        {
            get
            {
                return "Assembly";
            }
        }

        /// <summary>
        /// Get custom attributes specified on the assembly
        /// </summary>
        public override TAttr[] GetCustomAttributes<TAttr>(bool inherit)
        {
            return Assembly != null
                ? Assembly.GetAttributes<TAttr>()
                : new TAttr[0];
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

