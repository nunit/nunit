// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Legacy;

namespace NUnit.Framework.Tests.Syntax
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
                type
                    .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .All(member => member is ConstructorInfo)); // Since they are inheritable, the exception is an instance constructor.

        [TestCaseSource(nameof(InheritableClassesWithNoInstanceMembers))]
        public static void InheritableClassWithOnlyStaticMembersShouldBeAbstract(Type type)
        {
            Assert.That(type.IsAbstract);
        }

        private static bool IsInheritable(Type type)
        {
            if (type.IsSealed)
                return false;

            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any()
                || type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Any(IsAccessibleFromExternalSubclass);
        }

        private static bool IsAccessibleFromExternalSubclass(MethodBase method)
        {
            return (method.Attributes & MethodAttributes.MemberAccessMask) switch
            {
                MethodAttributes.Public => true,
                MethodAttributes.Family => true,
                MethodAttributes.FamORAssem => true,
                _ => false
            };
        }
    }
}
