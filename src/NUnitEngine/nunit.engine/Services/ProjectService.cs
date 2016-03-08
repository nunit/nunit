// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
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
using NUnit.Common;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Summary description for ProjectService.
    /// </summary>
    public class ProjectService : Service, IProjectService
    {
        Dictionary<string, ExtensionNode> _extensionIndex = new Dictionary<string, ExtensionNode>();

        #region IProjectService Members

        public bool CanLoadFrom(string path)
        {
            ExtensionNode node = GetNodeForPath(path);
            if (node != null)
            {
                var loader = node.ExtensionObject as IProjectLoader;
                if (loader.CanLoadFrom(path))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Expands a TestPackage based on a known project format, populating it
        /// with the project contents and any settings the project provides. 
        /// Note that the package file path must be checked to ensure that it is
        /// a known project format before calling this method.
        /// </summary>
        /// <param name="package">The TestPackage to be expanded</param>
        public void ExpandProjectPackage(TestPackage package)
        {
            Guard.ArgumentNotNull(package, "package");
            Guard.ArgumentValid(package.SubPackages.Count == 0, "Package is already expanded", "package");

            string path = package.FullName;
            if (!File.Exists(path))
                return;

            IProject project = LoadFrom(path);
            Guard.ArgumentValid(project != null, "Unable to load project " + path, "package");

            string configName = package.GetSetting(PackageSettings.ActiveConfig, (string)null); // Need RunnerSetting
            TestPackage tempPackage = project.GetTestPackage(configName);

            // The original package held overrides, so don't change them, but
            // do apply any settings specified within the project itself.
            foreach (string key in tempPackage.Settings.Keys)
                if (!package.Settings.ContainsKey(key)) // Don't override settings from command line
                    package.Settings[key] = tempPackage.Settings[key];

            foreach (var subPackage in tempPackage.SubPackages)
                package.AddSubPackage(subPackage);

            // If no config is specified (by user or by the proejct loader) check
            // to see if one exists in same directory as the package. If so, we
            // use it. If not, each assembly will use it's own config, if present.
            if (!package.Settings.ContainsKey(PackageSettings.ConfigurationFile))
            {
                var packageConfig = Path.ChangeExtension(path, ".config");
                if (File.Exists(packageConfig))
                    package.Settings[PackageSettings.ConfigurationFile] = packageConfig;
            }
        }

        #endregion

        #region Service Overrides

        public override void StartService()
        {
            if (Status == ServiceStatus.Stopped)
            {
                try
                {
                    var extensionService = ServiceContext.GetService<ExtensionService>();

                    if (extensionService != null && extensionService.Status == ServiceStatus.Started)
                    {
                        foreach (var node in extensionService.GetExtensionNodes<IProjectLoader>())
                        {
                            foreach (string ext in node.GetProperties("FileExtension"))
                            {
                                if (ext != null)
                                {
                                    if (_extensionIndex.ContainsKey(ext))
                                        throw new NUnitEngineException(string.Format("ProjectLoader extension {0} is already handled by another extension.", ext));

                                    _extensionIndex.Add(ext, node);
                                }
                            }
                        }

                        Status = ServiceStatus.Started;
                    }
                    else
                        Status = ServiceStatus.Error;
                }
                catch
                {
                    // TODO: Should we just ignore any addin that doesn't load?
                    Status = ServiceStatus.Error;
                    throw;
                }
            }
        }

        #endregion

        #region Helper Methods

        private IProject LoadFrom(string path)
        {
            if (File.Exists(path))
            {
                ExtensionNode node = GetNodeForPath(path);
                if (node != null)
                {
                    var loader = node.ExtensionObject as IProjectLoader;
                    if (loader.CanLoadFrom(path))
                        return loader.LoadFrom(path);
                }
            }

            return null;
        }

        private ExtensionNode GetNodeForPath(string path)
        {
            var ext = Path.GetExtension(path);

            if (string.IsNullOrEmpty(ext) || !_extensionIndex.ContainsKey(ext))
                return null;

            return _extensionIndex[ext];
        }

        #endregion
    }
}
