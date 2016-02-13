using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Enables compiling extension methods in .NET 2.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    sealed class ExtensionAttribute : Attribute { }
}
