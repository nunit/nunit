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
using System.Text.RegularExpressions;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// StackFilter class is used to remove internal NUnit
    /// entries from a stack trace so that the resulting
    /// trace provides better information about the test.
    /// </summary>
    public static class StackFilter
    {
        private static readonly Regex assertOrAssumeRegex = new Regex(
            @" NUnit\.Framework\.Ass(ert|ume)\.");

        /// <summary>
        /// Filters a raw stack trace and returns the result.
        /// </summary>
        /// <param name="rawTrace">The original stack trace</param>
        /// <returns>A filtered stack trace</returns>
        public static string Filter(string rawTrace)
        {
            if (rawTrace == null) return null;

            StringReader sr = new StringReader(rawTrace);
            StringWriter sw = new StringWriter();

            try
            {
                string line;
                // Skip past any Assert or Assume lines
                while ((line = sr.ReadLine()) != null && assertOrAssumeRegex.IsMatch(line))
                    /*Skip*/
                    ;

                // Copy lines down to the line that invoked the failing method.
                // This is actually only needed for the compact framework, but 
                // we do it on all platforms for simplicity. Desktop platforms
                // won't have any System.Reflection lines.
                while (line != null && line.IndexOf(" System.Reflection.") < 0)
                {
                    sw.WriteLine(line.Trim());
                    line = sr.ReadLine();
                }
            }
            catch (Exception)
            {
                return rawTrace;
            }

            return sw.ToString();
        }
    }
}
