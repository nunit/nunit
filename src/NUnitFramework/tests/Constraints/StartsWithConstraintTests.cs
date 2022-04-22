// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class StartsWithConstraintTests : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new StartsWithConstraint("hello");
            ExpectedDescription = "String starting with \"hello\"";
            StringRepresentation = "<startswith \"hello\">";
        }

        static object[] SuccessData = new object[] { "hello", "hello there" };

        static object[] FailureData = new object[] {
            new TestCaseData( "goodbye", "\"goodbye\"" ), 
            new TestCaseData( "HELLO THERE", "\"HELLO THERE\"" ),
            new TestCaseData( "I said hello", "\"I said hello\"" ),
            new TestCaseData( "say hello to Fred", "\"say hello to Fred\"" ),
            new TestCaseData( string.Empty, "<string.Empty>" ),
            new TestCaseData( null , "null" ) };

        [Test]
        public void RespectsCulture()
        {
            var constraint = new StartsWithConstraint("r\u00E9sum\u00E9");

            var result = constraint.ApplyTo("re\u0301sume\u0301");
            Assert.That(result.IsSuccess, Is.True);
        }
    }

    [TestFixture]
    public class StartsWithConstraintTestsIgnoringCase : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new StartsWithConstraint("hello").IgnoreCase;
            ExpectedDescription = "String starting with \"hello\", ignoring case";
            StringRepresentation = "<startswith \"hello\">";
        }

        static object[] SuccessData = new object[] { "Hello", "HELLO there" };

        static object[] FailureData = new object[] {
            new TestCaseData( "goodbye", "\"goodbye\"" ), 
            new TestCaseData( "What the hell?", "\"What the hell?\"" ),
            new TestCaseData( "I said hello", "\"I said hello\"" ),
            new TestCaseData( "say hello to Fred", "\"say hello to Fred\"" ),
            new TestCaseData( string.Empty, "<string.Empty>" ),
            new TestCaseData( null , "null" ) };

        [Test]
        public void RespectsCulture()
        {
            var constraint = new EndsWithConstraint("r\u00E9sum\u00E9").IgnoreCase;

            var result = constraint.ApplyTo("re\u0301sume\u0301");
            Assert.That(result.IsSuccess, Is.True);
        }
    }
}
