// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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

using NUnit.Common;

namespace NUnitLite
{
    /// <summary>
    /// NUnitLiteOptions encapsulates the option settings for NUnitLite.
    /// Currently, there are no additional options beyond those common
    /// options that are shared with nunit3-console. If NUnitLite should
    /// acquire some unique options, they should be placed here.
    /// </summary>
    public class NUnitLiteOptions : CommandLineOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NUnitLiteOptions(params string[] args) : base(false, args) { }

        public NUnitLiteOptions(bool requireInputFile, params string[] args) : base(requireInputFile, args) { }

        // Currently used only by test
        internal NUnitLiteOptions(IDefaultOptionsProvider provider, params string[] args) : base(provider, false, args) { }
    }
}
