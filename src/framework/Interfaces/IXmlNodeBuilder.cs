using System;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// An object implementing IXmlNodeBuilder is able to build 
    /// an XmlNode representation of itself and any children.
    /// </summary>
    public interface IXmlNodeBuilder
    {
        /// <summary>
        /// Returns an XmlNode representing the current object.
        /// </summary>
        /// <param name="recursive">If true, children are included where applicable</param>
        /// <returns>An XmlNode representing the result</returns>
        XmlNode ToXml(bool recursive);

        /// <summary>
        /// Returns an XmlNode representing the current object after 
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, children are included, where applicable</param>
        /// <returns></returns>
        XmlNode AddToXml(XmlNode parentNode, bool recursive);
    }
}
