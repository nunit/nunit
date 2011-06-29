// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.Xml;

namespace NUnit.Engine.Internal
{
	/// <summary>
	/// Summary description for ProjectConfigList.
	/// </summary>
	public class ProjectConfigList : IProjectConfigList
	{
        private NUnitProject project;
        private XmlNode projectNode;

        public ProjectConfigList(NUnitProject project)
        {
            this.project = project;
            this.projectNode = project.RootNode;
        }

		#region IProjectConfigList Members

        public int Count
        {
            get { return ConfigNodes.Count; }
        }

		public IProjectConfig this[int index]
		{
            get { return new Internal.ProjectConfig(project, ConfigNodes[index]); }
		}

        public IProjectConfig this[string name]
        {
            get
            {
                int index = IndexOf(name);
                return index >= 0 ? this[index] : null;
            }
        }

        #endregion

        #region Private Properties

        private XmlNodeList ConfigNodes
        {
            get { return projectNode.SelectNodes("Config"); }
        }

        private XmlNode SettingsNode
        {
            get { return projectNode.SelectSingleNode("Settings"); }
        }

		#endregion

		#region Private Methods

        private int IndexOf(string name)
        {
            for (int index = 0; index < ConfigNodes.Count; index++)
            {
                if (XmlHelper.GetAttribute(ConfigNodes[index], "name") == name)
                    return index;
            }

            return -1;
        }

		#endregion
    }
}
