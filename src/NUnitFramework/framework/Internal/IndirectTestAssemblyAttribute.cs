using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Causes the assembly to be skipped when autodiscovering tests.
    /// Useful for tests-on-tests scenarios so that tools that run all tests in the solution do not execute the indirect tests directly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class IndirectTestAssemblyAttribute : Attribute
    {
    }
}
