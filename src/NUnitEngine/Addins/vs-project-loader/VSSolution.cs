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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services.ProjectLoaders
{
    public class VSSolution : IProject
    {
        static readonly char[] DELIMS = { '=', ',' };
        static readonly char[] TRIM_CHARS = { ' ', '"' };
        const string BUILD_MARKER = ".Build.0 =";

        IDictionary<string, VSProject> _projectLookup = new Dictionary<string, VSProject>();
        IDictionary<string, SolutionConfig> _configs = new Dictionary<string, SolutionConfig>();

        #region Constructor

        public VSSolution(string projectPath)
        {
            ProjectPath = Path.GetFullPath(projectPath);

            Load();
        }

        #endregion

        #region IProject Members

        /// <summary>
        /// The path to the project
        /// </summary>
        public string ProjectPath { get; private set; }

        public string ActiveConfigName
        {
            get
            {
                var names = ConfigNames;
                return names.Count > 0 ? names[0] : null;
            }
        }

        public IList<string> ConfigNames
        {
            get 
            {
                var names = new List<string>();
                foreach (var name in _configs.Keys)
                    names.Add(name);
                return names;
            }
        }

        public TestPackage GetTestPackage()
        {
            return GetTestPackage(null);
        }

        public TestPackage GetTestPackage(string configName)
        {
            var package = new TestPackage(ProjectPath);

            foreach (var name in _configs.Keys)
            {
                if (configName == null || configName == name)
                {
                    var config = _configs[name];
                    foreach (string assembly in config.Assemblies)
                    {
                        package.AddSubPackage(new TestPackage(assembly));
                    }
                    break;
                }
            }

            return package;
        }

        #endregion

        #region HelperMethods

        private void Load()
        {
            string solutionDirectory = Path.GetDirectoryName(ProjectPath);
            using (StreamReader reader = new StreamReader(ProjectPath))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.StartsWith("Project("))
                    {
                        string[] parts = line.Split(DELIMS);
                        string vsProjectPath = parts[2].Trim(TRIM_CHARS);
                        string vsProjectGuid = parts[3].Trim(TRIM_CHARS);

                        if (VSProject.IsProjectFile(vsProjectPath))
                        {
                            var vsProject = new VSProject(Path.Combine(solutionDirectory, vsProjectPath));

                            if (CheckProjectReferencesNunit(vsProject))
                                _projectLookup[vsProjectGuid] = vsProject;
                        }
                    }
                    else if (line.IndexOf(BUILD_MARKER) >= 0)
                    {
                        line = line.Trim();
                        int endBrace = line.IndexOf('}');

                        string vsProjectGuid = line.Substring(0, endBrace + 1);
                        VSProject vsProject;
                        if (_projectLookup.TryGetValue(vsProjectGuid, out vsProject))
                        {
                            line = line.Substring(endBrace + 2);

                            int split = line.IndexOf(BUILD_MARKER) + 1;
                            string solutionConfig = line.Substring(0, split - 1);
                            int bar = solutionConfig.IndexOf('|');
                            if (bar >= 0)
                                solutionConfig = solutionConfig.Substring(0, bar);

                            string projectConfig = line.Substring(split + BUILD_MARKER.Length);
                            if (!vsProject.ConfigNames.Contains(projectConfig))
                            {
                                bar = projectConfig.IndexOf('|');
                                if (bar >= 0)
                                    projectConfig = projectConfig.Substring(0, bar);
                            }

                            SolutionConfig config = null;

                            if (_configs.ContainsKey(solutionConfig))
                                config = _configs[solutionConfig];
                            else
                            {
                                config = new SolutionConfig(solutionConfig);
                                _configs.Add(solutionConfig, config);
                            }

                            foreach (var subPackage in vsProject.GetTestPackage(projectConfig).SubPackages)
                                if (!config.Assemblies.Contains(subPackage.FullName))
                                    config.Assemblies.Add(subPackage.FullName);

                            //if (VSProject.IsProjectFile(vsProjectPath))
                            //    project.Add(new VSProject(Path.Combine(solutionDirectory, vsProjectPath)));
                        }
                    }

                    line = reader.ReadLine();
                }
            }
        }

        private bool CheckProjectReferencesNunit(VSProject vsProject)
        {
            if (vsProject.MsBuildDocument == null)
                return true;

            var doc = vsProject.MsBuildDocument;
            var namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            var hasNunitReference =
                doc.SelectNodes("/msbuild:Project/msbuild:ItemGroup/msbuild:Reference[@Include]", namespaceManager);

            if (hasNunitReference == null)
                return false;

            foreach (XmlNode reference in hasNunitReference)
            {
                if (reference.Attributes != null)
                {
                    var value = reference.Attributes["Include"].Value.ToUpper();

                    if (value.StartsWith("NUNIT.FRAMEWORK"))
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Nested SolutionConfig Class

        private class SolutionConfig
        {
            public SolutionConfig(string name)
            {
                Name = name;
                Assemblies = new List<string>();
            }

            public string Name { get; private set; }

            public IList<string> Assemblies { get; private set; }
        }

        #endregion
    }
}
