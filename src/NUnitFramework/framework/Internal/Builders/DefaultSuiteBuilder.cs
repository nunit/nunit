// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Built-in SuiteBuilder for all types of test classes.
    /// </summary>
    public class DefaultSuiteBuilder : ISuiteBuilder
    {
        private NUnitTestFixtureBuilder defaultBuilder = new NUnitTestFixtureBuilder();

        #region ISuiteBuilder Methods
        /// <summary>
        /// Checks to see if the provided Type is a fixture.
        /// To be considered a fixture, it must be a non-abstract
        /// class with one or more attributes implementing the
        /// IFixtureBuilder interface or one or more methods
        /// marked as tests.
        /// </summary>
        /// <param name="type">The fixture type to check</param>
        /// <returns>True if the fixture can be built, false if not</returns>
        public bool CanBuildFrom(Type type)
        {
            if (type.IsAbstract && !type.IsSealed)
                return false;
#if NETCF
            // No generic fixtures under CF
            if (type.IsGenericTypeDefinition)
                return false;
#endif

            if (type.IsDefined(typeof(IFixtureBuilder), true))
                return true;

#if !NETCF
            // Generics must have an attribute in order to provide
            // them with arguments to determine the specific type.
            // TODO: What about automatic fixtures? Should there
            // be some kind of error shown?
            if (type.IsGenericTypeDefinition)
                return false;
#endif

            return Reflect.HasMethodWithAttribute(type, typeof(IImplyFixture));
        }

        /// <summary>
        /// Build a TestSuite from type provided.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Test BuildFrom(Type type)
        {
            IFixtureBuilder[] attrs = GetFixtureBuilderAttributes(type);

            if (type.IsGenericType)
                return BuildMultipleFixtures(type, attrs);

            switch (attrs.Length)
            {
                case 0:
                    return defaultBuilder.BuildFrom(type);
                case 1:
                    return attrs[0].BuildFrom(type);
                    //object[] args = attrs[0].Arguments;
                    //return args == null || args.Length == 0
                    //    ? attrs[0].BuildFrom(type)
                    //    : BuildMultipleFixtures(type, attrs);
                default:
                    return BuildMultipleFixtures(type, attrs);
            }
        }
        #endregion

        #region Helper Methods

        private Test BuildMultipleFixtures(Type type, IFixtureBuilder[] attrs)
        {
            TestSuite suite = new ParameterizedFixtureSuite(type);

            foreach (IFixtureBuilder attr in attrs)
                suite.Add(attr.BuildFrom(type));

            return suite;
        }

        /// <summary>
        /// We look for attributes implementing IFixtureBuilder at one level 
        /// of inheritance at a time. Attributes on base classes are not used 
        /// unless there are no fixture builder attributes at all on the derived
        /// class. This is by design.
        /// </summary>
        /// <param name="type">The type being examined for attributes</param>
        /// <returns>A list of the attributes found.</returns>
        private IFixtureBuilder[] GetFixtureBuilderAttributes(Type type)
        {
            IFixtureBuilder[] attrs = new IFixtureBuilder[0];

            while (type != null)
            {
                attrs = (IFixtureBuilder[])type.GetCustomAttributes(typeof(IFixtureBuilder), false);

                if (attrs.Length > 0)
                    return attrs;

                type = type.BaseType;
            }

            return attrs;
        }

        #endregion
    }
}
