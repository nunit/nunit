// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
