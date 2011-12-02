// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
	[TestFixture]
	public class AssertThrowsTests : MessageChecker
	{
		[Test]
		public void CorrectExceptionThrown()
		{
            Assert.Throws(typeof(ArgumentException), TestDelegates.ThrowsArgumentException);
            Assert.Throws(typeof(ArgumentException),
                delegate { throw new ArgumentException(); });

            Assert.Throws<ArgumentException>(
                delegate { throw new ArgumentException(); });
            Assert.Throws<ArgumentException>(TestDelegates.ThrowsArgumentException);

            // Without cast, delegate is ambiguous before C# 3.0.
            Assert.That((TestDelegate)delegate { throw new ArgumentException(); },
                    Throws.Exception.TypeOf<ArgumentException>() );
            //Assert.Throws( Is.TypeOf(typeof(ArgumentException)),
            //        delegate { throw new ArgumentException(); } );
        }

		[Test]
		public void CorrectExceptionIsReturnedToMethod()
		{
			ArgumentException ex = Assert.Throws(typeof(ArgumentException),
                new TestDelegate(TestDelegates.ThrowsArgumentException)) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Is.StringStarting("myMessage"));
#if !NETCF
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.Throws<ArgumentException>(
                delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Is.StringStarting("myMessage"));
#if !NETCF
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

			ex = Assert.Throws(typeof(ArgumentException), 
                delegate { throw new ArgumentException("myMessage", "myParam"); } ) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Is.StringStarting("myMessage"));
#if !NETCF
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.Throws<ArgumentException>(TestDelegates.ThrowsArgumentException) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Is.StringStarting("myMessage"));
#if !NETCF
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif
		}

		[Test, ExpectedException(typeof(AssertionException))]
		public void NoExceptionThrown()
		{
			expectedMessage =
				"  Expected: <System.ArgumentException>" + Env.NewLine +
				"  But was:  null" + Env.NewLine;

            Assert.Throws<ArgumentException>(TestDelegates.ThrowsNothing);
		}

        [Test, ExpectedException(typeof(AssertionException))]
        public void UnrelatedExceptionThrown()
        {
            expectedMessage =
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.ApplicationException>" + Env.NewLine;

            Assert.Throws<ArgumentException>(TestDelegates.ThrowsApplicationException);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void BaseExceptionThrown()
        {
            expectedMessage =
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.Exception>" + Env.NewLine;

            Assert.Throws<ArgumentException>(TestDelegates.ThrowsSystemException);
        }

        [Test,ExpectedException(typeof(AssertionException))]
        public void DerivedExceptionThrown()
        {
            expectedMessage =
                "  Expected: <System.Exception>" + Env.NewLine +
                "  But was:  <System.ArgumentException>" + Env.NewLine;

            Assert.Throws<Exception>(TestDelegates.ThrowsArgumentException);
        }

        [Test]
        public void DoesNotThrowSuceeds()
        {
            Assert.DoesNotThrow(TestDelegates.ThrowsNothing);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void DoesNotThrowFails()
        {
            Assert.DoesNotThrow(TestDelegates.ThrowsArgumentException);
        }
    }
}
