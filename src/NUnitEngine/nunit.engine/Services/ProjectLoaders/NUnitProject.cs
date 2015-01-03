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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services.ProjectLoaders
{
    public class NUnitProject : IProject
    {
        #region Instance Fields

        /// <summary>
        /// The XmlDocument representing the loaded doc. It
        /// is generated from the text when the doc is loaded
        /// unless an exception is thrown. It is modified as the
        /// user makes changes.
        /// </summary>
        XmlDocument xmlDoc = new XmlDocument();

        #endregion

        #region IProject Members

        /// <summary>
        /// Gets the path to the file storing this project, if any.
        /// If the project has not been saved, this is null.
        /// </summary>
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Gets the active configuration, as defined
        /// by the particular project. For an NUnit
        /// project, we use the activeConfig attribute
        /// if present and otherwise return the first
        /// config found.
        /// </summary>
        public string ActiveConfigName
        {
            get 
            { 
                var activeConfig = GetSetting("activeconfig");
                if (activeConfig == null && ConfigNodes.Count > 0)
                    activeConfig = ConfigNodes[0].GetAttribute("name");
                return activeConfig;
            }
        }

        public IList<string> ConfigNames
        {
            get 
            {
                var result = new List<string>();
                foreach (XmlNode node in ConfigNodes)
                    result.Add(node.GetAttribute("name"));
                return result; 
            }
        }

        public TestPackage GetTestPackage()
        {
            return GetTestPackage(null);
        }

        public TestPackage GetTestPackage(string configName)
        {
            var package = new TestPackage(ProjectPath);

            if (configName == null)
                configName = ActiveConfigName;

            if (configName != null)
            {
                XmlNode configNode = GetConfigNode(configName);

                if (configNode != null)
                {
                    string basePath = GetBasePathForConfig(configNode);

                    foreach (XmlNode node in configNode.SelectNodes("assembly"))
                    {
                        string assembly = node.GetAttribute("path");
                        if (basePath != null)
                            assembly = Path.Combine(basePath, assembly);
                        package.Add(assembly);
                    }

                    var settings = GetSettingsForConfig(configNode);
                    foreach (var key in settings.Keys)
                        package.Settings.Add(key, settings[key]);
                }
            }

            return package;
        }

        #endregion

        #region Public Methods

        public void Load(string filename)
        {
            ProjectPath = NormalizePath(Path.GetFullPath(filename));
            xmlDoc.Load(ProjectPath);
        }

        public void LoadXml(string xmlText)
        {
            xmlDoc.LoadXml(xmlText);
        }

        public string GetSetting(string name)
        {
            return SettingsNode != null
                ? SettingsNode.GetAttribute(name)
                : null;
        }

        public string GetSetting(string name, string defaultValue)
        {
            string result = GetSetting(name);
            return result == null
                ? defaultValue
                : result;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// The top-level (NUnitProject) node
        /// </summary>
        internal XmlNode RootNode
        {
            get { return xmlDoc.FirstChild; }
        }

        /// <summary>
        /// The Settings node if present, otherwise null
        /// </summary>
        internal XmlNode SettingsNode
        {
            get { return RootNode.SelectSingleNode("Settings"); }
        }

        /// <summary>
        /// The collection of Config nodes - may be empty
        /// </summary>
        internal XmlNodeList ConfigNodes
        {
            get { return RootNode.SelectNodes("Config"); }
        }

        #endregion

        #region Helper Methods

        private XmlNode GetConfigNode(string name)
        {
            foreach (XmlNode node in ConfigNodes)
                if (node.GetAttribute("name") == name)
                    return node;

            return null;
        }

        /// <summary>
        /// GetProjectBasePath uses the BasePath if present and otherwise
        /// defaults to the directory part of the ProjectPath.
        /// </summary>
        public string GetProjectBasePath()
        {
            string appbase = NormalizePath(GetSetting("appbase"));

            if (ProjectPath == null)
                return appbase;

            if (appbase == null)
                return Path.GetDirectoryName(ProjectPath);

            return Path.Combine(
                Path.GetDirectoryName(ProjectPath),
                appbase);
        }

        private string GetBasePathForConfig(XmlNode configNode)
        {
            string projectBasePath = GetProjectBasePath();

            string configBasePath = NormalizePath(configNode.GetAttribute("appbase"));
            if (configBasePath == null)
                configBasePath = projectBasePath;
            else if (projectBasePath != null)
                configBasePath = Path.Combine(projectBasePath, configBasePath);

            return configBasePath;
        }

        private IDictionary<string, object> GetSettingsForConfig(XmlNode configNode)
        {
            var settings = new Dictionary<string, object>();

            string basePath = GetBasePathForConfig(configNode);
            if (basePath != ProjectPath)
                settings[RunnerSettings.BasePath] = basePath;

            string configFile = configNode.GetAttribute("configfile");
            if (configFile != null)
                settings[RunnerSettings.ConfigurationFile] = configFile;

            string binpath = configNode.GetAttribute("binpath");
            if (binpath != null)
                settings[RunnerSettings.PrivateBinPath] = binpath;

            string binpathtype = configNode.GetAttribute("binpathtype");
            if (binpathtype != null && binpathtype.ToLower() == "auto")
                settings[RunnerSettings.AutoBinPath] = true;

            string runtime = configNode.GetAttribute("runtimeFramework");
            if (runtime != null)
                settings[RunnerSettings.RuntimeFramework] = runtime;

            string processModel = GetSetting("processModel");
            if (processModel != null)
                settings[RunnerSettings.ProcessModel] = processModel;

            string domainUsage = GetSetting("domainUsage");
            if (domainUsage != null)
                settings[RunnerSettings.DomainUsage] = domainUsage;

            return settings;
        }

        static readonly char[] PATH_SEPARATORS = new char[] { '/', '\\' };

        private string NormalizePath(string path)
        {
            char sep = Path.DirectorySeparatorChar;

            if (path != null)
                foreach (char alt in PATH_SEPARATORS)
                    if (alt != sep)
                        path = path.Replace(alt, sep);

            return path;
        }

        #endregion
    }
}
