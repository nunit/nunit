// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Common;

namespace NUnitLite
{
    /// <summary>
    /// NUnitLiteOptions encapsulates the option settings for NUnitLite.
    /// Currently, there are no additional options beyond those common
    /// options that are shared with nunit3-console. If NUnitLite should
    /// acquire some unique options, they should be placed here.
    /// </summary>
    public class NUnitLiteOptions : CommandLineOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NUnitLiteOptions(params string[] args) : base(false, args) { }

        public NUnitLiteOptions(bool requireInputFile, params string[] args) : base(requireInputFile, args) { }

        // Currently used only by test
        internal NUnitLiteOptions(IDefaultOptionsProvider provider, params string[] args) : base(provider, false, args) { }
    }
}
