// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
