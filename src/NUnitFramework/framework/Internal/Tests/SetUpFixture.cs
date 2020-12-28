// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// SetUpFixture extends TestSuite and supports
    /// Setup and TearDown methods.
    /// </summary>
    public class SetUpFixture : TestSuite, IDisposableFixture
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpFixture"/> class.
        /// </summary>
        public SetUpFixture(ITypeInfo type) : base(type)
        {
            this.Name = type.Namespace;
            if (this.Name == null)
                this.Name = "[default namespace]";
            int index = this.Name.LastIndexOf('.');
            if (index > 0)
                this.Name = this.Name.Substring(index + 1);

            OneTimeSetUpMethods = Reflect.GetMethodsWithAttribute<OneTimeSetUpAttribute>(TypeInfo.Type, true);
            OneTimeTearDownMethods = Reflect.GetMethodsWithAttribute<OneTimeTearDownAttribute>(TypeInfo.Type, true);

            CheckSetUpTearDownMethods(OneTimeSetUpMethods);
            CheckSetUpTearDownMethods(OneTimeTearDownMethods);
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="setUpFixture">The <see cref="SetUpFixture"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public SetUpFixture(SetUpFixture setUpFixture, ITestFilter filter)
            : base(setUpFixture, filter)
        {
        }

        #endregion

        #region Test Suite Overrides

        /// <summary>
        /// Gets the TypeInfo of the fixture used in running this test.
        /// </summary>
        public new ITypeInfo TypeInfo => base.TypeInfo!;

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new SetUpFixture(this, filter);
        }

        #endregion
    }
}
