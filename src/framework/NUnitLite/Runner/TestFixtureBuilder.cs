using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace NUnitLite.Runner
{
    /// <summary>
    /// Static class used to create test fixtures from Types
    /// </summary>
    public class TestFixtureBuilder
    {
        /// <summary>
        /// Determines whether this instance can build a fixture from the specified type.
        /// </summary>
        /// <param name="type">The type to use as a fixture</param>
        /// <returns>
        /// 	<c>true</c> if this instance can build from the specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanBuildFrom(Type type)
        {
            if (Reflect.HasAttribute(type, typeof(TestFixtureAttribute)))
                return true;

            if (!type.IsPublic && !type.IsNestedPublic)
                return false;

            if (type.IsAbstract && !type.IsSealed)
                return false;

            foreach (MethodInfo method in type.GetMethods())
            {
                if (Reflect.HasAttribute(method, typeof(TestAttribute)) ||
                    Reflect.HasAttribute(method, typeof(TestCaseAttribute)))
                        return true;
            }

            return false;
        }

        /// <summary>
        /// Builds a fixture from the specified Type.
        /// </summary>
        /// <param name="type">The type to use as a fixture.</param>
        /// <returns></returns>
        public static TestSuite BuildFrom(Type type)
        {
            TestSuite suite = new TestSuite(type);

            object[] attrs = type.GetCustomAttributes( typeof(PropertyAttribute), true);
            foreach (PropertyAttribute attr in attrs)
                foreach( DictionaryEntry entry in attr.Properties )
                    suite.Properties[entry.Key] = entry.Value;

            IgnoreAttribute ignore = (IgnoreAttribute)Reflect.GetAttribute(type, typeof(IgnoreAttribute));
            if (ignore != null)
            {
                suite.RunState = RunState.Ignored;
                suite.IgnoreReason = ignore.Reason;
            }

            if (!Reflect.HasConstructor(type))
            {
                suite.RunState = RunState.NotRunnable;
                suite.IgnoreReason = string.Format("Class {0} has no default constructor", type.Name);
                return suite;
            }

            foreach (MethodInfo method in type.GetMethods())
            {
                if (TestCaseBuilder.IsTestMethod(method))
                    suite.AddTest(TestCaseBuilder.BuildFrom(method));
            }

            return suite;
        }
    }
}
