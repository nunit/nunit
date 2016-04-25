namespace NUnit.Engine
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
        Net,
        /// <summary>Microsoft .NET Compact Framework</summary>
        NetCF,
        /// <summary>Mono</summary>
        Mono,
        /// <summary>Silverlight</summary>
        Silverlight
    }
}
