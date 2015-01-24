// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

using System.IO;
using System.Xml;

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// Common interface for objects that process and write out test results
    /// </summary>
    public interface IResultWriter
    {
        /// <summary>
        /// Checks if the output path is writable. If the output is not
        /// writable, this method should throw an exception.
        /// </summary>
        /// <param name="outputPath"></param>
        void CheckWritability(string outputPath);

        /// <summary>
        /// Writes result to the specified output path.
        /// </summary>
        /// <param name="resultNode">XmlNode for the result</param>
        /// <param name="outputPath">Path to which it should be written</param>
        void WriteResultFile(XmlNode resultNode, string outputPath);

        /// <summary>
        /// Writes result to a TextWriter.
        /// </summary>
        /// <param name="resultNode">XmlNode for the result</param>
        /// <param name="writer">TextWriter to which it should be written</param>
        void WriteResultFile(XmlNode resultNode, TextWriter writer);
    }
}
