// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// FileOrDirectoryExistsConstraint is used to determine if a file or directory exists
    /// </summary>
    public class FileOrDirectoryExistsConstraint : Constraint
    {
        private bool _ignoreDirectories;
        private bool _ignoreFiles;

        /// <summary>
        /// If true, the constraint will only check if files exist, not directories
        /// </summary>
        public FileOrDirectoryExistsConstraint IgnoreDirectories
        {
            get
            {
                _ignoreDirectories = true;
                return this;
            }
        }

        /// <summary>
        /// If true, the constraint will only check if directories exist, not files
        /// </summary>
        public FileOrDirectoryExistsConstraint IgnoreFiles
        {
            get
            {
                _ignoreFiles = true;
                return this;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOrDirectoryExistsConstraint"/> class that
        /// will check files and directories.
        /// </summary>
        public FileOrDirectoryExistsConstraint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOrDirectoryExistsConstraint"/> class that
        /// will only check files if ignoreDirectories is true.
        /// </summary>
        /// <param name="ignoreDirectories">if set to <see langword="true"/> [ignore directories].</param>
        public FileOrDirectoryExistsConstraint(bool ignoreDirectories)
        {
            _ignoreDirectories = ignoreDirectories;
        }

        #region Overrides of Constraint

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                if (_ignoreDirectories)
                {
                    return "file exists";
                }
                if (_ignoreFiles)
                {
                    return "directory exists";
                }
                return "file or directory exists";
            }
        }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            ArgumentNullException.ThrowIfNull(actual);

            if (actual is string stringValue)
            {
                return CheckString(stringValue);
            }

            if (!_ignoreFiles && actual is FileInfo fileInfo)
            {
                return new ConstraintResult(this, actual, fileInfo.Exists);
            }

            if (!_ignoreDirectories && actual is DirectoryInfo directoryInfo)
            {
                return new ConstraintResult(this, actual, directoryInfo.Exists);
            }
            throw new ArgumentException("The actual value must be a string" + ErrorSubstring, nameof(actual));
        }

        private ConstraintResult CheckString(string actual)
        {
            if (string.IsNullOrEmpty(actual))
                throw new ArgumentException("The actual value cannot be an empty string", nameof(actual));

            var fileInfo = new FileInfo(actual);
            if (_ignoreDirectories && !_ignoreFiles)
            {
                return new ConstraintResult(this, actual, fileInfo.Exists);
            }
            var directoryInfo = new DirectoryInfo(actual);
            if (_ignoreFiles && !_ignoreDirectories)
            {
                return new ConstraintResult(this, actual, directoryInfo.Exists);
            }
            return new ConstraintResult(this, actual, fileInfo.Exists || directoryInfo.Exists);
        }

        private string ErrorSubstring
        {
            get
            {
                if (_ignoreDirectories)
                {
                    return " or FileInfo";
                }
                if (_ignoreFiles)
                {
                    return " or DirectoryInfo";
                }
                return ", FileInfo or DirectoryInfo";
            }
        }

        #endregion

    }
}
