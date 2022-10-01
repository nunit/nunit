// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Attaches information to a test assembly, fixture or method as a name/value pair.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public class PropertyAttribute : NUnitAttribute, IApplyToTest
    {
        private readonly PropertyBag properties = new PropertyBag();

        /// <summary>
        /// Construct a PropertyAttribute with a name and string value
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The property value</param>
        public PropertyAttribute(string propertyName, string propertyValue)
        {
            this.properties.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// Construct a PropertyAttribute with a name and int value
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The property value</param>
        public PropertyAttribute(string propertyName, int propertyValue)
        {
            this.properties.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// Construct a PropertyAttribute with a name and double value
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The property value</param>
        public PropertyAttribute(string propertyName, double propertyValue)
        {
            this.properties.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// Constructor for derived classes that set the
        /// property dictionary directly.
        /// </summary>
        protected PropertyAttribute() { }

        /// <summary>
        /// Constructor for use by derived classes that use the
        /// name of the type as the property name. Derived classes
        /// must ensure that the Type of the property value is
        /// a standard type supported by the BCL. Any custom
        /// types will cause a serialization Exception when
        /// in the client.
        /// </summary>
        protected PropertyAttribute( object propertyValue )
        {
            string propertyName = this.GetType().Name;
            if ( propertyName.EndsWith( "Attribute", StringComparison.Ordinal ) )
                propertyName = propertyName.Substring( 0, propertyName.Length - 9 );
            this.properties.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// Gets the property dictionary for this attribute
        /// </summary>
        public IPropertyBag Properties => properties;

        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding properties to it.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public virtual void ApplyToTest(Test test)
        {
            foreach (string key in Properties.Keys)
                foreach(object value in Properties[key])
                    test.Properties.Add(key, value);
        }

        #endregion
    }
}
