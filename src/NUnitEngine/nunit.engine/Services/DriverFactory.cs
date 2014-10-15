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
using NUnit.Engine.Drivers;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public class DriverFactory : IDriverFactory, IService
    {
        private static readonly List<string> NUnitAssemblies = new List<string>(new string[] { "nunit.framework", "nunitlite" });

        private const string OLDER_NUNIT_NOT_SUPPORTED_MESSAGE =
            "Unable to load {0}. This runner only supports tests written for NUnit 3.0 or higher.";

        #region IDriverFactory Members
        // TODO: This method is a standin for our future implementation, which will
        // load drivers as plugins, in separate assemblies. This implementation has
        // too much knowledge of what test framework the drivers can handle. In the
        // future, this responsibility will have to be passed to the driver itself.
        public IFrameworkDriver GetDriver(AppDomain domain, string assemblyPath, IDictionary<string, object> settings)
        {
            try
            {
                var testAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                var nunitV3 = new Version(3, 0);

                foreach (var refAssembly in testAssembly.GetReferencedAssemblies())
                {
                    if (NUnitAssemblies.Contains(refAssembly.Name))
                        if (refAssembly.Version >= nunitV3)
                            return new NUnitFrameworkDriver(domain, refAssembly.Name, assemblyPath, settings);
                        else
                            return new NotRunnableFrameworkDriver(assemblyPath, string.Format(OLDER_NUNIT_NOT_SUPPORTED_MESSAGE, assemblyPath));
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
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}
