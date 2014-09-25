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

using System.Xml;
using NUnit.Common;

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
                if (ConfigNodes[index].GetAttribute("name") == name)
                    return index;
            }

            return -1;
        }

        #endregion
    }
}
