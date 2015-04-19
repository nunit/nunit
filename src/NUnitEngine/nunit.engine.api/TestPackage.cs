// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// TestPackage holds information about a set of test files to
    /// be loaded by a TestRunner. Each TestPackage represents
    /// tests for one or more test files. TestPackages may be named 
    /// or anonymous, depending on how they are constructed.
    /// </summary>
    [Serializable]
    public class TestPackage
    {
        #region Constructors

        /// <summary>
        /// Construct a named TestPackage, specifying a file path for
        /// the assembly or project to be used.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public TestPackage(string filePath)
        {
            ID = GetNextID();

            if (filePath != null)
            {
                FullName = Path.GetFullPath(filePath);
                Settings = new Dictionary<string, object>();
                SubPackages = new List<TestPackage>();
            }
        }

        /// <summary>
        /// Construct an anonymous TestPackage that wraps test files.
        /// </summary>
        /// <param name="testFiles"></param>
        public TestPackage(IList<string> testFiles)
        {
            ID = GetNextID();
            SubPackages = new List<TestPackage>();
            Settings = new Dictionary<string, object>();

            foreach (string testFile in testFiles)
                SubPackages.Add(new TestPackage(testFile));
        }

        private static int _nextID = 0;

        private string GetNextID()
        {
            return (_nextID++).ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Every test package gets a unique ID used to prefix test IDs within that package.
        /// </summary>
        /// <remarks>
        /// The generated ID is only unique for packages created within the same AppDomain.
        /// For that reason, NUnit pre-creates all test packages that will be needed.
        /// </remarks>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the name of the package
        /// </summary>
        public string Name
        {
            get { return FullName == null ? null : Path.GetFileName(FullName); }
        }

        /// <summary>
        /// Gets the path to the file containing tests. It may be
        /// an assembly or a recognized project type.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the list of SubPackages contained in this package
        /// </summary>
        public IList<TestPackage> SubPackages { get; private set; }

        /// <summary>
        /// Gets the settings dictionary for this package.
        /// </summary>
        public IDictionary<string,object> Settings { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a subproject to the package.
        /// </summary>
        /// <param name="subPackage">The subpackage to be added</param>
        public void AddSubPackage(TestPackage subPackage)
        {
            SubPackages.Add(subPackage);

            foreach (var key in Settings.Keys)
                subPackage.Settings[key] = Settings[key];
        }

        /// <summary>
        /// Add a setting to a package and all of its subpackages.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The value of the setting</param>
        /// <remarks>
        /// Once a package is created, subpackages may have been created
        /// as well. If you add a setting directly to the Settings dictionary
        /// of the package, the subpackages are not updated. This method is
        /// used when the settings are intended to be reflected to all the
        /// subpackages under the package.
        /// </remarks>
        public void AddSetting(string name, object value)
        {
            Settings[name] = value;
            foreach (var subPackage in SubPackages)
                subPackage.AddSetting(name, value);
        }

        /// <summary>
        /// Return the value of a setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public T GetSetting<T>(string name, T defaultSetting)
        {
            return Settings.ContainsKey(name)
                ? (T)Settings[name]
                : defaultSetting;
        }

        #endregion

        #region Helper Methods

        private static bool IsAssemblyFileType(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            return extension == ".dll" || extension == ".exe";
        }

        #endregion
    }
}
