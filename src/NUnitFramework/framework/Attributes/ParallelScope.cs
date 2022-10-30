// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.ComponentModel;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies the degree to which a test, and its descendants, 
    /// may be run in parallel.
    /// </summary>
    [Flags]
    public enum ParallelScope
    {
        /// <summary>
        /// No ParallelScope was specified on the test
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Default = 0,

        /// <summary>
        /// The test may be run in parallel with others at the same level.
        /// Valid on classes and methods but has no effect on assemblies.
        /// </summary>
        Self = 1,

        /// <summary>
        /// Test may not be run in parallel with any others. Valid on
        /// classes and methods but not assemblies.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        None = 2,

        /// <summary>
        /// Mask used to extract the flags that apply to the item on which a
        /// ParallelizableAttribute has been placed, as opposed to descendants.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ItemMask = Self + None,

        /// <summary>
        /// Descendants of the test may be run in parallel with one another.
        /// Valid on assemblies and classes but not on non-parameterized methods.
        /// </summary>
        Children = 256,

        /// <summary>
        /// Descendants of the test down to the level of TestFixtures may be 
        /// run in parallel with one another. Valid on assemblies and classes
        /// but not on methods.
        /// </summary>
        Fixtures = 512,

        /// <summary>
        /// Mask used to extract all the flags that impact descendants of a 
        /// test and place them in the TestExecutionContext.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ContextMask = Children + Fixtures,

        /// <summary>
        /// The test and its descendants may be run in parallel with others at
        /// the same level. Valid on classes and parameterized methods.
        /// For assemblies it is recommended to use <see cref="Children"/>
        /// instead, as <see cref="Self"/> has no effect on assemblies.
        /// </summary>
        All = Self + Children
    }
}
