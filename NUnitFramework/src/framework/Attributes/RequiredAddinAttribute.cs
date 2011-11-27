// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

#if !NUNITLITE
using System;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// RequiredAddinAttribute may be used to indicate the names of any addins
    /// that must be present in order to run some or all of the tests in an
    /// assembly. If the addin is not loaded, the entire assembly is marked
    /// as NotRunnable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple=true, Inherited=false)]
    public class RequiredAddinAttribute : TestModificationAttribute, IApplyToTest
    {
        private string requiredAddin;
        private bool isAddinAvailable;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RequiredAddinAttribute"/> class.
        /// </summary>
        /// <param name="requiredAddin">The required addin.</param>
        public RequiredAddinAttribute(string requiredAddin)
        {
            this.requiredAddin = requiredAddin;
            this.isAddinAvailable = false;

            foreach (NUnit.Framework.Extensibility.Addin addin in CoreExtensions.Host.AddinRegistry.Addins)
                if (addin.Name == requiredAddin && addin.Status == NUnit.Framework.Extensibility.AddinStatus.Loaded)
                    this.isAddinAvailable = true;
        }

        /// <summary>
        /// Gets the name of required addin.
        /// </summary>
        /// <value>The required addin name.</value>
        public string RequiredAddin
        {
            get { return requiredAddin; }
        }

        #region IApplyToTest members

        /// <summary>
        /// Modifies the test to be skipped if the addin is not available.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(ITest test)
        {
            if (test.RunState != RunState.NotRunnable && !isAddinAvailable)
            {
                test.RunState = RunState.NotRunnable;
                test.Properties.Set(PropertyNames.SkipReason, string.Format("Required addin {0} not available", requiredAddin));
            }
        }

        #endregion
    }
}
#endif