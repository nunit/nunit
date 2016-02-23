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
using Mono.Cecil;
using NUnit.Engine.Drivers;
using NUnit.Engine.Extensibility;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// The DriverService provides drivers able to load and run tests
    /// using various frameworks.
    /// </summary>
    public class DriverService : Service, IDriverService
    {
        IList<IDriverFactory> _factories = new List<IDriverFactory>();

        #region IDriverService Members

        /// <summary>
        /// Get a driver suitable for use with a particular test assembly.
        /// </summary>
        /// <param name="domain">The AppDomain to use for the tests</param>
        /// <param name="assemblyPath">The full path to the test assembly</param>
        /// <returns></returns>
        public IFrameworkDriver GetDriver(AppDomain domain, string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                return new NotRunnableFrameworkDriver(assemblyPath, "File not found: " + assemblyPath);

            if (!PathUtils.IsAssemblyFileType(assemblyPath))
                return new NotRunnableFrameworkDriver(assemblyPath, "File type is not supported");

            try
            {
                var references = new List<AssemblyName>();
                foreach (var cecilRef in AssemblyDefinition.ReadAssembly(assemblyPath).MainModule.AssemblyReferences)
                    references.Add(new AssemblyName(cecilRef.FullName));

                foreach (var factory in _factories)
                {
                    foreach (var reference in references)
                    {
                        if (factory.IsSupportedTestFramework(reference))
                            return factory.GetDriver(domain, reference);
                    }
                }
            }
            catch (BadImageFormatException ex)
            {
                return new NotRunnableFrameworkDriver(assemblyPath, ex.Message);
            }

            return new NotRunnableFrameworkDriver(assemblyPath, string.Format("No suitable tests found in '{0}'.\n" +
                                                                              "Either assembly contains no tests or proper test driver has not been found.", assemblyPath));
        }

        #endregion

        #region Service Overrides

        public override void StartService()
        {
            Guard.OperationValid(ServiceContext != null, "Can't start DriverService outside of a ServiceContext");

            try
            {
                _factories.Add(new NUnit3DriverFactory());

                var extensionService = ServiceContext.GetService<ExtensionService>();
                if (extensionService != null)
                {
                    foreach (IDriverFactory factory in extensionService.GetExtensions<IDriverFactory>())
                        _factories.Add(factory);
                }

                var node = extensionService.GetExtensionNode("/NUnit/Engine/NUnitV2Driver");
                if (node != null)
                    _factories.Add(new NUnit2DriverFactory(node));
 
                Status = ServiceStatus.Started;
            }
            catch(Exception)
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        #endregion
    }
}
