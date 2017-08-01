// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The TestAttachment class represents a file attached to a TestResult,
    /// with an optional description.
    /// </summary>
    public class TestAttachment
    {
        /// <summary>
        /// Absolute file path to attachment file
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// User specifed description of attachment. May be null.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a TestAttachment class to represent a file attached to a test result.
        /// </summary>
        /// <param name="filePath">Absolute file path to attachment file</param>
        /// <param name="description">User specifed description of attachment. May be null.</param>
        public TestAttachment(string filePath, string description)
        {
            FilePath = filePath;
            Description = description;
        }
    }
}
