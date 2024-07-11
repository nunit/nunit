// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.AttributeThrowingExceptionFixture
{
    [TestFixture]
    public class AttributeOnTestMethodThrowingExceptionFixture
    {
        [Test]
        [ExceptionThrowing(nameof(NormalTest))]
        public void NormalTest()
        {
            Assert.Pass();
        }

        [Test]
        [ExceptionThrowing("")]
        public void TestWithFailingAttribute()
        {
            Assert.Pass();
        }

        [Test]
        [ExceptionThrowingApplyTo]
        public void SpecialTestHandling()
        {
        }
    }

    [TestFixture]
    public class AttributeOnOneTimeSetUpMethodsThrowingExceptionFixture : AttributeOnTestMethodThrowingExceptionFixture
    {
        [OneTimeSetUp]
        [ExceptionThrowing(null)]
        public void FixtureSetup()
        {
        }
    }

    [TestFixture]
    public class AttributeOnSetUpMethodsThrowingExceptionFixture : AttributeOnTestMethodThrowingExceptionFixture
    {
        [SetUp]
        [ExceptionThrowing(null)]
        public void FixtureSetup()
        {
        }
    }

    [TestFixture]
    [ExceptionThrowing(null)]
    public class AttributeOnFixtureThrowingExceptionFixture : AttributeOnTestMethodThrowingExceptionFixture
    {
    }

    public class ExceptionThrowingAttribute : Attribute
    {
        public ExceptionThrowingAttribute(string? message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
        }

        public string Message { get; }
    }

    public class ExceptionThrowingApplyToAttribute : Attribute, IApplyToTest
    {
        public void ApplyToTest(Test test)
        {
            throw new NotImplementedException();
        }
    }
}
