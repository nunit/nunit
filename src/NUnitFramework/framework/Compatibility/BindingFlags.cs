#if PORTABLE
using System;

namespace NUnit.Compatibility
{
    /// <summary>
    /// Specifies flags that control binding and the way in which the search for members
    /// and types is conducted by reflection.
    /// </summary>
    [Flags]
    public enum BindingFlags
    {
        ///<summary>
        /// Specifies no binding flag.
        /// </summary>
        Default = 0,

        ///<summary>
        /// Specifies that only members declared at the level of the supplied type's hierarchy
        /// should be considered. Inherited members are not considered.
        /// </summary>
        DeclaredOnly = 2,
        
        ///<summary>
        /// Specifies that instance members are to be included in the search.
        /// </summary>
        Instance = 4,

        ///<summary>
        /// Specifies that static members are to be included in the search.
        /// </summary>
        Static = 8,

        ///<summary>
        /// Specifies that public members are to be included in the search.
        /// </summary>
        Public = 16,

        ///<summary>
        /// Specifies that non-public members are to be included in the search.
        /// </summary>
        NonPublic = 32,

        ///<summary>
        /// Specifies that public and protected static members up the hierarchy should be
        /// returned. Private static members in inherited classes are not returned. Static
        /// members include fields, methods, events, and properties. Nested types are not
        /// returned.
        /// </summary>
        FlattenHierarchy = 64
    }
}
#endif