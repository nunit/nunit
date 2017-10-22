// *****************************************************************************
//Copyright(c) 2017 Charlie Poole, Rob Prouse

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
// ****************************************************************************

using System;
using System.Threading;
#if ASYNC
using System.Threading.Tasks;
#endif
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class MaxTimeConstraintTests : ConstraintTestBaseNoData
    {
        private const int TimeOut = 500;

        static object[] SuccessData = new object[]
        {
            new TestDelegate(() => Thread.Sleep(100)),
            new TestDelegate(() => Thread.Sleep(200)),
#if ASYNC
#if NET_4_0
            new AsyncTestDelegate(() => Task.Factory.StartNew(() => Thread.Sleep(100))),
            new AsyncTestDelegate(() => Task.Factory.StartNew(() => Thread.Sleep(200))),
#else
            new AsyncTestDelegate(() => Task.Delay(100)),
            new AsyncTestDelegate(() => Task.Delay(200)),
#endif
#endif
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData(new TestDelegate(() => Thread.Sleep(800))),
            new TestCaseData(new TestDelegate(() => Thread.Sleep(1000))),
#if ASYNC
#if NET_4_0
            new TestCaseData(new AsyncTestDelegate(() => Task.Factory.StartNew(() => Thread.Sleep(800)))),
            new TestCaseData(new AsyncTestDelegate(() => Task.Factory.StartNew(() => Thread.Sleep(1000)))),
#else
            new TestCaseData(new AsyncTestDelegate(() => Task.Delay(1500))),
            new TestCaseData(new AsyncTestDelegate(() => Task.Delay(2000))),
#endif
#endif
        };

        [SetUp]
        public void SetUp()
        {
            TimeSpan timeOut = TimeSpan.FromMilliseconds(TimeOut);
            theConstraint = new MaxTimeConstraint(timeOut);
            expectedDescription = $"Maximum time {timeOut}";
            stringRepresentation = $"<maxtime {timeOut}>";
        }

        [Test, TestCaseSource("SuccessData")]
        public void SucceedsWithGoodValues(object value)
        {
            var constraintResult = theConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [Test, TestCaseSource("FailureData")]
        public void FailsWithBadValues(object badValue)
        {
            string NL = Environment.NewLine;

            var constraintResult = theConstraint.ApplyTo(badValue);
            Assert.IsFalse(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.StartWith(
                TextMessageWriter.Pfx_Expected + expectedDescription + NL +
                TextMessageWriter.Pfx_Actual + constraintResult.ActualValue.ToString()));
        }
    }
}
