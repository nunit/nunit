// ***********************************************************************
// Copyright (c) 2007–2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestFixture is a surrogate for a user test fixture class,
    /// containing one or more tests.
    /// </summary>
    public class TestFixture : TestSuite, IDisposableFixture
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="arguments">Arguments used to instantiate the test fixture, or null if none used</param>
        public TestFixture(Type fixtureType, object[] arguments = null) : base(fixtureType, arguments)
        {
            SetUpMethods = Reflect.GetMethodsWithAttribute(Type, typeof(SetUpAttribute), true);
            TearDownMethods = Reflect.GetMethodsWithAttribute(Type, typeof(TearDownAttribute), true);
            OneTimeSetUpMethods = Reflect.GetMethodsWithAttribute(Type, typeof(OneTimeSetUpAttribute), true);
            OneTimeTearDownMethods = Reflect.GetMethodsWithAttribute(Type, typeof(OneTimeTearDownAttribute), true);

            CheckSetUpTearDownMethods(OneTimeSetUpMethods);
            CheckSetUpTearDownMethods(OneTimeTearDownMethods);
            CheckSetUpTearDownMethods(SetUpMethods);
            CheckSetUpTearDownMethods(TearDownMethods);
        }

        #endregion
    }
}
