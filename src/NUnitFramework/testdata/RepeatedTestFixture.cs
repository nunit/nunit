// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
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

// TODO: Rework this
// RepeatAttribute should either
//  1) Apply at load time to create the exact number of tests, or
//  2) Apply at run time, generating tests or results dynamically
//
// #1 is feasible but doesn't provide much benefit
// #2 requires infrastructure for dynamic test cases first
using System;
using NUnit.Framework;

namespace NUnit.TestData.RepeatingTests
{
    public class RepeatSuccessFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatSuccess()
        {
            count++;
            Assert.IsTrue (true);
        }
    }

    public class RepeatFailOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnFirst()
        {
            count++;
            Assert.IsFalse (true);
        }
    }

    public class RepeatFailOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            count++;

            if (count == 2)
                Assert.IsTrue(false);
        }
    }

    public class RepeatFailOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            count++;

            if (count == 3)
                Assert.IsTrue(false);
        }
    }

    public class RepeatedTestWithIgnoreAttribute : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3), Ignore("Ignore this test")]
        public void RepeatShouldIgnore()
        {
            Assert.Fail("Ignored test executed");
        }
    }

    public class RepeatIgnoredOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;
            Assert.Ignore("Ignoring");
        }
    }

    public class RepeatIgnoredOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;

            if (count == 2)
                Assert.Ignore("Ignoring");
        }
    }

    public class RepeatIgnoredOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;

            if (count == 3)
                Assert.Ignore("Ignoring");
        }
    }

    public class RepeatErrorOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;
            throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatErrorOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;

            if (count == 2)
                throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatErrorOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            count++;

            if (count == 3)
                throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatedTestWithCategory : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3), Category("SAMPLE")]
        public void TestWithCategory()
        {
            count++;
            Assert.IsTrue(true);
        }
    }
}
