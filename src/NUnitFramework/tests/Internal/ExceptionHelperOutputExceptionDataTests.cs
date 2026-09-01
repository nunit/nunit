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
        public static void AppendsPropertiesToExceptionMessageSkipsByRefProperties_Issue5401()
        {
            // Regression: on .NET Framework PropertyInfo.GetValue throws NotSupportedException for a by-ref
            // returning property before invoking the getter, which escaped BuildMessage and killed the worker.
            var exception = new ByRefPropertyException("blah");

            string message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring("ByValue: 42"));
            Assert.That(message, Does.Not.Contain($"{nameof(ByRefPropertyException.ByRefValue)}:"));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessageSkipsIndexers()
        {
            // Same escape as issue #5401: GetValue(obj) on an indexer throws TargetParameterCountException.
            var exception = new IndexerPropertyException("blah");

            string message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Does.Not.Contain("Item:"));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessageCanDealWithByRefLikeProperties()
        {
            // On .NET the runtime refuses to invoke a getter returning a ref struct (NotSupportedException), so the
            // property is rendered as "<unreadable: ...>". The .NET Framework CLR does not know IsByRefLike, so
            // System.Memory's span is boxed there like any other struct; Mono may refuse like .NET does.
            // Only the TFM-independent parts of the output are asserted.
            var exception = new ByRefLikePropertyException("blah");

            string message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring("blah"));
            Assert.That(message, Contains.Substring($"{nameof(ByRefLikePropertyException.SpanValue)}: "));
        }

        [Test]
        public static void AppendsPropertiesToExceptionMessageClosesGetterExceptionBracket()
        {
            var exception = new ExceptionHelperException("blah");

            string message = ExceptionHelper.BuildMessage(exception);
            Assert.That(message, Contains.Substring($"{nameof(ExceptionHelperException.CustomProperty)}: <getter threw NullReferenceException("));
            Assert.That(message, Contains.Substring(")>"));
        }

        private sealed class ByRefPropertyException : Exception
        {
            private readonly int _byRefValue = 42;

            public ByRefPropertyException(string message) : base(message)
            {
            }

            public ref readonly int ByRefValue => ref _byRefValue;

            public int ByValue => _byRefValue;
        }

        private sealed class IndexerPropertyException : Exception
        {
            public IndexerPropertyException(string message) : base(message)
            {
            }

            public string this[string key] => key;
        }

        private sealed class ByRefLikePropertyException : Exception
        {
            public ByRefLikePropertyException(string message) : base(message)
            {
            }

            public ReadOnlySpan<char> SpanValue => "span-value".AsSpan();
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
