using System.Threading;
using NUnit.Framework;

namespace NUnit.Tests
{
    public class SlowTests
    {
        const int DELAY = 100;
        
        public class AAA
        {
            [Test]
            public void Test1() { Thread.Sleep(DELAY); }
            [Test]
            public void Test2() { Thread.Sleep(DELAY); }
            [Test]
            public void Test3() { Thread.Sleep(DELAY); }
        }

        public class BBB
        {
            [Test]
            public void Test1() { Thread.Sleep(DELAY); }
            [Test]
            public void Test2() { Thread.Sleep(DELAY); }
            [Test]
            public void Test3() { Thread.Sleep(DELAY); }
        }

        public class CCC
        {
            [Test]
            public void Test1() { Thread.Sleep(DELAY); }
            [Test]
            public void Test2() { Thread.Sleep(DELAY); }
            [Test]
            public void Test3() { Thread.Sleep(DELAY); }
        }
    }
}
