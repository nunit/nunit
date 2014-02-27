// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

#if !NUNITLITE
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Built-in SuiteBuilder for LegacySuite
    /// </summary>
    public class LegacySuiteBuilder : ISuiteBuilder
    {
        /// <summary>
        /// Default builder used by legacy builder to create
        /// the suites included in the legacy suite.
        /// </summary>
        ISuiteBuilder _defaultSuiteBuilder = new DefaultSuiteBuilder();

        #region ISuiteBuilderMembers
        /// <summary>
        /// Checks to see if the fixture type has the a property
        /// with the SuiteAttribute.
        /// </summary>
        /// <param name="type">The fixture type to check</param>
        /// <returns>True if the fixture can be built, false if not</returns>
        public bool CanBuildFrom(Type type)
        {
            return GetSuiteProperty( type ) != null;
        }

        /// <summary>
        /// Build a TestSuite from type provided.
        /// </summary>
        /// <param name="type">The type of the fixture to be used</param>
        /// <returns>A TestSuite</returns>
        public Test BuildFrom( Type type )
        {
            TestSuite suite = new LegacySuite( type );

            if (suite.RunState == RunState.NotRunnable)
            {
                string reason = null;
                if (!IsValidFixtureType(type, ref reason))
                {
                    suite.RunState = RunState.NotRunnable;
                    suite.Properties.Set(PropertyNames.SkipReason, reason);
                }
            }

            PropertyInfo suiteProperty = GetSuiteProperty(type);
            MethodInfo method = suiteProperty.GetGetMethod(true);

            if (method.GetParameters().Length > 0)
            {
                suite.RunState = RunState.NotRunnable;
                suite.Properties.Set(PropertyNames.SkipReason, "Suite property may not be indexed");
            }
            // TODO: Stop checking for name
            else if (method.ReturnType.FullName == "NUnit.Framework.Internal.TestSuite")
            {
                TestSuite s = (TestSuite)suiteProperty.GetValue(null, new Object[0]);
                foreach (Test test in s.Tests)
                    suite.Add(test);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(method.ReturnType))
            {
                foreach (object obj in (IEnumerable)suiteProperty.GetValue(null, new object[0]))
                {
                    Type objType = obj as Type;
                    if (objType != null && _defaultSuiteBuilder.CanBuildFrom(objType))
                        suite.Add(_defaultSuiteBuilder.BuildFrom(objType));
                    else
                    {
                        var fixture = _defaultSuiteBuilder.BuildFrom(obj.GetType());
                        if (fixture != null)
                            fixture.Fixture = obj;
                        suite.Add(fixture);
                    }
                }
            }
            else
            {
                suite.RunState = RunState.NotRunnable;
                suite.Properties.Set(PropertyNames.SkipReason, "Suite property must return either TestSuite or IEnumerable");
            }

            return suite;
        }
        #endregion

        #region Helper Methods
        private bool IsValidFixtureType(Type type, ref string reason)
        {
            if (type.IsAbstract)
            {
                reason = string.Format("{0} is an abstract class", type.FullName);
                return false;
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                reason = string.Format("{0} does not have a valid constructor", type.FullName);
                return false;
            }

            if (Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.SetUpAttribute)))
            {
                reason = "SetUp method not allowed on a legacy suite";
                return false;
            }

            if (Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TearDownAttribute)))
            {
                reason = "TearDown method not allowed on a legacy suite";
                return false;
            }

            return true;
        }

        private static PropertyInfo GetSuiteProperty(Type testClass)
        {
            foreach (PropertyInfo property in testClass.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.IsDefined(typeof(SuiteAttribute), true))
                    return property;
            }

            return null;
        }
        #endregion
    }
}
#endif
