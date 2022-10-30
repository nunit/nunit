// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.ComponentModel;

namespace NUnit.Framework
{
    /// <summary>
    /// Used by third-party frameworks, or other software, that reference 
    /// the NUnit framework but do not contain any tests. Applying the 
    /// attribute indicates that the assembly is not a test assembly and 
    /// may prevent errors if certain runners attempt to load the assembly. 
    /// Note that recognition of the attribute depends on each individual runner.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class NonTestAssemblyAttribute : NUnitAttribute
    {
    }
}
