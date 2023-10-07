// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The TestAttachment class represents a file attached to a TestResult,
    /// with an optional description.
    /// </summary>
    public class TestAttachment
    {
        /// <summary>
        /// Absolute file path to attachment file
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// User specified description of attachment. May be null.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Creates a TestAttachment class to represent a file attached to a test result.
        /// </summary>
        /// <param name="filePath">Absolute file path to attachment file</param>
        /// <param name="description">User specified description of attachment. May be null.</param>
        public TestAttachment(string filePath, string? description)
        {
            FilePath = filePath;
            Description = description;
        }
    }
}
