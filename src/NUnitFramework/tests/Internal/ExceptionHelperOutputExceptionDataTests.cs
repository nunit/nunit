// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public static class ExceptionHelperOutputExceptionDataTests
    {
        [Test]
        public static void AppendsDataItemsToExceptionMessage()
        {
            var exception = new Exception("blah");
            exception.Data["data-prop"] = "data-value";

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("data-prop"));
            Assert.That(message, Contains.Substring("data-value"));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessage()
        {
            var exception = new ExceptionHelperException("blah")
            {
                CustomProperty = "custom-value",
                AuxiliaryValues = new Dictionary<string, string>
                {
                    ["aux-key1"] = "aux-key1-value",
                    ["aux-key2"] = "aux-key2-value",
                }
            };

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("CustomProperty: custom-value"));
            Assert.That(message, Contains.Substring("AuxiliaryValues: ["));
            Assert.That(message, Contains.Substring("[aux-key1] = aux-key1-value"));
            Assert.That(message, Contains.Substring("[aux-key2] = aux-key2-value"));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessageEmptyDictionary()
        {
            var exception = new ExceptionHelperException("blah")
            {
                CustomProperty = "custom-value",
                AuxiliaryValues = [],
            };

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("CustomProperty: custom-value"));
            Assert.That(message, Contains.Substring("AuxiliaryValues: []"));
            Assert.That(message, Does.Not.Contain("Message: Override"));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessageCanDealWithExceptions()
        {
            var exception = new ExceptionHelperException("blah");

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("CustomProperty"));
            Assert.That(message, Contains.Substring("NullReferenceException"));
            Assert.That(message, Contains.Substring("AuxiliaryValues: <null>"));
        }

        private sealed class ExceptionHelperException : Exception
        {
            private readonly string? _customProperty;

            public ExceptionHelperException(string message) : base(message)
            {
            }

            public override string Message => $"Override({base.Message})";

            public string? CustomProperty
            {
                get => _customProperty ?? throw new NullReferenceException();
                init => _customProperty = value;
            }

            public Dictionary<string, string>? AuxiliaryValues { get; init; }
        }
        [Test]
        public static void IncludesNullProperties()
        {
            var exception = new Exception("blah");
            exception.Data["data-prop"] = null;

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("data-prop"));
            Assert.That(message, Contains.Substring("<null>"));
        }

        [Test]
        public static void SkipsDataSectionOnEmptyData()
        {
            var exception = new Exception("blah");

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, !Contains.Substring("Data"));
        }

        [Test]
        public static void NoTrailingNewline()
        {
            var exception = new Exception("blah") { Data = { ["Foo"] = "Bar" } };

            var message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Does.Not.EndWith("\n"));
        }
    }
}
