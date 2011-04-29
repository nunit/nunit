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

namespace NUnit.Engine.Api
{
    public class TestPackage
    {
        private IList<string> testFiles = new List<string>();
        private IDictionary<string,object> settings = new Dictionary<string,object>();

        private string name;
        private string fullName;

        #region Constructors

        public TestPackage(string filePath)
        {
            this.testFiles.Add(filePath);
            this.fullName = filePath;
            this.name = Path.GetFileName(filePath);
        }

        public TestPackage(IList<string> files)
        {
            this.fullName = files[0];
            this.name = Path.GetFileName(fullName);

            foreach (string file in files)
                this.testFiles.Add(file);
        }

        public TestPackage(string name, IList<string> files)
        {
            this.name = this.fullName = name;

            foreach (string file in files)
                this.testFiles.Add(file);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public IList<string> TestFiles
        {
            get { return testFiles; }
        }

        public IDictionary<string, object> Settings
        {
            get { return settings; }
        }

        #endregion

        #region Public Methods

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
    }
}
