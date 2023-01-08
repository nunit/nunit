// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Applies a category to a test
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public class CategoryAttribute : NUnitAttribute, IApplyToTest
    {
        /// <summary>
        /// The name of the category
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected string categoryName;
#pragma warning restore IDE1006

        /// <summary>
        /// Construct attribute for a given category based on
        /// a name. The name may not contain the characters ',',
        /// '+', '-' or '!'. However, this is not checked in the
        /// constructor since it would cause an error to arise at
        /// as the test was loaded without giving a clear indication
        /// of where the problem is located. The error is handled
        /// in NUnitFramework.cs by marking the test as not
        /// runnable.
        /// </summary>
        /// <param name="name">The name of the category</param>
        public CategoryAttribute(string name)
        {
            this.categoryName = name.Trim();
        }

        /// <summary>
        /// Protected constructor uses the Type name as the name
        /// of the category.
        /// </summary>
        protected CategoryAttribute()
        {
            this.categoryName = this.GetType().Name;
            if ( categoryName.EndsWith( "Attribute", StringComparison.Ordinal ) )
                categoryName = categoryName.Substring( 0, categoryName.Length - 9 );
        }

        /// <summary>
        /// The name of the category
        /// </summary>
        public string Name => categoryName;

        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding a category to it.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            test.Properties.Add(PropertyNames.Category, this.Name);
        }

        #endregion
    }
}
