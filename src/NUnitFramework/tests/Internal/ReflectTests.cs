// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    public static class ReflectTests
    {
        [Test]
        public static void TypeAndBaseTypesReturnsEmptyForNull()
        {
            Assert.That((default(Type)).TypeAndBaseTypes(), Is.Empty);
        }

        [Test]
        public static void TypeAndBaseTypesReturnsSingleTypeForSystemObject()
        {
            Assert.That(typeof(object).TypeAndBaseTypes(), Is.EqualTo(new[] { typeof(object) }));
        }

        [Test]
        public static void TypeAndBaseTypesReturnsSingleTypeForInterface()
        {
            Assert.That(typeof(IDisposable).TypeAndBaseTypes(), Is.EqualTo(new[] { typeof(IDisposable) }));
        }

        [Test]
        public static void TypeAndBaseTypesStartsWithSpecifiedClassAndTraversesAllBaseClasses()
        {
            Assert.That(typeof(InheritsFromExternalClass).TypeAndBaseTypes(), Is.EqualTo(new[]
            {
                typeof(InheritsFromExternalClass),
                typeof(List<InheritsFromExternalClass>),
                typeof(object)
            }));
        }

        private class InheritsFromExternalClass : List<InheritsFromExternalClass>
        {
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodSearchesBaseClasses()
        {
            Assert.That(
                typeof(B).GetNonGenericPublicInstanceMethod(nameof(A.InstanceMethod), Type.EmptyTypes),
                Is.EqualTo(typeof(A).GetMethod(nameof(A.InstanceMethod))));
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodIgnoresGenericOverloads()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.HasGenericOverloads), Type.EmptyTypes),
                Is.EqualTo(typeof(A).GetMethods().Single(m =>
                    m.Name == nameof(A.HasGenericOverloads)
                    && !m.IsGenericMethod)));
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodIgnoresGenericMethods()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.GenericMethod), Type.EmptyTypes),
                Is.Null);
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodIgnoresStaticMethod()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.StaticMethod), Type.EmptyTypes),
                Is.Null);
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodIgnoresInternalMethod()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.InternalMethod), Type.EmptyTypes),
                Is.Null);
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodChoosesCorrectOverload()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.OverloadedMethod), new[] { typeof(object) }),
                Is.EqualTo(typeof(A).GetMethod(nameof(A.OverloadedMethod), new[] { typeof(object) })));
        }

        [Test]
        public static void GetNonGenericPublicInstanceMethodRequiresExactParameterTypes()
        {
            Assert.That(
                typeof(A).GetNonGenericPublicInstanceMethod(nameof(A.HasOptionalParameter), new[] { typeof(int) }),
                Is.Null);
        }

        [Test]
        public static void GetPublicInstancePropertySearchesBaseClasses()
        {
            Assert.That(
                typeof(B).GetPublicInstanceProperty(nameof(A.InstanceProperty), Type.EmptyTypes),
                Is.EqualTo(typeof(A).GetProperty(nameof(A.InstanceProperty))));
        }

        [Test]
        public static void GetPublicInstancePropertyIgnoresStaticProperty()
        {
            Assert.That(
                typeof(A).GetPublicInstanceProperty(nameof(A.StaticProperty), Type.EmptyTypes),
                Is.Null);
        }

        [Test]
        public static void GetPublicInstancePropertyIgnoresInternalProperty()
        {
            Assert.That(
                typeof(A).GetPublicInstanceProperty(nameof(A.InternalProperty), Type.EmptyTypes),
                Is.Null);
        }

        [Test]
        public static void GetPublicInstancePropertyChoosesCorrectOverload()
        {
            Assert.That(
                typeof(A).GetPublicInstanceProperty("Item", new[] { typeof(object) }),
                Is.EqualTo(typeof(A).GetProperty("Item", typeof(int), new[] { typeof(object) })));
        }

        [Test]
        public static void GetPublicInstancePropertyRequiresExactParameterTypes()
        {
            Assert.That(
                typeof(A).GetPublicInstanceProperty("Item", new[] { typeof(byte) }),
                Is.Null);
        }

        private class A
        {
            public void InstanceMethod() { }

            public void HasGenericOverloads() { }
            public void HasGenericOverloads<T>() { }
            public void HasGenericOverloads<T1, T2>() { }

            public void GenericMethod<T>() { }

            public static void StaticMethod() { }

            internal void InternalMethod() { }

            public void OverloadedMethod(object arg) { }

            public void OverloadedMethod(int arg) { }

            public void HasOptionalParameter(int a, int b = 42) { }

            public int InstanceProperty { get; set; }

            public static int StaticProperty { get; set; }

            internal int InternalProperty { get; set; }

            public int this[object arg] { get => 0;
                set { } }
            public int this[int arg] { get => 0;
                set { } }
            public int this[byte a, byte b = 42] { get => 0;
                set { } }
        }

        private class B : A
        {
        }

        [Test]
        public static void InvokeWithTransparentExceptionsReturnsCorrectValue()
        {
            Assert.That(GetPrivateMethod(nameof(MethodReturning42))
                    .InvokeWithTransparentExceptions(instance: null),
                Is.EqualTo(42));
        }

        private static MethodInfo GetPrivateMethod(string methodName)
        {
            MethodInfo? methodInfo = typeof(ReflectTests).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            return methodInfo ?? throw new ArgumentException($"Method ReflectTests.{methodName} not found");
        }

        [Test]
        public static void DynamicInvokeWithTransparentExceptionsReturnsCorrectValue()
        {
            Assert.That(
                new Func<int>(MethodReturning42).DynamicInvokeWithTransparentExceptions(),
                Is.EqualTo(42));
        }

        [Test]
        public static void InvokeWithTransparentExceptionsDoesNotWrap()
        {
            Assert.That(() => GetPrivateMethod(nameof(MethodThrowingException))
                    .InvokeWithTransparentExceptions(instance: null),
                Throws.TypeOf<Exception>());
        }

        [Test]
        public static void DynamicInvokeWithTransparentExceptionsDoesNotWrap()
        {
            Assert.That(
                () => new Func<int>(MethodThrowingException).DynamicInvokeWithTransparentExceptions(),
                Throws.TypeOf<Exception>());
        }

        [Test]
        public static void InvokeWithTransparentExceptionsDoesNotUnwrap()
        {
            Assert.That(() => GetPrivateMethod(nameof(MethodThrowingTargetInvocationException))
                    .InvokeWithTransparentExceptions(instance: null),
                Throws.TypeOf<TargetInvocationException>());
        }

        [Test]
        public static void DynamicInvokeWithTransparentExceptionsDoesNotUnwrap()
        {
            Assert.That(
                () => new Func<int>(MethodThrowingTargetInvocationException).DynamicInvokeWithTransparentExceptions(),
                Throws.TypeOf<TargetInvocationException>());
        }

        [Test]
        public static void InvokeWithTransparentExceptionsPreservesStackTrace()
        {
            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.IgnoreOnAffectedPlatform(() =>
            {
                Assert.That(() => GetPrivateMethod(nameof(MethodThrowingTargetInvocationException))
                        .InvokeWithTransparentExceptions(instance: null),
                    Throws.Exception
                        .With.Property(nameof(Exception.StackTrace))
                            .Contains(nameof(MethodThrowingTargetInvocationException)));
            });
        }

        [Test]
        public static void DynamicInvokeWithTransparentExceptionsPreservesStackTrace()
        {
            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.IgnoreOnAffectedPlatform(() =>
            {
                Assert.That(
                    () => new Func<int>(MethodThrowingTargetInvocationException).DynamicInvokeWithTransparentExceptions(),
                    Throws.Exception
                        .With.Property(nameof(Exception.StackTrace))
                            .Contains(nameof(MethodThrowingTargetInvocationException)));
            });
        }

        private static int MethodReturning42() => 42;

        private static int MethodThrowingException()
        {
            throw new Exception();
        }

        private static int MethodThrowingTargetInvocationException()
        {
            throw new TargetInvocationException(new Exception());
        }

        private const string ValueInBase = "Base";
        private const string ValueInDerived = "Derived";

        [TestCase(typeof(DerivedWithoutMember), ValueInBase)]
        [TestCase(typeof(DerivedWithMember), ValueInDerived)]
        public static void FindMember(Type type, string expected)
        {
            MemberInfo[] members = type.GetMemberIncludingFromBase("Data",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            Assert.That(members, Has.Length.EqualTo(1), "Expected one result");
            string? actual = null;
            if (members[0] is FieldInfo field)
            {
                actual = (string?)field.GetValue(null);
            }
            else if (members[0] is PropertyInfo property)
            {
                actual = (string?)property.GetValue(null, null);
            }

            Assert.That(actual, Is.EqualTo(expected), "Value");
        }

        private abstract class BaseWithMember
        {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
            private static readonly string Data = ValueInBase;
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        }

        private sealed class DerivedWithMember : BaseWithMember
        {
            public static string Data => ValueInDerived;
        }

        private sealed class DerivedWithoutMember : BaseWithMember
        {
        }
    }
}
