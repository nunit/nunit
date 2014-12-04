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

using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace NUnit.Engine.Services
{
    using ResultWriters;

    public class ResultService : IResultService, IService
    {
        public string[] Formats
        {
            get { return new string[] { "nunit3", "nunit2", "cases", "user" }; }
        }

        /// <summary>
        /// Gets a ResultWriter for a given format and set of arguments.
        /// </summary>
        /// <param name="format">The name of the format to be used</param>
        /// <param name="args">A set of arguments to be used in constructing the writer or null if non arguments are needed</param>
        /// <returns>An IResultWriter</returns>
        public IResultWriter GetResultWriter(string format, object[] args)
        {
            // NOTE: When we switch to having ResultWriters as addins, this
            // construction will have to be done by reflection, so we keep
            // the method signature as general as possible, even though
            // we currently make assumptions about what is being passed.
            // TODO: Handle invalid arguments.
            switch (format)
            {
                case "nunit3":
                    return new NUnit3XmlResultWriter();

                case "nunit2":
                    return new NUnit2XmlResultWriter();

                case "cases":
                    return new TestCaseResultWriter();

                case "user":
                    // TODO: Move logic specific to this case into the ResultWriter itself
                    //Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    //string dir = Path.GetDirectoryName(uri.LocalPath);
                    //string transform = (string)args[0];
                    //return new XmlTransformOutputWriter(Path.Combine(dir, args));
                    return new XmlTransformResultWriter(args);

                default:
                    throw new ArgumentException(string.Format("Invalid XML output format '{0}'", format), "format");
            }
        }

        #region IService Members

        public ServiceContext ServiceContext { get; set; }

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}
