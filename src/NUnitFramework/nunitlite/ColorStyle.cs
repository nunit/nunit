// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

namespace NUnit.Common
{
    /// <summary>
    /// ColorStyle enumerates the various styles used in the console display
    /// </summary>
    public enum ColorStyle
    {
        /// <summary>
        /// Color for headers
        /// </summary>
        Header,
        /// <summary>
        /// Color for sub-headers
        /// </summary>
        SubHeader,
        /// <summary>
        /// Color for each of the section headers
        /// </summary>
        SectionHeader,
        /// <summary>
        /// The default color for items that don't fit into the other categories
        /// </summary>
        Default,
        /// <summary>
        /// Test output
        /// </summary>
        Output,
        /// <summary>
        /// Color for help text
        /// </summary>
        Help,
        /// <summary>
        /// Color for labels
        /// </summary>
        Label,
        /// <summary>
        /// Color for values, usually go beside labels
        /// </summary>
        Value,
        /// <summary>
        /// Color for passed tests
        /// </summary>
        Pass,
        /// <summary>
        /// Color for failed tests
        /// </summary>
        Failure,
        /// <summary>
        /// Color for warnings, ignored or skipped tests
        /// </summary>
        Warning,
        /// <summary>
        /// Color for errors and exceptions
        /// </summary>
        Error
    }
}
