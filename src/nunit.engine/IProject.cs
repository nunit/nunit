// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************


namespace NUnit.Engine
{
    public interface IProject
    {
        #region Properties

        /// <summary>
        /// Gets the path to the file storing this project, if any.
        /// If the project has not been saved, this is null.
        /// </summary>
        string ProjectPath { get; }

        /// <summary>
        /// Gets the active configuration, as defined
        /// by the particular project.
        /// </summary>
        IProjectConfig ActiveConfig { get; }

        /// <summary>
        /// Gets a list of the configs for this project
        /// </summary>
        IProjectConfigList Configs { get; }

        #endregion
    }
}
