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

        public ExtensionPoint GetExtensionPoint(string path)
        {
            return _pathIndex[path];
        }

        public ExtensionPoint GetExtensionPoint(Type type)
        {
            foreach (var ep in _extensionPoints)
                if (ep.Type == type)
                    return ep;

            return null;
        }

        public ExtensionPoint GetExtensionPoint(TypeReference type)
        {
            foreach (var ep in _extensionPoints)
                if (ep.Type.FullName == type.FullName)
                    return ep;

            return null;
        }

        public T[] GetExtensions<T>()
        {
            var extensions = new List<T>();

            var ep = GetExtensionPoint(typeof(T));
            foreach (var node in ep.Extensions)
                extensions.Add((T)node.ExtensionObject);

            return extensions.ToArray();
        }

        #region Service Overrides

        public override void StartService()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var startDir = new DirectoryInfo(AssemblyHelper.GetDirectoryName(thisAssembly));

            FindExtensionPoints(thisAssembly);
            FindExtensionsInDirectory(startDir);

            base.StartService();
        }

        #endregion

        #region Helper Methods

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
                    Description = attr.Description
                };

                _extensionPoints.Add(ep);
                _pathIndex.Add(ep.Path, ep);
            }
        }

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

        private void ProcessAddinsFile(DirectoryInfo baseDir, string fileName)
        {
#if true
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
#else
            var doc = new XmlDocument();

            using (var rdr = new StreamReader(fileName))
            {
                doc.Load(rdr);
                foreach (XmlNode dirNode in doc.SelectNodes("Addins/Directory"))
                {
                    var path = Path.Combine(baseDir.FullName, dirNode.InnerText);
                    FindExtensionsInDirectory(new DirectoryInfo(path));
                }

                foreach (XmlNode addinNode in doc.SelectNodes("Addins/Addin"))
                {
                    foreach (var addin in baseDir.GetFiles(addinNode.InnerText))
                        FindExtensionsInAssembly(addin.FullName);
                }
            }
#endif
        }

        private void FindExtensionsInAssembly(string assemblyName)
        {
            var module = AssemblyDefinition.ReadAssembly(assemblyName).MainModule;
            foreach (var type in module.GetTypes())
                foreach (var attr in type.CustomAttributes)
                    if (attr.AttributeType.FullName == "NUnit.Engine.Extensibility.ExtensionAttribute")
                    {
                        var node = new ExtensionNode(assemblyName, type.FullName);
                        foreach (var x in attr.Properties)
                        {
                            switch (x.Name)
                            {
                                case "Path":
                                    node.Path = x.Argument.Value as string;
                                    break;
                                case "Description":
                                    node.Description = x.Argument.Value as string;
                                    break;
                            }
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

                        ep.Extensions.Add(node);
                    }
        }

        private ExtensionPoint DeduceExtensionPointFromType(TypeReference type)
        {
            var ep = GetExtensionPoint(type);
            if (ep != null)
                return ep;

            foreach (var iface in type.Resolve().Interfaces)
            {
                ep = DeduceExtensionPointFromType(iface);
                if (ep != null)
                    return ep;
            }

            var baseType = type.Resolve().BaseType;
            return baseType != null && baseType.FullName != "System.Object"
                ? DeduceExtensionPointFromType(baseType)
                : null;
        }

        #endregion
    }
}
