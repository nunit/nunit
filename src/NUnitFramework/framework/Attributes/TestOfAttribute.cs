// **********************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2014 Charlie Poole, Rob Prouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// **********************************************************************************

#nullable enable

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Indicates the method or class the assembly, test fixture or test method is testing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class TestOfAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOfAttribute"/> class.
        /// </summary>
        /// <param name="type">The type that is being tested.</param>
        public TestOfAttribute(Type type)
            : base(PropertyNames.TestOf, type.FullName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOfAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The type that is being tested.</param>
        public TestOfAttribute(string typeName) 
            : base(PropertyNames.TestOf, typeName)
        {
        }
    }
}
