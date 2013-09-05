// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

#if !NETCF && !SILVERLIGHT
using System.Collections.Specialized;
using System.Configuration;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// LogCapture is the abstract base for classes that
    /// capture log info from a specific logging system.
    /// </summary>
    public abstract class LogCapture : TextCapture
    {
        private string defaultThreshold;

        /// <summary>
        /// The default threshold for log capture
        /// is read from the config file. If not
        /// found, we use "Error".
        /// </summary>
        public override string DefaultThreshold
        {
            get
            {
                if (defaultThreshold == null)
                {
                    defaultThreshold = "Error";

#if !NUNITLITE
                    NameValueCollection settings = (NameValueCollection)
                        ConfigurationManager.GetSection("NUnit/TestRunner");

                    if (settings != null)
                    {
                        string level = settings["DefaultLogThreshold"];
                        if (level != null)
                            defaultThreshold = level;
                    }
#endif
                }

                return defaultThreshold;
            }
        }
    }
}
#endif
