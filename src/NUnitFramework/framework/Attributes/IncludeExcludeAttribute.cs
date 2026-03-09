// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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
        public IncludeExcludeAttribute()
        {
        }

        /// <summary>
        /// Constructor taking one or more included items
        /// </summary>
        /// <param name="include">Comma-delimited list of included items</param>
        public IncludeExcludeAttribute(string? include)
        {
            _include = include;
        }

        /// <summary>
        /// Constructor taking an array of included items
        /// </summary>
        /// <param name="includes">Array included items</param>
        public IncludeExcludeAttribute(string[] includes)
        {
            Includes = includes;
        }

        /// <summary>
        /// Name of the item that is needed in order for
        /// a test to run. Multiple items may be given,
        /// separated by a comma.
        /// </summary>
        public string? Include
        {
            get => _include;
            set => _include = value;
        }

        /// <summary>
        /// Name of the item to be excluded. Multiple items
        /// may be given, separated by a comma.
        /// </summary>
        public string? Exclude
        {
            get => _exclude;
            set => _exclude = value;
        }

        private static readonly char[] CommaCharacter = [','];

        /// <summary>
        /// An array of items to be included. This is a helper that assigns a
        /// comma-separated list to the <see cref="Include" /> property.
        /// </summary>
        public string[] Includes
        {
            get => _include?.Split(CommaCharacter, StringSplitOptions.RemoveEmptyEntries) ?? [];
            set => _include = string.Join(",", value);
        }

        /// <summary>
        /// An array of items to be excluded. This is a helper that assigns a
        /// comma-separated list to the <see cref="Exclude" /> property.
        /// </summary>
        public string[] Excludes
        {
            get => _exclude?.Split(CommaCharacter, StringSplitOptions.RemoveEmptyEntries) ?? [];
            set => _exclude = string.Join(",", value);
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
