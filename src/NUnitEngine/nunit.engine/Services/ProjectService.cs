// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.IO;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Summary description for ProjectService.
    /// </summary>
    public class ProjectService : IProjectLoader, IService
    {
        /// <summary>
        /// Seed used to generate names for new projects
        /// </summary>
        //private int projectSeed = 0;

        /// <summary>
        /// The extension used for test projects
        /// </summary>
        //private static readonly string nunitExtension = ".nunit";

        /// <summary>
        /// Array of all installed ProjectLoaders
        /// </summary>
        IProjectLoader[] loaders = new IProjectLoader[] 
        {
            new NUnitProjectLoader(),
            //new VisualStudioLoader()
        };

        #region Instance Methods

        public TestPackage MakeTestPackage(IProject project)
        {
            return MakeTestPackage(project, null);
        }

        public TestPackage MakeTestPackage(IProject project, string configName)
        {
            TestPackage package = new TestPackage(project.ProjectPath);

            if (project.Configs.Count == 0)
                return package;

            foreach (string assembly in project.ActiveConfig.Assemblies)
                package.Add(assembly);

            return package;
        }

        ///// <summary>
        ///// Creates a project to wrap a list of assemblies
        ///// </summary>
        //public IProject WrapAssemblies( string[] assemblies )
        //{
        //    // if only one assembly is passed in then the configuration file
        //    // should follow the name of the assembly. This will only happen
        //    // if the LoadAssembly method is called. Currently the console ui
        //    // does not differentiate between having one or multiple assemblies
        //    // passed in.
        //    if ( assemblies.Length == 1)
        //        return WrapAssembly(assemblies[0]);


        //    NUnitProject project = ServiceContext.ProjectService.EmptyProject();
        //    ProjectConfig config = new ProjectConfig( "Default" );
        //    foreach( string assembly in assemblies )
        //    {
        //        string fullPath = Path.GetFullPath( assembly );

        //        if ( !File.Exists( fullPath ) )
        //            throw new FileNotFoundException( string.Format( "Assembly not found: {0}", fullPath ) );
                
        //        config.Assemblies.Add( fullPath );
        //    }

        //    project.Configs.Add( config );

        //    // TODO: Deduce application base, and provide a
        //    // better value for loadpath and project path
        //    // analagous to how new projects are handled
        //    string basePath = Path.GetDirectoryName( Path.GetFullPath( assemblies[0] ) );
        //    project.ProjectPath = Path.Combine( basePath, project.Name + ".nunit" );

        //    project.IsDirty = true;

        //    return project;
        //}

        ///// <summary>
        ///// Creates a project to wrap an assembly
        ///// </summary>
        //public IProject WrapAssembly( string assemblyPath )
        //{
        //    if ( !File.Exists( assemblyPath ) )
        //        throw new FileNotFoundException( string.Format( "Assembly not found: {0}", assemblyPath ) );

        //    string fullPath = Path.GetFullPath( assemblyPath );

        //    NUnitProject project = new NUnitProject( fullPath );
            
        //    ProjectConfig config = new ProjectConfig( "Default" );
        //    config.Assemblies.Add( fullPath );
        //    project.Configs.Add( config );

        //    project.IsAssemblyWrapper = true;
        //    project.IsDirty = false;

        //    return project;
        //}

        //public string GenerateProjectName()
        //{
        //    return string.Format( "Project{0}", ++projectSeed );
        //}

        //public IProject EmptyProject()
        //{
        //    return new NUnitProject( GenerateProjectName() );
        //}

        //public IProject NewProject()
        //{
        //    NUnitProject project = EmptyProject();

        //    project.Configs.Add( "Debug" );
        //    project.Configs.Add( "Release" );
        //    project.IsDirty = false;

        //    return project;
        //}

        //public void SaveProject( IProject project )
        //{
        //    project.Save();
        //}
        #endregion

        #region IProjectLoader Members

        public bool IsProjectFile(string path)
        {
            foreach( IProjectLoader loader in loaders )
                if ( loader.IsProjectFile(path) )
                    return true;

            return false;
        }

        public IProject LoadProject(string path)
        {
            foreach( IProjectLoader loader in loaders )
            {
                if ( loader.IsProjectFile( path ) )
                    return loader.LoadProject( path );
            }

            return null;
        }

        /// <summary>
        /// Expands a TestPackages based on a known project format,
        /// creating a subpackage for each assembly. The FilePath
        /// of hte package must be checked to ensure that it is
        /// a known project format before calling this method.
        /// </summary>
        /// <param name="package">The TestPackage to be expanded</param>
        public void ExpandProjectPackage(TestPackage package)
        {
            IProject project = LoadProject(package.FullName);

            string configName = package.GetSetting(RunnerSettings.ActiveConfig, string.Empty); // Need RunnerSetting
            IProjectConfig config = configName != string.Empty
                ? project.Configs[configName]
                : project.ActiveConfig;

            foreach (string key in config.Settings.Keys)
                if (!package.Settings.ContainsKey(key)) // Don't override settings from command line
                    package.Settings[key] = config.Settings[key];

            foreach (string assembly in config.Assemblies)
                package.Add(assembly);
        }

        #endregion

        #region IService Members

        private ServiceContext services;
        public ServiceContext ServiceContext
        {
            get { return services; }
            set { services = value; }
        }

        public void InitializeService()
        {
            // TODO:  Add ProjectLoader.InitializeService implementation
        }

        public void UnloadService()
        {
            // TODO:  Add ProjectLoader.UnloadService implementation
        }

        #endregion
    }
}
