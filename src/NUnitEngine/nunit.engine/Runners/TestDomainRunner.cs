// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using NUnit.Engine.Services;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// TestDomainRunner loads and runs tests in a separate
    /// domain whose lifetime it controls.
    /// </summary>
    public class TestDomainRunner : DirectTestRunner
    {
        public TestDomainRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region DirectTestRunner Overrides

        protected override TestEngineResult LoadPackage()
        {
            this.TestDomain = Services.DomainManager.CreateDomain(TestPackage);

            return base.LoadPackage();
        }

        /// <summary>
        /// Unload any loaded TestPackage as well as the AppDomain.
        /// </summary>
        public override void UnloadPackage()
        {
            if (this.TestDomain != null)
            {
                Services.DomainManager.Unload(this.TestDomain);
                this.TestDomain = null;
            }
        }

        #endregion
    }
}
