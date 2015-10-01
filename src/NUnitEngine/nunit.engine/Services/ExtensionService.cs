// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Reflection;
using System.Xml;
using Mono.Cecil;
using NUnit.Engine.Extensibility;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// The ExtensionService discovers ExtensionPoints and Extensions and
    /// maintains them in a database. It can return extension nodes or 
    /// actual extension objects on request.
    /// </summary>
    public class ExtensionService : Service
    {
        private List<ExtensionPoint> _extensionPoints = new List<ExtensionPoint>();
        private Dictionary<string, ExtensionPoint> _pathIndex = new Dictionary<string, ExtensionPoint>();

        private List<ExtensionNode> _extensions = new List<ExtensionNode>();

        #region Public Properties

        public IEnumerable<ExtensionPoint> ExtensionPoints
        {
            get { return _extensionPoints; }
        }

        public IEnumerable<ExtensionNode> Extensions
        {
            get { return _extensions;  }
        }

        #endregion

        #region Public Methods - Extension Points

        /// <summary>
        /// Get an ExtensionPoint based on it's unique identifying path.
        /// </summary>
        public ExtensionPoint GetExtensionPoint(string path)
        {
            return _pathIndex.ContainsKey(path) ? _pathIndex[path] : null;
        }

        /// <summary>
        /// Get an ExtensionPoint based on the required Type for extensions.
        /// </summary>
        public ExtensionPoint GetExtensionPoint(Type type)
        {
            foreach (var ep in _extensionPoints)
                if (ep.Type == type)
                    return ep;

            return null;
        }

        /// <summary>
        /// Get an ExtensionPoint based on a Cecil TypeReference.
        /// </summary>
        public ExtensionPoint GetExtensionPoint(TypeReference type)
        {
            foreach (var ep in _extensionPoints)
                if (ep.Type.FullName == type.FullName)
                    return ep;

            return null;
        }

        #endregion

        #region Public Methods - Extensions

        public IEnumerable<ExtensionNode> GetExtensionNodes(string path)
        {
            var ep = GetExtensionPoint(path);
            if (ep != null)
                foreach (var node in ep.Extensions)
                    yield return node;
        }

        public ExtensionNode GetExtensionNode(string path)
        {
            var ep = GetExtensionPoint(path);

            return ep != null && ep.Extensions.Count > 0 ? ep.Extensions[0] : null;
        }

        public IEnumerable<ExtensionNode> GetExtensionNodes<T>()
        {
            var ep = GetExtensionPoint(typeof(T));
            if (ep != null)
                foreach (var node in ep.Extensions)
                    yield return node;
        }

        public IEnumerable<T> GetExtensions<T>()
        {
            foreach (var node in GetExtensionNodes<T>())
                yield return (T)node.ExtensionObject;
        }

        #endregion

        #region Service Overrides

        public override void StartService()
        {
            try
            {
                var thisAssembly = Assembly.GetExecutingAssembly();
                var apiAssembly = typeof(ITestEngine).Assembly;
                var startDir = new DirectoryInfo(AssemblyHelper.GetDirectoryName(thisAssembly));

                FindExtensionPoints(thisAssembly);
                FindExtensionPoints(apiAssembly);
                FindExtensionsInDirectory(startDir);

                Status = ServiceStatus.Started;
            }
            catch
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        #endregion

        #region Helper Methods - Extension Points

        /// <summary>
        /// Find the extension points in a loaded assembly
        /// </summary>
        private void FindExtensionPoints(Assembly assembly)
        {
            foreach (ExtensionPointAttribute attr in assembly.GetCustomAttributes(typeof(ExtensionPointAttribute), false))
            {
                if (_pathIndex.ContainsKey(attr.Path))
                {
                    string msg = string.Format(
                        "The Path {0} is already in use for another extension point.",
                        attr.Path);
                    throw new NUnitEngineException(msg);
                }

                var ep = new ExtensionPoint(attr.Path, attr.Type)
                {
                    Description = attr.Description,
                };

                _extensionPoints.Add(ep);
                _pathIndex.Add(ep.Path, ep);
            }

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (TypeExtensionPointAttribute attr in type.GetCustomAttributes(typeof(TypeExtensionPointAttribute), false))
                {
                    string path = attr.Path ?? "/NUnit/Engine/TypeExtensions/" + type.Name;

                    if (_pathIndex.ContainsKey(path))
                    {
                        string msg = string.Format(
                            "The Path {0} is already in use for another extension point.",
                            attr.Path);
                        throw new NUnitEngineException(msg);
                    }

                    var ep = new ExtensionPoint(path, type)
                    {
                        Description = attr.Description,
                    };

                    _extensionPoints.Add(ep);
                    _pathIndex.Add(path, ep);
                }
            }
        }

        /// <summary>
        /// Deduce the extension point based on the Type of an extension. 
        /// Returns null if no extension point can be found that would
        /// be satisfied by the provided Type.
        /// </summary>
        private ExtensionPoint DeduceExtensionPointFromType(TypeReference typeRef)
        {
            var ep = GetExtensionPoint(typeRef);
            if (ep != null)
                return ep;

            TypeDefinition typeDef = typeRef.Resolve();

            foreach (TypeReference iface in typeDef.Interfaces)
            {
                ep = DeduceExtensionPointFromType(iface);
                if (ep != null)
                    return ep;
            }

            TypeReference baseType = typeDef.BaseType;
            return baseType != null && baseType.FullName != "System.Object"
                ? DeduceExtensionPointFromType(baseType)
                : null;
        }

        #endregion

        #region Helper Methods - Extensions

        /// <summary>
        /// Scans a directory for addins. Note that assemblies in the directory
        /// are only scanned if no file of type .addins is found. If such a file
        /// is found, then those assemblies it references are scanned.
        /// </summary>
        private void FindExtensionsInDirectory(DirectoryInfo startDir)
        {
            var addinsFiles = startDir.GetFiles("*.addins");
            if (addinsFiles.Length > 0)
                foreach (var file in addinsFiles)
                    ProcessAddinsFile(startDir, file.FullName);
            else
                foreach (var file in startDir.GetFiles("*.dll"))
                    FindExtensionsInAssembly(file.FullName);
        }

        /// <summary>
        /// Process a .addins type file. The file contains one entry per
        /// line. Each entry may be a directory to scan, an assembly
        /// path or a wildcard pattern used to find assemblies. Blank
        /// lines and comments started by # are ignored.
        /// </summary>
        private void ProcessAddinsFile(DirectoryInfo baseDir, string fileName)
        {
            using (var rdr = new StreamReader(fileName))
            {
                while (!rdr.EndOfStream)
                {
                    var line = rdr.ReadLine();
                    if (line == null)
                        break;

                    line = line.Split(new char[] { '#' })[0].Trim();

                    if (line == string.Empty)
                        continue;

                    if (line.Contains("*"))
                        foreach (var file in baseDir.GetFiles(line))
                            FindExtensionsInAssembly(file.FullName);
                    else
                    {
                        var path = Path.Combine(baseDir.FullName, line);
                        if (Directory.Exists(path))
                            FindExtensionsInDirectory(new DirectoryInfo(path));
                        else if (File.Exists(path))
                            FindExtensionsInAssembly(path);
                    }
                }
            }
        }

        /// <summary>
        /// Scan a single assembly for extensions marked by ExtensionAttribute.
        /// For each extension, create an ExtensionNode and link it to the
        /// correct ExtensionPoint.
        /// </summary>
        private void FindExtensionsInAssembly(string assemblyName)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyName));
            var parameters = new ReaderParameters() { AssemblyResolver = resolver };
            var module = AssemblyDefinition.ReadAssembly(assemblyName, parameters).MainModule;
            foreach (var type in module.GetTypes())
            {
                CustomAttribute extensionAttr = type.GetAttribute("NUnit.Engine.Extensibility.ExtensionAttribute");

                if (extensionAttr != null)
                {
                    var node = new ExtensionNode(assemblyName, type.FullName);
                    node.Path = extensionAttr.GetNamedArgument("Path") as string;
                    node.Description = extensionAttr.GetNamedArgument("Description") as string;

                    foreach (var attr in type.GetAttributes("NUnit.Engine.Extensibility.ExtensionPropertyAttribute"))
                    {
                        string name = attr.ConstructorArguments[0].Value as string;
                        string value = attr.ConstructorArguments[1].Value as string;

                        if (name != null && value != null)
                            node.AddProperty(name, value);
                    }

                    _extensions.Add(node);

                    ExtensionPoint ep;
                    if (node.Path == null)
                    {
                        ep = DeduceExtensionPointFromType(type);
                        if (ep == null)
                        {
                            string msg = string.Format(
                                "Unable to deduce ExtensionPoint for Type {0}. Specify Path on ExtensionAttribute to resolve.",
                                type.FullName);
                            throw new NUnitEngineException(msg);
                        }

                        node.Path = ep.Path;
                    }
                    else
                    {
                        ep = GetExtensionPoint(node.Path);
                        if (ep == null)
                        {
                            string msg = string.Format(
                                "Unable to locate ExtensionPoint for Type {0}. The Path {1} cannot be found.",
                                type.FullName,
                                node.Path);
                            throw new NUnitEngineException(msg);
                        }
                    }

                    ep.Install(node);
                }
            }
        }

        #endregion

    }
}
