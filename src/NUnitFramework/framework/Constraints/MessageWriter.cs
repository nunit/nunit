// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// MessageWriter is the abstract base for classes that write
    /// constraint descriptions and messages in some form. The
    /// class has separate methods for writing various components
    /// of a message, allowing implementations to tailor the
    /// presentation as needed.
    /// </summary>
    public abstract class MessageWriter : StringWriter
    {

        /// <summary>
        /// Construct a MessageWriter given a culture
        /// </summary>
        protected MessageWriter() : base(System.Globalization.CultureInfo.InvariantCulture) { }

        /// <summary>
        /// Abstract method to get the max line length
        /// </summary>
        public abstract int MaxLineLength { get; set; }

        /// <summary>
        /// Method to write single line message with optional args, usually
        /// written to precede the general failure message.
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">Any arguments used in formatting the message</param>
        public void WriteMessageLine(string message, params object[] args)
        {
            WriteMessageLine(0, message, args);
        }

        /// <summary>
        /// Method to write single line message with optional args, usually
        /// written to precede the general failure message, at a given 
        /// indentation level.
        /// </summary>
        /// <param name="level">The indentation level of the message</param>
        /// <param name="message">The message to be written</param>
        /// <param name="args">Any arguments used in formatting the message</param>
        public abstract void WriteMessageLine(int level, string message, params object[] args);

        /// <summary>
        /// Display Expected and Actual lines for a constraint. This
        /// is called by MessageWriter's default implementation of 
        /// WriteMessageTo and provides the generic two-line display. 
        /// </summary>
        /// <param name="result">The failing constraint result</param>
        public abstract void DisplayDifferences(ConstraintResult result);

        /// <summary>
        /// Display Expected and Actual lines for given values. This
        /// method may be called by constraints that need more control over
        /// the display of actual and expected values than is provided
        /// by the default implementation.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value causing the failure</param>
        public abstract void DisplayDifferences(object expected, object actual);

        /// <summary>
        /// Display Expected and Actual lines for given values, including
        /// a tolerance value on the Expected line.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value causing the failure</param>
        /// <param name="tolerance">The tolerance within which the test was made</param>
        public abstract void DisplayDifferences(object expected, object actual, Tolerance tolerance);

        /// <summary>
        /// Display the expected and actual string values on separate lines.
        /// If the mismatch parameter is >=0, an additional line is displayed
        /// line containing a caret that points to the mismatch point.
        /// </summary>
        /// <param name="expected">The expected string value</param>
        /// <param name="actual">The actual string value</param>
        /// <param name="mismatch">The point at which the strings don't match or -1</param>
        /// <param name="ignoreCase">If true, case is ignored in locating the point where the strings differ</param>
        /// <param name="clipping">If true, the strings should be clipped to fit the line</param>
        public abstract void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping);

        /// <summary>
        /// Writes the text for an actual value.
        /// </summary>
        /// <param name="actual">The actual value.</param>
        public abstract void WriteActualValue(object actual);

        /// <summary>
        /// Writes the text for a generalized value.
        /// </summary>
        /// <param name="val">The value.</param>
        public abstract void WriteValue(object val);

        /// <summary>
        /// Writes the text for a collection value,
        /// starting at a particular point, to a max length
        /// </summary>
        /// <param name="collection">The collection containing elements to write.</param>
        /// <param name="start">The starting point of the elements to write</param>
        /// <param name="max">The maximum number of elements to write</param>
        public abstract void WriteCollectionElements(IEnumerable collection, long start, int max);
    }
}
