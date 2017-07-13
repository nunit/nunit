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

        [TestCase(" ss ", "ß", StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "s", StringComparison.CurrentCulture, true)]
        [TestCase(" SS ", "s", StringComparison.CurrentCulture, false)]
        [TestCase(" ss ", "ß", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.CurrentCultureIgnoreCase, true)]
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)
        [TestCase(" ss ", "ß", StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "s", StringComparison.InvariantCulture, true)]
        [TestCase(" SS ", "s", StringComparison.InvariantCulture, false)]
        [TestCase(" ss ", "ß", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" ss ", "s", StringComparison.InvariantCultureIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.InvariantCultureIgnoreCase, true)]
#endif
        [TestCase(" ss ", "ß", StringComparison.Ordinal, false)]
        [TestCase(" SS ", "ß", StringComparison.Ordinal, false)]
        [TestCase(" ss ", "s", StringComparison.Ordinal, true)]
        [TestCase(" SS ", "s", StringComparison.Ordinal, false)]
        [TestCase(" ss ", "ß", StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" SS ", "ß", StringComparison.OrdinalIgnoreCase, false)]
        [TestCase(" ss ", "s", StringComparison.OrdinalIgnoreCase, true)]
        [TestCase(" SS ", "s", StringComparison.OrdinalIgnoreCase, true)]
        public void SpecifyComparisonType(string actual, string expected, StringComparison comparison, bool succeeds)
        {
            Constraint constraint = Contains.Substring(expected).Using(comparison);
            if (!succeeds)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [Test]
        public void UseDifferentComparisonTypes_ThrowsException()
        {
            var subStringConstriant = theConstraint as SubstringConstraint;
            // Invoke Using method before IgnoreCase
            Assert.That(() => subStringConstriant.Using(StringComparison.CurrentCulture).IgnoreCase,
    Throws.TypeOf<InvalidOperationException>());
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)
            Assert.That(() => subStringConstriant.Using(StringComparison.InvariantCulture).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => subStringConstriant.Using(StringComparison.InvariantCultureIgnoreCase).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());
#endif
            Assert.That(() => subStringConstriant.Using(StringComparison.Ordinal).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => subStringConstriant.Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());

            // Invoke IgnoreCase before Using method
            Assert.That(() => (subStringConstriant.IgnoreCase as SubstringConstraint).Using(StringComparison.CurrentCulture),
                Throws.TypeOf<InvalidOperationException>());
#if !(NETSTANDARD1_3 || NETSTANDARD1_6)
            Assert.That(() => (subStringConstriant.IgnoreCase as SubstringConstraint).Using(StringComparison.InvariantCulture),
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => (subStringConstriant.IgnoreCase as SubstringConstraint).Using(StringComparison.InvariantCultureIgnoreCase),
                Throws.TypeOf<InvalidOperationException>());
#endif
            Assert.That(() => (subStringConstriant.IgnoreCase as SubstringConstraint).Using(StringComparison.Ordinal).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => (subStringConstriant.IgnoreCase as SubstringConstraint).Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void UseSameComparisonTypes_DoesNotThrowException()
        {
            var subStringConstriant = theConstraint as SubstringConstraint;
            Assert.DoesNotThrow(() =>
            {
                var newConstriant = subStringConstriant.Using(StringComparison.CurrentCultureIgnoreCase).IgnoreCase;
            });

            var stringConstriant = theConstraint as StringConstraint;
            Assert.DoesNotThrow(() =>
            {
                var newConstriant = stringConstriant.IgnoreCase as SubstringConstraint;
                newConstriant = newConstriant.Using(StringComparison.CurrentCultureIgnoreCase);
            });
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
