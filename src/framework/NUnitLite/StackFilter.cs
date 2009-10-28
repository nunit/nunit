// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.IO;

namespace NUnit.Framework
{
    /// <summary>
    /// Static class used to filter stack entries before they are displayed
    /// </summary>
    public class StackFilter
    {
        /// <summary>
        /// Filter a raw stack trace
        /// </summary>
        /// <param name="rawTrace">The original trace</param>
        /// <returns>The filtered trace</returns>
        public static string Filter(string rawTrace)
        {
            if (rawTrace == null) return null;

            StringReader sr = new StringReader(rawTrace);
            StringWriter sw = new StringWriter();

            string line;
            while ((line = sr.ReadLine()) != null && line.IndexOf("NUnit.Framework.Assert")>=0) 
                /*Skip*/;

            while (line != null)
            {
                sw.WriteLine(line);
                line = sr.ReadLine();
            }

            return sw.ToString();;
        }
    }
}
