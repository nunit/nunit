// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test assembly as needing a special assembly resolution hook that will 
    /// explicitly search the test assembly's directory for dependent assemblies. 
    /// This works around a conflict between mixed-mode assembly initialization and 
    /// tests running in their own AppDomain in some cases.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public class TestAssemblyDirectoryResolveAttribute : NUnitAttribute
    {
    }

}
