using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// Interface implemented by objects that can create an IResultWriter for a given format.
    /// </summary>
    public interface IResultWriterFactory
    {
        /// <summary>
        /// Gets the supported format
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Creates an IResultWriter using the args provided
        /// </summary>
        IResultWriter GetResultWriter(object[] args);
    }
}
