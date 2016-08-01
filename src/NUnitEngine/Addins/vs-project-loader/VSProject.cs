// ***********************************************************************
// Copyright (c) 2002-2014 Charlie Poole
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
using System.Text.RegularExpressions;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services.ProjectLoaders
{
    /// <summary>
    /// This class allows loading information about
    /// configurations and assemblies in a Visual
    /// Studio project file and inspecting them.
    /// Only the most common project types are
    /// supported and an exception is thrown if
    /// an attempt is made to load an invalid
    /// file or one of an unknown type.
    /// </summary>
    public class VSProject : IProject
    {
        #region Static and Instance Variables

        /// <summary>
        /// VS Project extentions
        /// </summary>
        private static readonly string[] PROJECT_EXTENSIONS = { ".csproj", ".vbproj", ".vjsproj", ".vcproj", ".fsproj" };
        
        /// <summary>
        /// VS Solution extension
        /// </summary>
        private const string SOLUTION_EXTENSION = ".sln";

        /// <summary>
        /// The XML representation of the project
        /// </summary>
        private XmlDocument _doc;

        /// <summary>
        /// The list of all our configs
        /// </summary>
        private IDictionary<string, ProjectConfig> _configs = new Dictionary<string, ProjectConfig>();

        #endregion

        #region Constructor

        public VSProject( string projectPath )
        {
            ProjectPath = Path.GetFullPath( projectPath );

            Load();
        }

        #endregion

        #region IProject Members

        /// <summary>
        /// The path to the project
        /// </summary>
        public string ProjectPath { get; private set; }
        public XmlDocument MsBuildDocument { get; private set; }

        /// <summary>
        /// Gets the active configuration, as defined
        /// by the particular project. For a VS
        /// project, we use the first config found.
        /// </summary>
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
            TestPackage package = new TestPackage(ProjectPath);

            string appbase = null;
            foreach (var name in _configs.Keys)
            {
                if (configName == null || configName == name)
                {
                    var config = _configs[name];
                    package.AddSubPackage(new TestPackage(config.AssemblyPath));
                    appbase = config.OutputDirectory;
                    break;
                }
            }

            if (appbase != null)
                package.AddSetting("BasePath", appbase);

            return package;
        }

        #endregion

        #region Other Properties

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name
        {
            get { return Path.GetFileNameWithoutExtension( ProjectPath ); }
        }

        #endregion

        #region Public Methods

        public static bool IsProjectFile( string path )
        {
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;

            if ( path.ToLower().IndexOf( "http:" ) >= 0 )
                return false;
        
            string extension = Path.GetExtension( path );

            foreach( string validExtension in PROJECT_EXTENSIONS )
                if ( extension == validExtension )
                    return true;

            return false;
        }

        public static bool IsSolutionFile( string path )
        {
            return Path.GetExtension( path ) == SOLUTION_EXTENSION;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Load a project in various ways, depending on the extension.
        /// </summary>
        private void Load()
        {
            if ( !IsProjectFile( ProjectPath ) ) 
                ThrowInvalidFileType( ProjectPath );

            StreamReader rdr = new StreamReader(ProjectPath, System.Text.Encoding.UTF8);
            
            try
            {
                _doc = new XmlDocument();
                _doc.Load( rdr );

                string extension = Path.GetExtension( ProjectPath );

                switch ( extension )
                {
                    case ".csproj":
                    case ".vbproj":
                    case ".vjsproj":
                    case ".fsproj":
                        // We try legacy projects first, as the initial check is simplest
                        if (!TryLoadLegacyProject())
                            LoadMSBuildProject();
                        break;

                    case ".vcproj":
                        LoadLegacyCppProject();
                        break;

                    default:
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                ThrowInvalidFormat(ProjectPath, e);
            }
            finally
            {
                rdr.Close();
            }
        }

        /// <summary>
        /// Load a project in the legacy VS2003 format. Note that this method is not 
        /// called for C++ projects using the same format, because the details differ.
        /// </summary>
        /// <returns>True if this project is in the VS2003 format, otherwise false.</returns>
        private bool TryLoadLegacyProject()
        {
            XmlNode settingsNode = _doc.SelectSingleNode("/VisualStudioProject/*/Build/Settings");
            if (settingsNode == null)
                return false;

            string assemblyName = RequiredAttributeValue(settingsNode, "AssemblyName");
            string outputType = RequiredAttributeValue(settingsNode, "OutputType");

            if (outputType == "Exe" || outputType == "WinExe")
                assemblyName = assemblyName + ".exe";
            else
                assemblyName = assemblyName + ".dll";

            XmlNodeList nodes = settingsNode.SelectNodes("Config");
            if (nodes != null)
                foreach (XmlNode configNode in nodes)
                {
                    string name = RequiredAttributeValue(configNode, "Name");
                    string outputPath = RequiredAttributeValue(configNode, "OutputPath");

                    _configs.Add(name, new ProjectConfig(this, name, outputPath, assemblyName));
                }

            return true;
        }

        /// <summary>
        /// Load a non-C++ project in the MsBuild format introduced with VS2005
        /// </summary>
        private void LoadMSBuildProject()
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(_doc.NameTable);
            namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNodeList nodes = _doc.SelectNodes("/msbuild:Project/msbuild:PropertyGroup", namespaceManager);
            if (nodes == null) return;

            MsBuildDocument = _doc;

            XmlElement assemblyNameElement = (XmlElement)_doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", namespaceManager);
            string assemblyName = assemblyNameElement.InnerText;

            XmlElement outputTypeElement = (XmlElement)_doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:OutputType", namespaceManager);
            string outputType = outputTypeElement.InnerText;

            if (outputType == "Exe" || outputType == "WinExe")
                assemblyName = assemblyName + ".exe";
            else
                assemblyName = assemblyName + ".dll";

            string commonOutputPath = null;

            foreach (XmlElement configNode in nodes)
            {
                string name = GetConfigNameFromCondition(configNode);

                XmlElement outputPathElement = (XmlElement)configNode.SelectSingleNode("msbuild:OutputPath", namespaceManager);
                string outputPath = null;
                if (outputPathElement != null)
                    outputPath = outputPathElement.InnerText;

                if (name == null)
                {
                    commonOutputPath = outputPath;
                    continue;
                }

                if (outputPath == null)
                    outputPath = commonOutputPath;

                if (outputPath != null)
                    _configs.Add(name, new ProjectConfig(this, name, outputPath, assemblyName));
            }
        }

        /// <summary>
        /// Load a C++ project in the legacy format, which was used for C++
        /// much longer than it was used for the other languages supported.
        /// </summary>
        private void LoadLegacyCppProject()
        {
            string[] extensionsByConfigType = { "", ".exe", ".dll", ".lib", "" };

            // TODO: This is all very hacked up... replace it.
            foreach (XmlNode configNode in _doc.SelectNodes("/VisualStudioProject/Configurations/Configuration"))
            {
                string name = RequiredAttributeValue(configNode, "Name");
                int config_type = System.Convert.ToInt32(RequiredAttributeValue(configNode, "ConfigurationType"));
                string dirName = name;
                int bar = dirName.IndexOf('|');
                if (bar >= 0)
                    dirName = dirName.Substring(0, bar);
                string outputPath = RequiredAttributeValue(configNode, "OutputDirectory");
                outputPath = outputPath.Replace("$(SolutionDir)", Path.GetFullPath(Path.GetDirectoryName(ProjectPath)) + Path.DirectorySeparatorChar);
                outputPath = outputPath.Replace("$(ConfigurationName)", dirName);

                XmlNode toolNode = configNode.SelectSingleNode("Tool[@Name='VCLinkerTool']");
                string assemblyName = null;
                if (toolNode != null)
                {
                    assemblyName = SafeAttributeValue(toolNode, "OutputFile");
                    if (assemblyName != null)
                        assemblyName = Path.GetFileName(assemblyName);
                    else
                        assemblyName = Path.GetFileNameWithoutExtension(ProjectPath) + extensionsByConfigType[config_type];
                }
                else
                {
                    toolNode = configNode.SelectSingleNode("Tool[@Name='VCNMakeTool']");
                    if (toolNode != null)
                        assemblyName = Path.GetFileName(RequiredAttributeValue(toolNode, "Output"));
                }

                assemblyName = assemblyName.Replace("$(OutDir)", outputPath);
                assemblyName = assemblyName.Replace("$(ProjectName)", Name);

                _configs.Add(name, new ProjectConfig(this, name, outputPath, assemblyName));
            }
        }

        private void ThrowInvalidFileType(string projectPath)
        {
            throw new ArgumentException(
                string.Format("Invalid project file type: {0}",
                                Path.GetFileName(projectPath)));
        }

        private void ThrowInvalidFormat(string projectPath, Exception e)
        {
            throw new ArgumentException(
                string.Format("Invalid project file format: {0}",
                                Path.GetFileName(projectPath)), e);
        }

        private string SafeAttributeValue(XmlNode node, string attrName)
        {
            XmlNode attrNode = node.Attributes[attrName];
            return attrNode == null ? null : attrNode.Value;
        }

        private string RequiredAttributeValue(XmlNode node, string name)
        {
            string result = SafeAttributeValue(node, name);
            if (result != null)
                return result;

            throw new ApplicationException("Missing required attribute " + name);
        }

        private static string GetConfigNameFromCondition(XmlElement configNode)
        {
            string configurationName = null;
            XmlAttribute conditionAttribute = configNode.Attributes["Condition"];
            if (conditionAttribute != null)
            {
                string condition = conditionAttribute.Value;
                if (condition.IndexOf("$(Configuration)") >= 0)
                {
                    int start = condition.IndexOf("==");
                    if (start >= 0)
                    {
                        configurationName = condition.Substring(start + 2).Trim(new char[] { ' ', '\'' });
                        if (configurationName.EndsWith("|AnyCPU"))
                            configurationName = configurationName.Substring(0, configurationName.Length - 7);
                    }
                }
            }
            return configurationName;
        }

        #endregion

        #region Nested ProjectConfig Class

        private class ProjectConfig
        {
            private IProject _project;
            private string _outputPath;
            private string _assemblyName;

            public ProjectConfig(IProject project, string name, string outputPath, string assemblyName)
            {
                _project = project;
                _outputPath = outputPath;
                _assemblyName = assemblyName;
            }

            public string OutputDirectory
            {
                get { return Normalize(Path.Combine(Path.GetDirectoryName(_project.ProjectPath), _outputPath));  }
            }

            public string AssemblyPath
            {
                get { return Normalize(Path.Combine(OutputDirectory, _assemblyName)); }
            }

            private static string Normalize(string path)
            {
                char sep = Path.DirectorySeparatorChar;

                if (sep != '\\')
                    path = path.Replace('\\', sep);

                return path;
            }
        }

        #endregion
    }
}
