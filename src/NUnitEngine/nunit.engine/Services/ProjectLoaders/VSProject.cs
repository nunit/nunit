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
        private static readonly string[] PROJECT_EXTENSIONS = { ".csproj", ".vbproj", ".vjsproj", ".vcproj" };
        
        /// <summary>
        /// VS Solution extension
        /// </summary>
        private const string SOLUTION_EXTENSION = ".sln";

        #endregion

        #region Constructor

        public VSProject( string projectPath )
        {
            ProjectPath = Path.GetFullPath( projectPath );
            Configs = new List<VSProjectConfig>();

            Load();
        }

        #endregion

        #region IProject Members

        /// <summary>
        /// The path to the project
        /// </summary>
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Gets the active configuration, as defined
        /// by the particular project. For a VS
        /// project, we use the first config found.
        /// </summary>
        public string ActiveConfigName
        {
            get
            {
                return Configs.Count > 0
                    ? Configs[0].Name
                    : null;
            }
        }

        public IList<string> ConfigNames
        {
            get
            {
                var result = new List<string>();
                foreach (var config in Configs)
                    result.Add(config.Name);
                return result;
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
            foreach (var config in Configs)
            {
                if (configName == null || configName == config.Name)
                {
                    foreach (string assembly in config.Assemblies)
                    {
                        package.Add(assembly);
                        appbase = Path.GetDirectoryName(assembly);
                    }
                    break;
                }
            }

            if (appbase != null)
                package.Settings["BasePath"] = appbase;

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

        /// <summary>
        /// The list of all our configs
        /// </summary>
        public List<VSProjectConfig> Configs { get; private set; }

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

        private void Load()
        {
            if ( !IsProjectFile( ProjectPath ) ) 
                ThrowInvalidFileType( ProjectPath );

            string projectDirectory = Path.GetFullPath(Path.GetDirectoryName(ProjectPath));
            StreamReader rdr = new StreamReader(ProjectPath, System.Text.Encoding.UTF8);
            string[] extensions = { "", ".exe", ".dll", ".lib", "" };
            
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( rdr );

                string extension = Path.GetExtension( ProjectPath );
                string assemblyName = null;

                switch ( extension )
                {
                    case ".vcproj":

                        // TODO: This is all very hacked up... replace it.
                        foreach (XmlNode configNode in doc.SelectNodes("/VisualStudioProject/Configurations/Configuration"))
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

                            string outputDirectory = Path.Combine(projectDirectory, outputPath);
                            XmlNode toolNode = configNode.SelectSingleNode("Tool[@Name='VCLinkerTool']");
                            if (toolNode != null)
                            {
                                assemblyName = SafeAttributeValue(toolNode, "OutputFile");
                                if (assemblyName != null)
                                    assemblyName = Path.GetFileName(assemblyName);
                                else
                                    assemblyName = Path.GetFileNameWithoutExtension(ProjectPath) + extensions[config_type];
                            }
                            else
                            {
                                toolNode = configNode.SelectSingleNode("Tool[@Name='VCNMakeTool']");
                                if (toolNode != null)
                                    assemblyName = Path.GetFileName(RequiredAttributeValue(toolNode, "Output"));
                            }

                            assemblyName = assemblyName.Replace("$(OutDir)", outputPath);
                            assemblyName = assemblyName.Replace("$(ProjectName)", this.Name);

                            VSProjectConfig config = new VSProjectConfig(name);
                            if (assemblyName != null)
                                config.Assemblies.Add(Path.Combine(outputDirectory, assemblyName));

                            this.Configs.Add(config);
                        }

                        break;

                    case ".csproj":
                    case ".vbproj":
                    case ".vjsproj":
                        LoadProject(projectDirectory, doc);
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

        private bool LoadProject(string projectDirectory, XmlDocument doc)
        {
            bool loaded = LoadVS2003Project(projectDirectory, doc);
            if (loaded) return true;

            loaded = LoadMSBuildProject(projectDirectory, doc);
            if (loaded) return true;

            return false;
        }

        private bool LoadVS2003Project(string projectDirectory, XmlDocument doc)
        {
            XmlNode settingsNode = doc.SelectSingleNode("/VisualStudioProject/*/Build/Settings");
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
                    string outputDirectory = Path.Combine(projectDirectory, outputPath);
                    string assemblyPath = Path.Combine(outputDirectory, assemblyName);

                    VSProjectConfig config = new VSProjectConfig(name);
                    config.Assemblies.Add(assemblyPath);

                    Configs.Add(config);
                }

            return true;
        }

        private bool LoadMSBuildProject(string projectDirectory, XmlDocument doc)
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNodeList nodes = doc.SelectNodes("/msbuild:Project/msbuild:PropertyGroup", namespaceManager);
            if (nodes == null) return false;

            XmlElement assemblyNameElement = (XmlElement)doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", namespaceManager);
            string assemblyName = assemblyNameElement.InnerText;

            XmlElement outputTypeElement = (XmlElement)doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:OutputType", namespaceManager);
            string outputType = outputTypeElement.InnerText;

            if (outputType == "Exe" || outputType == "WinExe")
                assemblyName = assemblyName + ".exe";
            else
                assemblyName = assemblyName + ".dll";

            string commonOutputPath = null;

            foreach (XmlElement configNode in nodes)
            {
                if (configNode.Name != "PropertyGroup")
                    continue;

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

                XmlElement outputPathElement = (XmlElement)configNode.SelectSingleNode("msbuild:OutputPath", namespaceManager);
                string outputPath = null;
                if (outputPathElement != null)
                    outputPath = outputPathElement.InnerText;

                if (configurationName == null)
                {
                    commonOutputPath = outputPath;
                    continue;
                }

                if (outputPath == null)
                    outputPath = commonOutputPath;

                if (outputPath == null) continue;

                string outputDirectory = Path.Combine(projectDirectory, outputPath);
                string assemblyPath = Path.Combine(outputDirectory, assemblyName);

                VSProjectConfig config = new VSProjectConfig(configurationName);
                config.Assemblies.Add(assemblyPath);

                Configs.Add(config);
            }

            return true;
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

        #endregion
    }
}
