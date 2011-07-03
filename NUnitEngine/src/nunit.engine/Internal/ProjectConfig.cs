// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
                string assemblyPath = XmlHelper.GetAttribute(node, "path");
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
                        settings["BasePath"] = EffectiveBasePath;

                    string configFile = GetAttribute("configfile");
                    if (configFile != null)
                        settings["ConfigurationFile"] = configFile;

                    string binpath = GetAttribute("binpath");
                    if (binpath != null)
                        settings["PrivateBinPath"] = binpath;

                    string binpathtype = GetAttribute("binpathtype");
                    if (binpathtype != null && binpathtype.ToLower() == "auto")
                        settings["AutoBinPath"] = true;

                    string runtime = GetAttribute("runtimeFramework");
                    if (runtime != null)
                        settings["RuntimeFramework"] = RuntimeFramework.Parse(runtime);

                    string processModel = project.GetSetting("processModel");
                    if (processModel != null)
                        settings["ProcessModel"] = (ProcessModel)Enum.Parse(typeof(ProcessModel), processModel);

                    string domainUsage = project.GetSetting("domainUsage");
                    if (domainUsage != null)
                        settings["DomainUsage"] = (DomainUsage)Enum.Parse(typeof(DomainUsage), domainUsage);
                }

                return settings;
            }
        }

		#endregion

        #region Helper Methods

        private string GetAttribute(string name)
        {
            return XmlHelper.GetAttribute(configNode, name);
        }

        #endregion
    }
}
