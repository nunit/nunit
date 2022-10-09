// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Syntax
{
    public static class SyntaxHelperExtensionTests
    {
        public static IEnumerable<Type> ClassesExtensibleThroughInheritance => new[]
        {
            typeof(Is),
            typeof(Has),
            typeof(Does),
            typeof(Contains),
            typeof(Throws),
            typeof(Iz),
            typeof(Assert),
            typeof(StringAssert),
            typeof(CollectionAssert)
        };

        [TestCaseSource(nameof(ClassesExtensibleThroughInheritance))]
        public static void ClassShouldBeInheritable(Type type)
        {
            Assert.That(IsInheritable(type));
        }

        public static IEnumerable<Type> InheritableClassesWithNoInstanceMembers =>
            ClassesExtensibleThroughInheritance.Where(type =>
                type.GetTypeInfo()
                    .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .All(member => member is ConstructorInfo)); // Since they are inheritable, the exception is an instance constructor.

        [TestCaseSource(nameof(InheritableClassesWithNoInstanceMembers))]
        public static void InheritableClassWithOnlyStaticMembersShouldBeAbstract(Type type)
        {
            Assert.That(type.GetTypeInfo().IsAbstract);
        }

        private static bool IsInheritable(Type type)
        {
            if (type.GetTypeInfo().IsSealed) return false;

            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any()
                || type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Any(IsAccessibleFromExternalSubclass);
        }

        private static bool IsAccessibleFromExternalSubclass(MethodBase method)
        {
            switch (method.Attributes & MethodAttributes.MemberAccessMask)
            {
                case MethodAttributes.Public:
                case MethodAttributes.Family:
                case MethodAttributes.FamORAssem:
                    return true;
                default:
                    return false;
            }
        }
    }
}
