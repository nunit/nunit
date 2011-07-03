// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Engine
{
    public enum BinPathType
    {
        Auto,
        Manual,
        None
    }

    public interface IProjectConfig
    {
        string Name { get; }

        System.Collections.Generic.IDictionary<string, object> Settings { get; }

        string[] Assemblies { get; }
    }
}
