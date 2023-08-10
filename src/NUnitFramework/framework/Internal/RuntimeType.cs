// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Enumeration identifying a common language
    /// runtime implementation.
    /// </summary>
    public enum RuntimeType
    {
        /// <summary>Any supported runtime framework</summary>
        Any,
        /// <summary>Microsoft .NET Framework</summary>
        NetFramework,
        /// <summary>Microsoft Shared Source CLI</summary>
        SSCLI,
        /// <summary>Mono</summary>
        Mono,
        /// <summary>MonoTouch</summary>
        MonoTouch,
        /// <summary>Microsoft .NET Core, including .NET 5+</summary>
        NetCore
    }
}
