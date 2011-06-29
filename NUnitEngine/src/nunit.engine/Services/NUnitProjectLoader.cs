using System;
using System.IO;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public class NUnitProjectLoader : IProjectLoader
    {
        #region IProjectLoader Members

        public bool IsProjectFile(string path)
        {
            return Path.GetExtension(path) == ".nunit";
        }

        public IProject LoadProject(string path)
        {
            NUnitProject project = new NUnitProject();
            project.Load(path);
            return project;
        }

        #endregion
    }
}
