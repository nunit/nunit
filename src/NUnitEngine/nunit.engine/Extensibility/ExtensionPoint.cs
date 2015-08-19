// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Reflection;
using System.Collections.Generic;

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// ExtensionPoint is used as a base class for all 
    /// extension points.
    /// </summary>
    public class ExtensionPoint
    {
        public ExtensionPoint(string path, Type type)
        {
            Path = path;
            Type = type;
            Extensions = new List<ExtensionNode>();
        }

        public string Path { get; private set; }

        public Type Type { get; private set; }

        public string Description { get; set; }

        public List<ExtensionNode> Extensions { get; private set; }

        ///// <summary>
        ///// Install an extension at this extension point. If the
        ///// extension object does not meet the requirements for
        ///// this extension point, an exception is thrown.
        ///// </summary>
        ///// <param name="extension">The extension to install</param>
        //public void Install(object extension)
        //{
        //    Extensions.Add(extension);
        //}

        ///// <summary>
        ///// Removes an extension from this extension point. If the
        ///// extension object is not present, the method returns
        ///// without error.
        ///// </summary>
        ///// <param name="extension"></param>
        //public void Remove(object extension)
        //{
        //    Extensions.Remove( extension );
        //}
    }

    public class ExtensionNode
    {
        Assembly _assembly;
        string _assemblyPath;
        string _typeName;

        public ExtensionNode(string assemblyPath, string typeName)
        {
            _assemblyPath = assemblyPath;
            _typeName = typeName;
        }

        public string AssemblyPath
        {
            get { return _assemblyPath;  }
        }

        public string TypeName
        {
            get { return _typeName;  }
        }
    }
}
