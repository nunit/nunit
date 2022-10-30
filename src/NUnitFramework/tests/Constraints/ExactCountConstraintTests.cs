// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    public class ExactCountConstraintTests
    {
        private static readonly string[] names = new[] { "Charlie", "Fred", "Joe", "Charlie" };

        [Test]
        public void ZeroItemsMatch()
        {
            Assert.That(names, new ExactCountConstraint(0, Is.EqualTo("Sam")));
            Assert.That(names, Has.Exactly(0).EqualTo("Sam"));
        }

        [Test]
        public void ZeroItemsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "no item equal to \"Charlie\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "2 items < \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(0, Is.EqualTo("Charlie"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(names, new ExactCountConstraint(1, Is.EqualTo("Fred")));
            Assert.That(names, Has.Exactly(1).EqualTo("Fred"));
        }

        [Test]
        public void ExactlyOneItemMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly one item equal to \"Charlie\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "2 items < \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(1, Is.EqualTo("Charlie"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyTwoItemsMatch()
        {
            Assert.That(names, new ExactCountConstraint(2, Is.EqualTo("Charlie")));
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyAndExactly()
        {
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie").And.Exactly(1).EqualTo("Fred"));
            Assert.That(names, Has.Exactly(4).Items.And.Exactly(2).EqualTo("Charlie"));
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie").And.Exactly(4).Items);
        }

        [Test]
        public void ExactlyOrExactly()
        {
            Assert.That(names, Has.Exactly(3).EqualTo("Fred").Or.Exactly(2).EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyFollowedByOr()
        {
            Assert.That(names, Has.Exactly(3).EqualTo("Fred").Or.EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyTwoItemsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly 2 items equal to \"Fred\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "1 item < \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(2, Is.EqualTo("Fred"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyFourItemsNoPredicate()
        {
            Assert.That(names, new ExactCountConstraint(4));
        }

        [Test]
        public void ExactlyFourItemsNoopNoPredicate()
        {
            Assert.That(names, Has.Exactly(4).Items);
        }

        [Test]
        public void ExactlyOneItemNoPredicateFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly one item" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "4 items < \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(1)));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyTwoItemsNoopMatch()
        {
            Assert.That(names, Has.Exactly(2).Items.EqualTo("Charlie"));
        }
        
        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new ExactCountConstraint(1));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }

        [Test]
        [Description("Test that the " + nameof(ExactCountConstraint) + " returns all ten elements of the given list without a triple dot")]
        public void OutputStringTestWithTenItems()
        {
            var longElementList = new[]
            {
                "Alfa", "Bravo", "Charlie", "Delta", "Echo",
                "Foxtrot", "Golf", "Hotel", "India", "Juliett",
            };

            var expectedMessage =
                TextMessageWriter.Pfx_Expected
                + "exactly 5 items"
                + Environment.NewLine
                + TextMessageWriter.Pfx_Actual
                + "10 items < \"Alfa\", \"Bravo\", \"Charlie\", \"Delta\", \"Echo\", \"Foxtrot\", \"Golf\", \"Hotel\", \"India\", \"Juliett\" >"
                + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(longElementList, Has.Exactly(5).Items));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        [Description("Test that the " + nameof(ExactCountConstraint) + " returns only ten elements of the given list with a triple dot")]
        public void OutputStringTestWithMoreAsTenItems()
        {
            var longElementList = new[]
            {
                "Alfa", "Bravo", "Charlie", "Delta", "Echo",
                "Foxtrot", "Golf", "Hotel", "India", "Juliett",
                "Kilo", "Lima", "Mike", "November", "Oscar"
            };

            var expectedMessage =
                TextMessageWriter.Pfx_Expected
                + "exactly 10 items"
                + Environment.NewLine
                + TextMessageWriter.Pfx_Actual
                + "15 items < \"Alfa\", \"Bravo\", \"Charlie\", \"Delta\", \"Echo\", \"Foxtrot\", \"Golf\", \"Hotel\", \"India\", \"Juliett\"... >"
                + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(longElementList, Has.Exactly(10).Items));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}
