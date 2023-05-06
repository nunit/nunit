// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base for attributes that are used to include tests in
    /// the test run based on environmental settings.
    /// </summary>
    public abstract class IncludeExcludeAttribute : NUnitAttribute
    {
        private string? _include;
        private string? _exclude;
        private string? _reason;

        /// <summary>
        /// Constructor with no included items specified, for use
        /// with named property syntax.
        /// </summary>
        public IncludeExcludeAttribute() { }

        /// <summary>
        /// Constructor taking one or more included items
        /// </summary>
        /// <param name="include">Comma-delimited list of included items</param>
        public IncludeExcludeAttribute(string? include)
        {
            this._include = include;
        }

        /// <summary>
        /// Name of the item that is needed in order for
        /// a test to run. Multiple items may be given,
        /// separated by a comma.
        /// </summary>
        public string? Include
        {
            get => this._include;
            set => _include = value;
        }

        /// <summary>
        /// Name of the item to be excluded. Multiple items
        /// may be given, separated by a comma.
        /// </summary>
        public string? Exclude
        {
            get => this._exclude;
            set => this._exclude = value;
        }

        /// <summary>
        /// The reason for including or excluding the test
        /// </summary>
        public string? Reason
        {
            get => _reason;
            set => _reason = value;
        }
    }
}
