// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <example>
    /// [TestFixture]
    /// public class ExampleClass 
    /// {}
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class TestFixtureAttribute : FixtureBuilderAttribute, IFixtureBuilder, IApplyToTest
    {
        private readonly NUnitTestFixtureBuilder builder = new NUnitTestFixtureBuilder();

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestFixtureAttribute() : this( new object[0] ) { }
        
        /// <summary>
        /// Construct with a object[] representing a set of arguments. 
        /// In .NET 2.0, the arguments may later be separated into
        /// type arguments and constructor arguments.
        /// </summary>
        /// <param name="arguments"></param>
        public TestFixtureAttribute(params object[] arguments)
        {
            Arguments = arguments;
            TypeArgs = new Type[0];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Descriptive text for this fixture
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The author of this fixture
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The type that this fixture is testing
        /// </summary>
        public Type TestOf { get; set; }

        /// <summary>
        /// The arguments originally provided to the attribute
        /// </summary>
        public object[] Arguments { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TestFixtureAttribute"/> should be ignored.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        public bool Ignore { get; set; }

        /// <summary>
        /// Gets or sets the ignore reason. May set Ignored as a side effect.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return _ignoreReason; }
            set
            {
                _ignoreReason = value;
                Ignore = !string.IsNullOrEmpty(_ignoreReason);
            }
        }
        private string _ignoreReason;

        /// <summary>
        /// Get or set the type arguments. If not set
        /// explicitly, any leading arguments that are
        /// Types are taken as type arguments.
        /// </summary>
        public Type[] TypeArgs { get; set; }

        /// <summary>
        /// Gets and sets the category for this fixture.
        /// May be a comma-separated list of categories.
        /// </summary>
        public string Category { get; set; }
 
        /// <summary>
        /// Gets a list of categories for this fixture
        /// </summary>
        public IList Categories
        {
            get { return Category == null ? null : Category.Split(','); }
        }

        #endregion

        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding a description, if not already set.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (!test.Properties.ContainsKey(PropertyNames.Description) && Description != null)
                test.Properties.Set(PropertyNames.Description, Description);

            if (!test.Properties.ContainsKey(PropertyNames.Author) && Author != null)
                test.Properties.Set(PropertyNames.Author, Author);

            if (!test.Properties.ContainsKey(PropertyNames.TestOf) && TestOf != null)
                test.Properties.Set(PropertyNames.TestOf, TestOf.FullName);
            
            if (Category != null)
                foreach (string cat in Category.Split(new[] { ',' }) )
                    test.Properties.Add(PropertyNames.Category, cat);
        }

        #endregion

        #region IFixtureBuilder Members

        /// <summary>
        /// Build a SetUpFixture from type provided. Normally called for a Type
        /// on which the attribute has been placed.
        /// </summary>
        /// <param name="type">The type of the fixture to be used.</param>
        /// <returns>A SetUpFixture object as a TestSuite.</returns>
        public TestSuite BuildFrom(Type type)
        {
            return builder.BuildFrom(type, this);
        }

        #endregion

        #region Helper Methods

//        /// <summary>
//        /// Adjust the originally provided arguments, segregating them
//        /// into Type arguments and actual arguments.
//        /// </summary>
//        private void AdjustArguments(Type type)
//        {
//#if !NETCF
//            if (type.ContainsGenericParameters)
//            {
//                Type[] typeArgs = this.TypeArgs;
//                if (typeArgs.Length == 0)
//                {
//                    int cnt = 0;
//                    foreach (object o in Arguments)
//                        if (o is Type) cnt++;
//                        else break;

//                    typeArgs = new Type[cnt];
//                    for (int i = 0; i < cnt; i++)
//                        typeArgs[i] = (Type)Arguments[i];

//                    if (cnt > 0)
//                    {
//                        object[] args = new object[Arguments.Length - cnt];
//                        for (int i = 0; i < args.Length; i++)
//                            args[i] = Arguments[cnt + i];

//                        Arguments = args;
//                    }
//                }

//                if (typeArgs.Length > 0 ||
//                    TypeHelper.CanDeduceTypeArgsFromArgs(type, Arguments, ref TypeArgs))
//                {
//                    type = TypeHelper.MakeGenericType(type, typeArgs);
//                }
//            }
//#endif
        //        }

        #endregion
    }
}
