// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

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
