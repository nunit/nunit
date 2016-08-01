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

using System.Collections.Generic;

namespace NUnit.Common
{
    /// <summary>
    /// TestNameParser is used to parse the arguments to the 
    /// -run option, separating testnames at the correct point.
    /// </summary>
    public class TestNameParser
    {
        /// <summary>
        /// Parse the -run argument and return an array of argument
        /// </summary>
        /// <param name="argument">argument</param>
        /// <returns></returns>
        public static string[] Parse(string argument)
        {
            List<string> list = new List<string>();

            int index = 0;
            while (index < argument.Length)
            {
                string name = GetTestName(argument, ref index);
                if (name != null && name != string.Empty)
                    list.Add(name);
            }

            return list.ToArray();
        }

        private static string GetTestName(string argument, ref int index)
        {
            int separator = GetSeparator(argument, index);
            string result;

            if (separator >= 0)
            {
                result = argument.Substring(index, separator - index).Trim();
                index = separator + 1;
            }
            else
            {
                result = argument.Substring(index).Trim();
                index = argument.Length;
            }

            return result;
        }

        private static int GetSeparator(string argument, int index)
        {
            int nest = 0;

            while (index < argument.Length)
            {
                switch (argument[index])
                {
                    case ',':
                        if (nest == 0)
                            return index;
                        break;

                    case '"':
                        while (++index < argument.Length && argument[index] != '"')
                            ;
                        break;

                    case '(':
                    case '<':
                        nest++;
                        break;

                    case ')':
                    case '>':
                        nest--;
                        break;
                }

                index++;
            }

            return -1;
        }
    }
}
