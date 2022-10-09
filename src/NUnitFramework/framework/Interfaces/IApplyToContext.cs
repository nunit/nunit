// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IApplyToContext interface is implemented by attributes
    /// that want to make changes to the execution context before
    /// a test is run.
    /// </summary>
    public interface IApplyToContext
    {
        /// <summary>
        /// Apply changes to the execution context
        /// </summary>
        /// <param name="context">The execution context</param>
        void ApplyToContext(TestExecutionContext context);
    }
}
