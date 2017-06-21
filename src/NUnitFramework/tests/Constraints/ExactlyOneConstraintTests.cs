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
        private static IEnumerable<string> testCollection = new List<string> { "item" };

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(testCollection, new ExactlyOneConstraint(Is.EqualTo("item")));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatch()
        {
            Assert.That(testCollection, new ExactlyOneConstraint(Is.Not.EqualTo("notItem")));
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
