// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace NUnit.Engine
{
    /// <summary>
    /// TestPackage holds information about a set of tests to
    /// be loaded by a TestRunner. It may represent a single
    /// assembly or a set of assemblies. It supports selection
    /// of a single test fixture for loading.
    /// </summary>
    [Serializable]
    public class TestPackage
    {
        private ArrayList testFiles = new ArrayList();
        private ListDictionary settings = new ListDictionary();

        private string name;
        private string fullName;

        #region Constructors

        /// <summary>
        /// Construct a TestPackage, specifying the path to an assembly
        /// or project file to which it pertains. The name of the file
        /// is used as the package name.
        /// </summary>
        /// <param name="filePath">The file path for which a package is being constructed.</param>
        public TestPackage(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            this.testFiles.Add(filePath);
            this.fullName = filePath;
            this.name = Path.GetFileName(filePath);
        }

        /// <summary>
        /// Construct a package, specifying a list of test files.
        /// The name of the first file becomes the package name.
        /// </summary>
        /// <param name="assemblies">The list of test files comprising the package</param>
        public TestPackage(IList files)
        {
            this.fullName = Path.GetFullPath((string)files[0]);
            this.name = Path.GetFileName(fullName);

            foreach (string file in files)
                this.testFiles.Add(Path.GetFullPath(file));
        }

        /// <summary>
        /// Construct a package, specifying the name to be used
        /// and a list of test files.
        /// </summary>
        /// <param name="name">The package name, used to name the top-level test node</param>
        /// <param name="assemblies">The list of test files comprising the package</param>
        public TestPackage(string name, IList files)
        {
            this.name = this.fullName = name;

            foreach (string file in files)
                this.testFiles.Add(file);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the package
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the full name of the package, which is usually
        /// the path to the NUnit project used to create the it
        /// </summary>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// The test files (assemblies or projects) to be loaded
        /// </summary>
        public string[] TestFiles
        {
            get { return (string[])testFiles.ToArray(typeof(string)); }
        }

        /// <summary>
        /// Gets the settings dictionary for this package.
        /// </summary>
        public IDictionary Settings
        {
            get { return settings; }
        }

        #endregion

        #region Public Methods

        ///// <summary>
        ///// Return the value of a setting or a default.
        ///// </summary>
        ///// <param name="name">The name of the setting</param>
        ///// <param name="defaultSetting">The default value</param>
        ///// <returns></returns>
        //public T GetSetting<T>(string name, T defaultSetting)
        //{
        //    return Settings.ContainsKey(name)
        //        ? (T)Settings[name]
        //        : defaultSetting;
        //}

        /// <summary>
        /// Return the value of a setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public object GetSetting(string name, object defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : setting;
        }

        /// <summary>
        /// Return the value of a string setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public string GetSetting(string name, string defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (string)setting;
        }

        /// <summary>
        /// Return the value of a bool setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public bool GetSetting(string name, bool defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (bool)setting;
        }

        /// <summary>
        /// Return the value of an int setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public int GetSetting(string name, int defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (int)setting;
        }

        /// <summary>
        /// Return the value of a enum setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public System.Enum GetSetting(string name, System.Enum defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (System.Enum)setting;
        }

        #endregion
    }
}
