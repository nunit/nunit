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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NUnit.Engine
{
    /// <summary>
    /// TestPackage holds information about a set of tests to
    /// be loaded by a TestRunner. Each TestPackage represents
    /// tests for a single assembly. Multiple assemblies are
    /// represented by use of subpackages.
    /// </summary>
    [Serializable]
    public class TestPackage
    {
        private string fullName;
        private List<string> testFiles = new List<string>();
        private Dictionary<string, object> settings = new Dictionary<string, object>();

        #region Constructors

        /// <summary>
        /// Construct a TestPackage, specifying a file path for
        /// the assembly or project to be used.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public TestPackage(string filePath)
        {
            fullName = Path.GetFullPath(filePath);
            if (IsAssemblyFileType(filePath))
                Add(FullName);
        }

        /// <summary>
        /// Construct an anonymous TestPackage that wraps 
        /// multiple assemblies or projects as subpackages.
        /// </summary>
        /// <param name="testFiles"></param>
        public TestPackage(IList<string> testFiles)
        {
            foreach (string testFile in testFiles)
                Add(Path.GetFullPath(testFile));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the package
        /// </summary>
        public string Name
        {
            get { return fullName == null ? null : Path.GetFileName(fullName); }
        }

        /// <summary>
        /// Gets the path to the file containing tests. It may be
        /// an assembly or a recognized project type.
        /// </summary>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// Gets an array of the test files contained in this package
        /// </summary>
        public string[] TestFiles
        {
            get { return testFiles.ToArray(); }
        }

        /// <summary>
        /// Gets the settings dictionary for this package.
        /// </summary>
        public IDictionary<string,object> Settings
        {
            get { return settings; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a test file to the package.
        /// </summary>
        /// <param name="testFile">The test file to be added</param>
        public void Add(string testFile)
        {
            testFiles.Add(testFile);
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
