// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtainingn
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

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// ParallelTestProcessRunner runs tests using separate
    /// processes for each assembly and runs them in parallel
    /// </summary>
    public class ParallelTestProcessRunner : AggregatingTestRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelTestProcessRunner"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="package">The package.</param>
        public ParallelTestProcessRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region AggregatingTestRunner Overrides

        protected override ITestEngineRunner CreateRunner(TestPackage package)
        {
            return new ProcessRunner(Services, package);
        }

        protected override int GetLevelOfParallelism()
        {
            return Math.Max(Environment.ProcessorCount, 2);
        }

        #endregion
    }
}
