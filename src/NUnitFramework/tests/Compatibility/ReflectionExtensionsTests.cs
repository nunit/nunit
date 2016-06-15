// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Reflection;
using System.Text;
using NUnit.Compatibility;

namespace NUnit.Framework.Tests.Compatibility
{
    /// <summary>
    /// A series of unit tests to ensure that the type extensions in the portable
    /// framework behave the same as their counterparts in the full framework
    /// </summary>
    [TestFixture]
    public class ReflectionExtensionsTests
    {
        private static bool REALLY_RUNNING_ON_CF = false;

#if NETCF
        static ReflectionExtensionsTests()
        {
            // We may be running on the desktop using assembly unification
            REALLY_RUNNING_ON_CF = Type.GetType("System.ConsoleColor") == null;
        }
#endif

        [Test]
        public void CanCallTypeInfoOnAllPlatforms()
        {
            var result = typeof(Object).GetTypeInfo();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CanGetGenericArguments()
        {
            var result = typeof(GenericTestClass<string, DateTime>).GetGenericArguments();
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(typeof(string)));
            Assert.That(result[1], Is.EqualTo(typeof(DateTime)));
        }

        [Test]
        public void CanGetConstructors()
        {
            var result = typeof(DerivedTestClass).GetConstructors();
            Assert.That(result.Length, Is.EqualTo(3));
            foreach (var ctor in result)
            {
                Assert.That(ctor.IsStatic, Is.False);
                Assert.That(ctor.IsPublic, Is.True);
            }
        }

        [TestCase(typeof(string))]
        [TestCase(typeof(double))]
        [TestCase(typeof(int))]
        public void CanGetContructorWithParams(Type paramType)
        {
            var result = typeof(DerivedTestClass).GetConstructor(new[] { paramType });
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CanGetGenericConstructor()
        {
            var result = typeof(GenericTestClass<double, double>)
                .GetConstructor(new[] { typeof(double), typeof(int) });
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void IsAssignableFromTest()
        {
            bool pass = typeof(BaseTestClass).IsAssignableFrom(typeof(DerivedTestClass));
            bool fail = typeof(DerivedTestClass).IsAssignableFrom(typeof(BaseTestClass));
            Assert.That(pass, Is.True);
            Assert.That(fail, Is.False);
        }

        [Test]
        public void IsInstanceOfTypeTest()
        {
            var baseClass = new BaseTestClass();
            var derivedClass = new DerivedTestClass();

            Assert.That(typeof(BaseTestClass).IsInstanceOfType(baseClass), Is.True);
            Assert.That(typeof(BaseTestClass).IsInstanceOfType(derivedClass), Is.True);
            Assert.That(typeof(DerivedTestClass).IsInstanceOfType(derivedClass), Is.True);
            Assert.That(typeof(DerivedTestClass).IsInstanceOfType(baseClass), Is.False);
            Assert.That(typeof(DerivedTestClass).IsInstanceOfType(null), Is.False);
        }

        [TestCase(typeof(BaseTestClass), typeof(IDisposable))]
        [TestCase(typeof(DerivedTestClass), typeof(IDisposable))]
        [TestCase(typeof(List<int>), typeof(IList<int>))]
        [TestCase(typeof(List<int>), typeof(IEnumerable<int>))]
        public void CanGetInterfaces(Type type, Type @interface)
        {
            var result = type.GetInterfaces();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain(@interface));
        }

        [TestCase(typeof(BaseTestClass), "Name")]
        [TestCase(typeof(BaseTestClass), "StaticString")]
        [TestCase(typeof(BaseTestClass), "Protected")]
        [TestCase(typeof(BaseTestClass), "Dispose")]
        [TestCase(typeof(BaseTestClass), "Private")]
        [TestCase(typeof(DerivedTestClass), "Name")]
        [TestCase(typeof(DerivedTestClass), "Protected")]
        [TestCase(typeof(DerivedTestClass), "Dispose")]
        public void CanGetMember(Type type, string name)
        {
            var result = type.GetMember(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(1));
        }

        [Test]
        public void GetStaticMemberOnBaseClass()
        {
            var result = typeof(DerivedTestClass).GetMember("StaticString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public void CanGetPrivatePropertiesOnBaseClassOnlyOnCF()
        {
            var result = typeof(DerivedTestClass).GetMember("Private", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(REALLY_RUNNING_ON_CF ? 1 : 0));
        }

        [Test]
        public void CanGetPrivateMethodsOnBaseClassOnlyOnCF()
        {
            var result = typeof(DerivedTestClass).GetMember("Goodbye", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(REALLY_RUNNING_ON_CF ? 1 : 0));
        }

        [Test]
        public void CanGetPublicField()
        {
            var result = typeof(DerivedTestClass).GetField("_public");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void DoesNotGetPrivateField()
        {
            var result = typeof(DerivedTestClass).GetField("_private");
            Assert.That(result, Is.Null);
        }

        [TestCase(typeof(BaseTestClass), "Name")]
        [TestCase(typeof(DerivedTestClass), "Name")]
        [TestCase(typeof(BaseTestClass), "StaticString")]
        public void CanGetProperty(Type type, string name)
        {
            var result = type.GetProperty(name);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void DoesNotGetStaticPropertyOnBase()
        {
            var result = typeof(DerivedTestClass).GetProperty("StaticString");
            Assert.That(result, Is.Null);
        }

        [TestCase(typeof(BaseTestClass), "Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "Private", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "StaticString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, false)]
        [TestCase(typeof(DerivedTestClass), "StaticString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, false)]
        [TestCase(typeof(BaseTestClass), "Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, false)]
        [TestCase(typeof(DerivedTestClass), "Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, false)]
        [TestCase(typeof(BaseTestClass), "Private", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, false)]
        [TestCase(typeof(DerivedTestClass), "Private", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, false)]
        [TestCase(typeof(BaseTestClass), "StaticString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, true)]
        [TestCase(typeof(DerivedTestClass), "StaticString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, false)]
        [TestCase(typeof(BaseTestClass), "Name", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "Name", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "Private", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(DerivedTestClass), "Private", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(BaseTestClass), "StaticString", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "StaticString", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(BaseTestClass), "Name", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(DerivedTestClass), "Name", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(BaseTestClass), "Private", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "StaticString", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(DerivedTestClass), "StaticString", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(BaseTestClass), "PubPriv", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "PubPriv", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "PubPriv", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        [TestCase(typeof(DerivedTestClass), "PubPriv", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, false)]
        public void CanGetPropertyWithBindingFlags(Type type, string name, BindingFlags flags, bool shouldFind)
        {
            var result = type.GetProperty(name, flags);
            Assert.AreEqual(shouldFind, result != null);
        }

        [TestCase(typeof(BaseTestClass), "Dispose", true)]
        [TestCase(typeof(DerivedTestClass), "Dispose", true)]
        [TestCase(typeof(BaseTestClass), "Goodbye", false)]
        [TestCase(typeof(DerivedTestClass), "Goodbye", false)]
        public void CanGetMethodByName(Type type, string name, bool shouldFind)
        {
            var result = type.GetMethod(name);
            Assert.AreEqual(shouldFind, result != null);
        }

        [TestCase(typeof(BaseTestClass), "Hello", new Type[] { }, true)]
        [TestCase(typeof(DerivedTestClass), "Hello", new Type[] { }, true)]
        [TestCase(typeof(BaseTestClass), "Hello", new Type[] { typeof(string) }, true)]
        [TestCase(typeof(DerivedTestClass), "Hello", new Type[] { typeof(string) }, true)]
        [TestCase(typeof(BaseTestClass), "Goodbye", new Type[] { typeof(double) }, false)]
        [TestCase(typeof(DerivedTestClass), "Goodbye", new Type[] { typeof(int) }, false)]
        public void CanGetMethodByNameAndArgs(Type type, string name, Type[] args, bool shouldFind)
        {
            var result = type.GetMethod(name, args);
            Assert.AreEqual(shouldFind, result != null);
        }

        [TestCase(typeof(BaseTestClass), "Dispose", BindingFlags.Public | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "Dispose", BindingFlags.Public | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "Dispose", BindingFlags.Public | BindingFlags.Instance, true)]
        [TestCase(typeof(DerivedTestClass), "Dispose", BindingFlags.Public | BindingFlags.Instance, true)]
        [TestCase(typeof(BaseTestClass), "StaticMethod", BindingFlags.Public | BindingFlags.Static, true)]
        [TestCase(typeof(DerivedTestClass), "StaticMethod", BindingFlags.Public | BindingFlags.Static, false)]
        [TestCase(typeof(BaseTestClass), "Goodbye", BindingFlags.NonPublic | BindingFlags.Instance, true)]
        public void CanGetMethodByNameAndBindingFlags(Type type, string name, BindingFlags flags, bool shouldFind)
        {
            var result = type.GetMethod(name, flags);
            Assert.AreEqual(shouldFind, result != null);
        }

        public void CanGetStaticMethodsOnBase(BindingFlags flags)
        {
            var result = typeof(DerivedTestClass).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach(var info in result)
            {
                if (info.Name == "StaticMethod")
                    Assert.Pass();
            }
            Assert.Fail("Did not find StaticMethod on the base class");
        }

        [TestCase(BindingFlags.Static)]
        [TestCase(BindingFlags.FlattenHierarchy)]
        [TestCase(BindingFlags.Default)]
        public void DoesNotFindStaticMethodsOnBase(BindingFlags flags)
        {
            var result = typeof(DerivedTestClass).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | flags);
            foreach (var info in result)
            {
                if (info.Name == "StaticMethod")
                    Assert.Fail("Should not have found StaticMethod on the base class");
            }
        }

        [TestCase("Name", false, true)]
        [TestCase("Name", true, true)]
        [TestCase("Private", false, false)]
        [TestCase("Private", true, true)]
        public void CanGetGetMethod(string name, bool nonPublic, bool shouldFind)
        {
            var pinfo = typeof(BaseTestClass).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(pinfo, Is.Not.Null);
            var minfo = pinfo.GetGetMethod(nonPublic);
            Assert.That(minfo != null, Is.EqualTo(shouldFind));
        }

#if PORTABLE
        [Test]
        public void CanGetAttributesUsingAnInterface()
        {
            var method = typeof(ReflectionExtensionsTests).GetMethod("CanGetAttributesUsingAnInterface");
            Assert.That(method, Is.Not.Null);
            var attr = method.GetAttributes<ITestAction>(false);
            Assert.That(attr, Is.Not.Null);
        }
#endif
    }

    public class BaseTestClass : IDisposable
    {
        public static string StaticString { get; set; }

        protected string Protected { get; set; }

        public string Name { get; set; }

        private string Private { get; set; }

        public string PubPriv { get; private set; }

        public BaseTestClass() : this("Joe") { }

        public BaseTestClass(string name) { Name = name; }

        public virtual void Hello() { }

        public virtual void Hello(string msg) { }

        public static void StaticMethod() { }

        private void Goodbye(double d) { }

        public void Dispose() { }
    }

    public class DerivedTestClass : BaseTestClass
    {
#pragma warning disable 0169
        private string _private;
#pragma warning restore
        public string _public;

        static DerivedTestClass() { StaticString = "static"; }

        public DerivedTestClass() : this("Rob") { }

        public DerivedTestClass(string name) : base(name) { }

        public DerivedTestClass(double d) : this(d.ToString()) { }

        private DerivedTestClass(StringBuilder name) : this(name.ToString()) { }

        public override void Hello() { }

        public override void Hello(string msg) { }
    }

    public class GenericTestClass<T1, T2>
    {
        public T1 PropertyOne { get; set; }
        public T2 PropertyTwo { get; set; }

        public GenericTestClass(T1 one, T2 two)
        {
            PropertyOne = one;
            PropertyTwo = two;
        }
    }
}
