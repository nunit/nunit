// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InternalTraceLevel is an enumeration controlling the
    /// level of detailed presented in the internal log.
    /// </summary>
    public enum InternalTraceLevel
    {
        /// <summary>
        /// Use the default settings as specified by the user.
        /// </summary>
        Default,

        /// <summary>
        /// Do not display any trace messages
        /// </summary>
        Off,

        /// <summary>
        /// Display Error messages only
        /// </summary>
        Error,

        /// <summary>
        /// Display Warning level and higher messages
        /// </summary>
        Warning,

        /// <summary>
        /// Display informational and higher messages
        /// </summary>
        Info,

        /// <summary>
        /// Display debug messages and higher - i.e. all messages
        /// </summary>
        Debug,

        /// <summary>
        /// Display debug messages and higher - i.e. all messages
        /// </summary>
        Verbose = Debug
    }
}
