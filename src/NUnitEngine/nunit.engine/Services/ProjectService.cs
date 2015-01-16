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

using System.Collections.Generic;
using NUnit.Common;
using Mono.Addins;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Summary description for ProjectService.
    /// </summary>
    public class ProjectService : IProjectLoader, IService
    {
        /// <summary>
        /// List of all installed ProjectLoaders
        /// </summary>
        IList<IProjectLoader> _loaders = new List<IProjectLoader>();

        bool _isInitialized;

        #region IProjectLoader Members

        public bool CanLoadFrom(string path)
        {
            foreach (IProjectLoader loader in _loaders)
                if (loader.CanLoadFrom(path))
                    return true;

            return false;
        }

        public IProject LoadFrom(string path)
        {
            foreach (IProjectLoader loader in _loaders)
                if (loader.CanLoadFrom(path))
                    return loader.LoadFrom(path);

            return null;
        }

        #endregion
        
        #region Other Public Methods

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
            Guard.ArgumentValid(package.TestFiles.Count == 0, "Package is already expanded", "package");

            string path = package.FullName;
            IProject project = LoadFrom(path);
            Guard.ArgumentValid(project != null, "Unable to load project " + path, "package");

            string configName = package.GetSetting(PackageSettings.ActiveConfig, (string)null); // Need RunnerSetting
            TestPackage tempPackage = project.GetTestPackage(configName);

            // The original package held overrides, so don't change them, but
            // do apply any settings specified within the project itself.
            foreach (string key in tempPackage.Settings.Keys)
                if (!package.Settings.ContainsKey(key)) // Don't override settings from command line
                    package.Settings[key] = tempPackage.Settings[key];

            foreach (string assembly in tempPackage.TestFiles)
                package.Add(assembly);
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
            if (!_isInitialized)
            {
                _isInitialized = true;

                foreach (IProjectLoader loader in AddinManager.GetExtensionObjects<IProjectLoader>())
                    _loaders.Add(loader);
            }
        }

        public void UnloadService()
        {
            // TODO:  Add ProjectLoader.UnloadService implementation
        }

        #endregion
    }
}
