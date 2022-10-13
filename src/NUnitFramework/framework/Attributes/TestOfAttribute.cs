// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            : base(PropertyNames.TestOf, type.FullName!)
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
