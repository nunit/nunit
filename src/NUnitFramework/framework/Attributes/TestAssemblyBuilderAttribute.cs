// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

namespace NUnit.Framework.Attributes 
{
    /// <summary>
    /// When declared on an assembly, specified which type should be used for discovery of tests within the assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class TestAssemblyBuilderAttribute : Attribute 
    {
        /// <summary>
        /// Gets the type that is used for discovery of tests in the assembly. The type specified must implement <see cref="NUnit.Framework.Api.ITestAssemblyBuilder"/>
        /// </summary>
        public Type AssemblyBuilderType { get; private set; }

        /// <summary>
        /// Constructs a TestAssemblyBuilderAttribute
        /// </summary>
        /// <param name="assemblyBuilderType"></param>
        public TestAssemblyBuilderAttribute(Type assemblyBuilderType) 
        {
            this.AssemblyBuilderType = assemblyBuilderType;
        }
    }
}