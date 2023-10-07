// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// Base class for different Assert, containing helper functions
    /// </summary>
    public abstract class AssertBase
    {
        /// <summary>
        /// Check if message comes with args, and convert that to a formatted string
        /// </summary>
        protected static string ConvertMessageWithArgs(string message, object?[]? args)
            => (args is null || args.Length == 0) ? message : string.Format(message, args);
    }
}
