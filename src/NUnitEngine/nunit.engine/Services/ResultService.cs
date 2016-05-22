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
using System.Collections.Generic;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services
{
    public class ResultService : Service, IResultService
    {
        private readonly string[] BUILT_IN_FORMATS = new string[] { "nunit3", "cases", "user" };

        private IEnumerable<ExtensionNode> _extensionNodes;

        private string[] _formats;
        public string[] Formats
        {
            get
            {
                if (_formats == null)
                {
                    var formatList = new List<string>(BUILT_IN_FORMATS);

                    foreach (var node in _extensionNodes)
                        foreach (var format in node.GetValues("Format"))
                            formatList.Add(format);
 
                    _formats = formatList.ToArray();
                }

                return _formats;
            }
        }

        /// <summary>
        /// Gets a ResultWriter for a given format and set of arguments.
        /// </summary>
        /// <param name="format">The name of the format to be used</param>
        /// <param name="args">A set of arguments to be used in constructing the writer or null if non arguments are needed</param>
        /// <returns>An IResultWriter</returns>
        public IResultWriter GetResultWriter(string format, object[] args)
        {
            switch (format)
            {
                case "nunit3":
                    return new NUnit3XmlResultWriter();
                case "cases":
                    return new TestCaseResultWriter();
                case "user":
                    return new XmlTransformResultWriter(args);
                default:
                    foreach (var node in _extensionNodes)
                        foreach (var supported in node.GetValues("Format"))
                            if (supported == format)
                                return node.ExtensionObject as IResultWriter;

                    return null;
            }
        }

        #region IService Members

        public override void StartService()
        {
            try
            {
                var extensionService = ServiceContext.GetService<ExtensionService>();

                if (extensionService != null && extensionService.Status == ServiceStatus.Started)
                    _extensionNodes = extensionService.GetExtensionNodes<IResultWriter>();
                
                // If there is no extension service, we start anyway using builtin writers
                Status = ServiceStatus.Started;
            }
            catch
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        #endregion
    }
}
