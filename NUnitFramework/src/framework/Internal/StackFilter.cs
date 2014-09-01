// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// StackFilter class is used to remove internal NUnit
    /// entries from a stack trace so that the resulting
    /// trace provides better information about the test.
    /// </summary>
    public class StackFilter
    {
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
                // TODO: Handle Assume and any other verbs
                // Best way is probably to check first line and
                // see where the exception was thrown, then
                // discard all leading lines within the same class.
                while ((line = sr.ReadLine()) != null && line.IndexOf("at NUnit.Framework.Assert.") >= 0)
                    /*Skip*/
                    ;

                while (line != null)
                {
                    sw.WriteLine(line);
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
