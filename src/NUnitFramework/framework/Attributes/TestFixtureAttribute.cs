// ***********************************************************************
// Copyright (c) 2009-2018 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// TestFixtureAttribute is used to mark a class that represents a TestFixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class TestFixtureAttribute : NUnitAttribute, IFixtureBuilder2, ITestFixtureData
    {
        private readonly NUnitTestFixtureBuilder _builder = new NUnitTestFixtureBuilder();

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
            RunState = RunState.Runnable;
            Arguments = arguments ?? new object[] { null };
            TypeArgs = new Type[0];
            Properties = new PropertyBag();
        }

        #endregion

        #region ITestData Members

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>The name of the test.</value>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the RunState of this test fixture.
        /// </summary>
        public RunState RunState { get; private set; }

        /// <summary>
        /// The arguments originally provided to the attribute
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        /// Properties pertaining to this fixture
        /// </summary>
        public IPropertyBag Properties { get; }

        #endregion

        #region ITestFixtureData Members

        /// <summary>
        /// Get or set the type arguments. If not set
        /// explicitly, any leading arguments that are
        /// Types are taken as type arguments.
        /// </summary>
        public Type[] TypeArgs { get; set; }

        #endregion

        #region Other Properties

        /// <summary>
        /// Descriptive text for this fixture
        /// </summary>
        public string Description
        {
            get { return Properties.Get(PropertyNames.Description) as string; }
            set { Properties.Set(PropertyNames.Description, value); }
        }

        /// <summary>
        /// The author of this fixture
        /// </summary>
        public string Author
        {
            get { return Properties.Get(PropertyNames.Author) as string; }
            set { Properties.Set(PropertyNames.Author, value); }
        }

        /// <summary>
        /// The type that this fixture is testing
        /// </summary>
        public Type TestOf 
        {
            get { return _testOf;  }
            set
            {
                _testOf = value;
                Properties.Set(PropertyNames.TestOf, value.FullName);
            }
        }
        private Type _testOf;

        /// <summary>
        /// Gets or sets the ignore reason. May set RunState as a side effect.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string Ignore
        {
            get { return IgnoreReason;  }
            set { IgnoreReason = value; }
        }

        /// <summary>
        /// Gets or sets the reason for not running the fixture.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason
        {
            get { return this.Properties.Get(PropertyNames.SkipReason) as string; }
            set { this.Properties.Set(PropertyNames.SkipReason, value); }
        }

        /// <summary>
        /// Gets or sets the ignore reason. When set to a non-null
        /// non-empty value, the test is marked as ignored.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return Reason; }
            set
            {
                RunState = RunState.Ignored;
                Reason = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NUnit.Framework.TestFixtureAttribute"/> is explicit.
        /// </summary>
        /// <value>
        /// <c>true</c> if explicit; otherwise, <c>false</c>.
        /// </value>
        public bool Explicit
        {
            get { return RunState == RunState.Explicit; }
            set { RunState = value ? RunState.Explicit : RunState.Runnable; }
        }

        /// <summary>
        /// Gets and sets the category for this fixture.
        /// May be a comma-separated list of categories.
        /// </summary>
        public string Category
        {
            get 
            { 
                //return Properties.Get(PropertyNames.Category) as string;
                var catList = Properties[PropertyNames.Category];
                if (catList == null)
                    return null;

                switch (catList.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return catList[0] as string;
                    default:
                        var cats = new string[catList.Count];
                        int index = 0;
                        foreach (string cat in catList)
                            cats[index++] = cat;

                        return string.Join(",", cats);
                }
            }
            set
            {
                foreach (string cat in value.Split(new char[] { ',' }))
                    Properties.Add(PropertyNames.Category, cat);
            }
        }

        #endregion

        #region IFixtureBuilder Members

        /// <summary>
        /// Builds a single test fixture from the specified type.
        /// </summary>
        /// <param name="type">The type to be used as a fixture.</param>
        public IEnumerable<TestSuite> BuildFrom(Type type)
        {
            yield return _builder.BuildFrom(type, PreFilter.Empty, this);
        }

        #endregion

        #region IFixtureBuilder2 Members

        /// <summary>
        /// Builds a single test fixture from the specified type.
        /// </summary>
        /// <param name="type">The type to be used as a fixture.</param>
        /// <param name="filter">Filter used to select methods as tests.</param>
        public IEnumerable<TestSuite> BuildFrom(Type type, IPreFilter filter)
        {
            yield return _builder.BuildFrom(type, filter, this);
        }

        #endregion
    }
}
