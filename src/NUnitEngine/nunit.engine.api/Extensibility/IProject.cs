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

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// Interface for the various project types that the engine can load.
    /// </summary>
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
        string ActiveConfigName { get; }

        /// <summary>
        /// Gets a list of the configs for this project
        /// </summary>
        IList<string> ConfigNames { get; }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets a test package for the primary or active
        /// configuration within the project. The package 
        /// includes all the assemblies and any settings
        /// specified in the project format.
        /// </summary>
        /// <returns>A TestPackage</returns>
        TestPackage GetTestPackage();

        /// <summary>
        /// Gets a TestPackage for a specific configuration
        /// within the project. The package includes all the
        /// assemblies and any settings specified in the 
        /// project format.
        /// </summary>
        /// <param name="configName">The name of the config to use</param>
        /// <returns>A TestPackage for the named configuration.</returns>
        TestPackage GetTestPackage(string configName);

        #endregion
    }
}
