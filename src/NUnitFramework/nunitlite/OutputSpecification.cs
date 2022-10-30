// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Common
{
    /// <summary>
    /// OutputSpecification encapsulates a file output path and format
    /// for use in saving the results of a run.
    /// </summary>
    public class OutputSpecification
    {
        #region Constructor

        /// <summary>
        /// Construct an OutputSpecification from an option value.
        /// </summary>
        /// <param name="spec">The option value string.</param>
        public OutputSpecification(string spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec), "Output spec may not be null.");

            string[] parts = spec.Split(';');

            if (parts.Length > 2)
                throw new ArgumentException($"Invalid output spec: {spec}.");

            this.OutputPath = parts[0];

            if (parts.Length == 1)
                return;

            string[] opt = parts[1].Split('=');

            if (opt.Length != 2 || opt[0].Trim() != "format")
                throw new ArgumentException($"Invalid output spec: {spec}.");

            this.Format = opt[1].Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to which output will be written
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Gets the name of the format to be used
        /// </summary>
        public string Format { get; } = "nunit3";

        #endregion
    }
}
