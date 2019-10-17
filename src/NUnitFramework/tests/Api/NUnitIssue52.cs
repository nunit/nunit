using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Api
{
    [TestFixture]
    public class NUnitIssue52
    {
        class SelfContainer : IEnumerable
        {
            public IEnumerator GetEnumerator() { yield return this; }
        }

        [Test]
        public void SelfContainedItemFoundInArray()
        {
            var item = new SelfContainer();
            var items = new SelfContainer[] { new SelfContainer(), item };



            // work around
            //Assert.True(((ICollection<SelfContainer>)items).Contains(item));

            // causes StackOverflowException
            //Assert.Contains(item, items);

            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            var equal = equalityComparer.AreEqual(item, items, ref tolerance);
            Assert.IsFalse(equal);
            //Console.WriteLine("test completed");
        }
    }
}
