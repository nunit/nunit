using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace NUnitLite.Runner
{
    public class TestCaseBuilder
    {
        public static bool IsTestMethod(MethodInfo method)
        {
            return Reflect.HasAttribute(method, typeof(TestAttribute))
                || Reflect.HasAttribute(method, typeof(TestCaseAttribute))
                || Reflect.HasAttribute(method, typeof(TestCaseSourceAttribute));
        }

        public static ITest BuildFrom(MethodInfo method)
        {
            IList testdata = GetTestCaseData(method);
            
            if (testdata.Count == 0)
                return new TestCase(method);

            TestSuite testcases = new TestSuite(method.Name);

            foreach (object[] args in testdata)
                testcases.AddTest(new TestCase(method, args));
            return testcases;
        }

        private static IList GetTestCaseData(MethodInfo method)
        {
            ArrayList data = new ArrayList();

            object[] attrs = method.GetCustomAttributes(typeof(TestCaseAttribute), false);
            foreach (TestCaseAttribute attr in attrs)
                data.Add(attr.Arguments);

            attrs = method.GetCustomAttributes(typeof(TestCaseSourceAttribute), false);
            foreach (TestCaseSourceAttribute attr in attrs)
            {
                string sourceName = attr.SourceName;
                Type sourceType = attr.SourceType;
                if (sourceType == null)
                    sourceType = method.ReflectedType;

                IEnumerable source = null;
                MemberInfo[] members = sourceType.GetMember(sourceName, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (members.Length == 1)
                {
                    MemberInfo member = members[0];
                    object sourceobject = Reflect.Construct(sourceType);
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo field = member as FieldInfo;
                            source = (IEnumerable)field.GetValue(sourceobject);
                            break;
                        case MemberTypes.Property:
                            PropertyInfo property = member as PropertyInfo;
                            source = (IEnumerable)property.GetValue(sourceobject, null);
                            break;
                        case MemberTypes.Method:
                            MethodInfo m = member as MethodInfo;
                            source = (IEnumerable)m.Invoke(sourceobject, null);
                            break;
                    }

                    int nparms = method.GetParameters().Length;
                    
                    foreach (object obj in source)
                        if (obj is TestCaseData)
                            data.Add(((TestCaseData)obj).Arguments);
                        else
                        {
                            object[] array = obj as object[];
                            if (array != null && array.Length == nparms)
                                data.Add(obj);
                            else
                                data.Add(new object[] { obj });
                        }
                }
            }

            return data;
        }
    }
}
