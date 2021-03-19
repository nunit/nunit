// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
