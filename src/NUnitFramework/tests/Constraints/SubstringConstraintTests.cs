// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class SubstringConstraintTests : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new SubstringConstraint("hello");
            expectedDescription = "String containing \"hello\"";
            stringRepresentation = "<substring \"hello\">";
        }

        static object[] SuccessData = new object[] { "hello", "hello there", "I said hello", "say hello to fred" };
        
        static object[] FailureData = new object[] { 
            new TestCaseData( "goodbye", "\"goodbye\"" ), 
            new TestCaseData( "HELLO", "\"HELLO\"" ),
            new TestCaseData( "What the hell?", "\"What the hell?\"" ),
            new TestCaseData( string.Empty, "<string.Empty>" ),
            new TestCaseData( null, "null" ) };

        [TestCase(" ss ", "ß", false, StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "ß", false, StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "s", false, StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "s", false, StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "ß", false, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", false, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", false, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", false, StringComparison.CurrentCultureIgnoreCase, true)]
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)
        [TestCase(" ss ", "ß", false, StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "ß", false, StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "s", false, StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "s", false, StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "ß", false, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", false, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", false, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", false, StringComparison.InvariantCultureIgnoreCase, true)]
#endif
        [TestCase(" ss ", "ß", false, StringComparison.Ordinal, false)]
        [TestCase(" SS ", "ß", false, StringComparison.Ordinal, false)]
        [TestCase(" ss ", "s", false, StringComparison.Ordinal, true)]
        [TestCase(" SS ", "s", false, StringComparison.Ordinal, false)]
        [TestCase(" ss ", "ß", false, StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" SS ", "ß", false, StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" ss ", "s", false, StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" SS ", "s", false, StringComparison.OrdinalIgnoreCase, true)]

        [TestCase(" ss ", "ß", true, StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "ß", true, StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "s", true, StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "s", true, StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "ß", true, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", true, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", true, StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", true, StringComparison.CurrentCultureIgnoreCase, true)]
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)           
        [TestCase(" ss ", "ß", true, StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "ß", true, StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "s", true, StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "s", true, StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "ß", true, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", true, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", true, StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", true, StringComparison.InvariantCultureIgnoreCase, true)]
#endif                         
        [TestCase(" ss ", "ß", true, StringComparison.Ordinal, false)]
        [TestCase(" SS ", "ß", true, StringComparison.Ordinal, false)]
        [TestCase(" ss ", "s", true, StringComparison.Ordinal, true)]
        [TestCase(" SS ", "s", true, StringComparison.Ordinal, false)]
        [TestCase(" ss ", "ß", true, StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" SS ", "ß", true, StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" ss ", "s", true, StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" SS ", "s", true, StringComparison.OrdinalIgnoreCase, true)]

        public void UsingAfterIgnoreCase(string actual, string expected, bool ignoreCase, StringComparison comparison, bool succeeds)
        {
            SubstringConstraint substringConstraint = Contains.Substring(expected);
            // In case StringConstraint.IgnoreCase was set to true 
            if (ignoreCase)
                substringConstraint = substringConstraint.IgnoreCase as SubstringConstraint;

            Constraint constraint = substringConstraint.Using(comparison);
            if (!succeeds)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [TestCase(" ss ", "ß", StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCulture, true)]
        [TestCase(" ss ", "s", StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "s", StringComparison.CurrentCulture, true)]
        [TestCase(" ss ", "ß", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.CurrentCultureIgnoreCase, true)]
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)           
        [TestCase(" ss ", "ß", StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCulture, true)]
        [TestCase(" ss ", "s", StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "s", StringComparison.InvariantCulture, true)]
        [TestCase(" ss ", "ß", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.InvariantCultureIgnoreCase, true)]
#endif                         
        [TestCase(" ss ", "ß", StringComparison.Ordinal, true)]
        [TestCase(" SS ", "ß", StringComparison.Ordinal, true)]
        [TestCase(" ss ", "s", StringComparison.Ordinal, true)]
        [TestCase(" SS ", "s", StringComparison.Ordinal, true)]
        [TestCase(" ss ", "ß", StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" SS ", "ß", StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" ss ", "s", StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.OrdinalIgnoreCase, true)]
        public void UsingBeforeIgnoreCase(string actual, string expected, StringComparison comparison, bool succeeds)
        {
            SubstringConstraint substringConstraint = Contains.Substring(expected);
            Constraint constraint = substringConstraint.Using(comparison).IgnoreCase;
            if (!succeeds)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }
    }

    [TestFixture]
    public class SubstringConstraintTestsIgnoringCase : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new SubstringConstraint("hello").IgnoreCase;
            expectedDescription = "String containing \"hello\", ignoring case";
            stringRepresentation = "<substring \"hello\">";
        }

        static object[] SuccessData = new object[] { "Hello", "HellO there", "I said HELLO", "say hello to fred" };
        
        static object[] FailureData = new object[] {
            new TestCaseData( "goodbye", "\"goodbye\"" ), 
            new TestCaseData( "What the hell?", "\"What the hell?\"" ),
            new TestCaseData( string.Empty, "<string.Empty>" ),
            new TestCaseData( null, "null" ) };
    }

    //[TestFixture]
    //public class EqualIgnoringCaseTest : ConstraintTest
    //{
    //    [SetUp]
    //    public void SetUp()
    //    {
    //        Matcher = new EqualConstraint("Hello World!").IgnoreCase;
    //        Description = "\"Hello World!\", ignoring case";
    //    }

    //    static object[] SuccessData = new object[] { "hello world!", "Hello World!", "HELLO world!" };

    //    static object[] FailureData = new object[] { "goodbye", "Hello Friends!", string.Empty, null };


    //    string[] ActualValues = new string[] { "\"goodbye\"", "\"Hello Friends!\"", "<string.Empty>", "null" };
    //}
}
