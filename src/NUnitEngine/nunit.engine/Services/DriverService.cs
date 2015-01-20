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
using System.Text;
using Mono.Addins;
using NUnit.Engine.Drivers;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services
{
    public class DriverService : IDriverService, IService
    {
        private const string NUNIT_FRAMEWORK = "nunit.framework";
        private const string NUNITLITE_FRAMEWORK = "nunitlite";

        private const string OLDER_NUNIT_NOT_SUPPORTED_MESSAGE =
            "Unable to load {0}. This runner only supports tests written for NUnit 3.0 or higher.";

        IList<IDriverFactory> _factories = new List<IDriverFactory>();

        #region IDriverService Members

        public IFrameworkDriver GetDriver(AppDomain domain, string assemblyPath, IDictionary<string, object> settings)
        {
            if (!File.Exists(assemblyPath))
                return new NotRunnableFrameworkDriver(assemblyPath, "File not found: " + assemblyPath);

            try
            {
                var testAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                var references = testAssembly.GetReferencedAssemblies();

                foreach (var factory in _factories)
                {
                    foreach (var reference in references)
                    {
                        if (factory.IsSupportedFramework(reference))
                            return factory.GetDriver(domain, reference.Name, assemblyPath, settings);
                    }
                }
            }
            catch (Exception ex)
            {
                return new NotRunnableFrameworkDriver(assemblyPath, ex.Message);
            }

            return new NotRunnableFrameworkDriver(assemblyPath, "Unable to locate a driver for " + assemblyPath);
        }

        #endregion
 
        #region IService Members

        private ServiceContext services;
        public ServiceContext ServiceContext 
        {
            get { return services; }
            set { services = value; }
        }

        public void InitializeService()
        {
            _factories.Add(new NUnit3DriverFactory());

            foreach (IDriverFactory factory in AddinManager.GetExtensionObjects<IDriverFactory>())
                _factories.Add(factory);
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}
