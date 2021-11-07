// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// An object implementing IXmlNodeBuilder is able to build 
    /// an XML representation of itself and any children.
    /// </summary>
    public interface IXmlNodeBuilder
    {
        /// <summary>
        /// Returns a TNode representing the current object.
        /// </summary>
        /// <param name="recursive">If true, children are included where applicable</param>
        /// <returns>A TNode representing the result</returns>
        TNode ToXml(bool recursive);

        /// <summary>
        /// Returns a TNode representing the current object after 
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, children are included, where applicable</param>
        /// <returns></returns>
        TNode AddToXml(TNode parentNode, bool recursive);
    }
}
