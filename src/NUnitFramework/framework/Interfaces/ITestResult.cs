// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestResult interface represents the result of a test.
    /// </summary>
    public interface ITestResult : IXmlNodeBuilder
    {
        /// <summary>
        /// Gets the ResultState of the test result, which
        /// indicates the success or failure of the test.
        /// </summary>
        ResultState ResultState
        {
            get;
        }

        /// <summary>
        /// Gets the name of the test result
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the full name of the test result
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// Gets the elapsed time for running the test in seconds
        /// </summary>
        double Duration
        {
            get;
        }

        /// <summary>
        /// Gets or sets the time the test started running.
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// Gets or sets the time the test finished running.
        /// </summary>
        DateTime EndTime
        {
            get;
        }

        /// <summary>
        /// Gets the message associated with a test
        /// failure or with not running the test
        /// </summary>
        string? Message
        {
            get;
        }

        /// <summary>
        /// Gets any stack trace associated with an
        /// error or failure.</summary>
        string? StackTrace
        {
            get;
        }

        /// <summary>
        /// Gets the total number of tests executed
        /// when running the test and all its children.
        /// </summary>
        int TotalCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of asserts executed
        /// when running the test and all its children.
        /// </summary>
        int AssertCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        int FailCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of test cases that had warnings
        /// when running the test and all its children.
        /// </summary>
        int WarningCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        int PassCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        int SkipCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        int InconclusiveCount
        {
            get;
        }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// Accessing HasChildren should not force creation of the
        /// Children collection in classes implementing this interface.
        /// </summary>
        bool HasChildren
        {
            get;
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        IEnumerable<ITestResult> Children
        {
            get;
        }

        /// <summary>
        /// Gets the Test to which this result applies.
        /// </summary>
        ITest Test
        {
            get;
        }

        /// <summary>
        /// Gets any text output written to this result.
        /// </summary>
        string Output
        {
            get;
        }

        /// <summary>
        /// Gets a list of AssertionResults associated with the test
        /// </summary>
        IList<AssertionResult> AssertionResults
        {
            get;
        }

        /// <summary>
        /// Gets the collection of files attached to the test
        /// </summary>
        ICollection<TestAttachment> TestAttachments
        {
            get;
        }
    }
}
