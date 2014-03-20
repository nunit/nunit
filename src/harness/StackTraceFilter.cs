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

namespace NUnit.Framework.TestHarness
{
    using System;
    using System.IO;

    /// <summary>
    /// Summary description for StackTraceFilter.
    /// </summary>
    public class StackTraceFilter
    {
        public static string Filter(string stack)
        {
            if (stack == null) return null;
            StringWriter sw = new StringWriter();
            StringReader sr = new StringReader(stack);

            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!FilterLine(line))
                        sw.WriteLine(line.Trim());
                }
            }
            catch (Exception)
            {
                return stack;
            }
            return sw.ToString();
        }

        static bool FilterLine(string line)
        {
            string[] patterns = new string[]
            {
                "NUnit.Framework.Assert" 
            };

            for (int i = 0; i < patterns.Length; i++)
            {
                if (line.IndexOf(patterns[i]) > 0)
                    return true;
            }

            return false;
        }

    }
}
