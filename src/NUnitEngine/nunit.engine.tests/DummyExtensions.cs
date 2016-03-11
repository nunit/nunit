// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Tests
{
    [Extension]
    public class DummyFrameworkDriverExtension : IDriverFactory
    {
        public IFrameworkDriver GetDriver(AppDomain domain, AssemblyName reference)
        {
            throw new NotImplementedException();
        }

        public bool IsSupportedTestFramework(AssemblyName reference)
        {
            throw new NotImplementedException();
        }
    }

    [Extension]
    public class DummyProjectLoaderExtension : IProjectLoader
    {
        public bool CanLoadFrom(string path)
        {
            throw new NotImplementedException();
        }

        public IProject LoadFrom(string path)
        {
            throw new NotImplementedException();
        }
    }

    [Extension]
    public class DummyResultWriterExtension : IResultWriter
    {
        public void CheckWritability(string outputPath)
        {
            throw new NotImplementedException();
        }

        public void WriteResultFile(XmlNode resultNode, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public void WriteResultFile(XmlNode resultNode, string outputPath)
        {
            throw new NotImplementedException();
        }
    }

    [Extension]
    public class DummyEventListenerExtension : ITestEventListener
    {
        public void OnTestEvent(string report)
        {
            throw new NotImplementedException();
        }
    }

    [Extension]
    public class DummyServiceExtension : IService
    {
        public IServiceLocator ServiceContext
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ServiceStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void StartService()
        {
            throw new NotImplementedException();
        }

        public void StopService()
        {
            throw new NotImplementedException();
        }
    }

    [Extension]
    public class V2DriverExtension : IFrameworkDriver
    {
        public string ID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int CountTestCases(string filter)
        {
            throw new NotImplementedException();
        }

        public string Explore(string filter)
        {
            throw new NotImplementedException();
        }

        public string Load(string testAssemblyPath, IDictionary<string, object> settings)
        {
            throw new NotImplementedException();
        }

        public string Run(ITestEventListener listener, string filter)
        {
            throw new NotImplementedException();
        }

        public void StopRun(bool force)
        {
            throw new NotImplementedException();
        }
    }
}
