using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework.Tests.Constraints
{
    public class ExactlyOneConstraintTests
    {
        private static IEnumerable<string> testCollectionLen1 = new List<string> { "item" };
        private static IEnumerable<string> testCollectionLen2 = new List<string> { "item", "otherItem" };

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint(Is.EqualTo("item")));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatch()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint(Is.Not.EqualTo("notItem")));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsOne()
        {
            Assert.That(testCollectionLen1, new ExactlyOneConstraint());
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsTwo()
        {
            Assert.IsFalse(new ExactlyOneConstraint().ApplyTo(testCollectionLen2).IsSuccess);
        }

        [Test]
        public void ExactlyOneConstraintMatchingWhereCollectionIsTwo()
        {
            Assert.IsFalse(new ExactlyOneConstraint(Is.EqualTo("item")).ApplyTo(testCollectionLen2).IsSuccess);
        }

        [Test]
        public void ExactlyOneConstraintNotMatchingWhereCollectionIsTwo()
        {
            Assert.IsFalse(new ExactlyOneConstraint(Is.EqualTo("blah")).ApplyTo(testCollectionLen2).IsSuccess);
        }

        [Test]
        public void ExactlyOneThrowsWhenNotIEnumberable()
        {
            Assert.Throws<ArgumentException>(delegate ()
            {
                Assert.That(new int(), new ExactlyOneConstraint(Is.EqualTo("item")));
            });
        }

        [Test]
        public void ExactlyOneThrowsWhenConstraintIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var blah = new ExactlyOneConstraint(null);
            });
        }
    }
}
