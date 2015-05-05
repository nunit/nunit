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
using System.IO;
using System.Reflection;
using System.Xml;
using Mono.Addins;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services
{
    public class ResultService : IResultService, IService
    {
        IList<IResultWriterFactory> _factories = new List<IResultWriterFactory>();

        public string[] Formats
        {
            get
            {
                List<string> formats = new List<string>();

                foreach (var factory in _factories)
                    formats.Add(factory.Format);

                return formats.ToArray();
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
            // TODO: Handle invalid arguments.
            foreach (var factory in _factories)
            {
                if (factory.Format == format)
                    return factory.GetResultWriter(args);
            }

            throw new ArgumentException(string.Format("Invalid XML output format '{0}'", format), "format");
        }

        #region IService Members

        public ServiceContext ServiceContext { get; set; }

        public ServiceStatus Status { get; private set; }

        public void StartService()
        {
            try
            {
                _factories.Add(new NUnit3ResultWriterFactory());
                _factories.Add(new TestCaseResultWriterFactory());
                _factories.Add(new XmlTransformResultWriterFactory());

                foreach (var factory in AddinManager.GetExtensionObjects<IResultWriterFactory>())
                    _factories.Add(factory);

                Status = ServiceStatus.Started;
            }
            catch
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        public void StopService()
        {
            Status = ServiceStatus.Stopped;
        }

        #endregion

        #region Nested Factory Classes

        abstract class ResultWriterFactory : IResultWriterFactory
        {
            public ResultWriterFactory(string format)
            {
                Format = format;
            }

            public string Format { get; private set; }

            public abstract IResultWriter GetResultWriter(params object[] args);
        }

        class NUnit3ResultWriterFactory : ResultWriterFactory
        {
            public NUnit3ResultWriterFactory() : base("nunit3") { }

            public override IResultWriter GetResultWriter(params object[] args)
            {
                return new NUnit3XmlResultWriter();
            }
        }

        class TestCaseResultWriterFactory : ResultWriterFactory
        {
            public TestCaseResultWriterFactory() : base("cases") { }

            public override IResultWriter GetResultWriter(params object[] args)
            {
                return new TestCaseResultWriter();
            }
        }

        class XmlTransformResultWriterFactory : ResultWriterFactory
        {
            public XmlTransformResultWriterFactory() : base("user") { }

            public override IResultWriter GetResultWriter(params object[] args)
            {
                return new XmlTransformResultWriter(args);
            }
        }

        #endregion
    }
}
