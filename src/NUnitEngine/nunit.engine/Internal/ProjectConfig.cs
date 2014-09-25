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
using System.Xml;
using NUnit.Common;

namespace NUnit.Engine.Internal
{
    public class ProjectConfig : IProjectConfig
    {
        #region Instance Variables

        private NUnitProject project;

        /// <summary>
        /// The XmlNode representing this config
        /// </summary>
        private XmlNode configNode;
        
        /// <summary>
        /// List of the test assemblies in this config
        /// </summary>
        private List<string> assemblies;

        #endregion

        #region Constructor

        public ProjectConfig(NUnitProject project, XmlNode configNode)
        {
            this.project = project;
            this.configNode = configNode;
            this.assemblies = new List<string>();
            foreach (XmlNode node in configNode.SelectNodes("assembly"))
            {
                string assemblyPath = node.GetAttribute("path");
                if (this.EffectiveBasePath != null)
                    assemblyPath = Path.Combine(this.EffectiveBasePath, assemblyPath);
                this.assemblies.Add(assemblyPath);
            }
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return GetAttribute("name"); }
        }

        /// <summary>
        /// The actual base path used in loading the tests. Its
        /// value depends on the appbase entry of the config element
        /// as well as the project EffectiveBasePath.
        /// </summary>
        private string EffectiveBasePath
        {
            get
            {
                string basePath = GetAttribute("appbase");

                if (basePath == null)
                    return project.EffectiveBasePath;

                if (project.EffectiveBasePath == null)
                    return basePath;

                return Path.Combine(project.EffectiveBasePath, basePath);
            }
        }

        /// <summary>
        /// Return our AssemblyList
        /// </summary>
        public string[] Assemblies
        {
            get { return assemblies.ToArray(); }
        }

        private IDictionary<string,object> settings;
        public IDictionary<string,object>Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new Dictionary<string, object>();

                    if (EffectiveBasePath != project.ProjectPath)
                        settings[RunnerSettings.BasePath] = EffectiveBasePath;

                    string configFile = GetAttribute("configfile");
                    if (configFile != null)
                        settings[RunnerSettings.ConfigurationFile] = configFile;

                    string binpath = GetAttribute("binpath");
                    if (binpath != null)
                        settings[RunnerSettings.PrivateBinPath] = binpath;

                    string binpathtype = GetAttribute("binpathtype");
                    if (binpathtype != null && binpathtype.ToLower() == "auto")
                        settings[RunnerSettings.AutoBinPath] = true;

                    string runtime = GetAttribute("runtimeFramework");
                    if (runtime != null)
                        settings[RunnerSettings.RuntimeFramework] = runtime;

                    string processModel = project.GetSetting("processModel");
                    if (processModel != null)
                        settings[RunnerSettings.ProcessModel] = processModel;

                    string domainUsage = project.GetSetting("domainUsage");
                    if (domainUsage != null)
                        settings[RunnerSettings.DomainUsage] = domainUsage;
                }

                return settings;
            }
        }

        #endregion

        #region Helper Methods

        private string GetAttribute(string name)
        {
            return configNode.GetAttribute(name);
        }

        #endregion
    }
}
