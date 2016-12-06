// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Framework;
using NUnit.Framework.Constraints;

#if ASYNC
using System.Threading.Tasks;
#endif

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.TestData
{
    [TestFixture]
    public class WarningFixture
    {
        [Test]
        public void WarningPasses_Boolean()
        {
            Warn.Unless(2 + 2 == 4);
        }

        [Test]
        public void WarningPasses_BooleanWithMessage()
        {
            Warn.Unless(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void WarningPasses_BooleanWithMessageAndArgs()
        {
            Warn.Unless(2 + 2 == 4, "Not Equal to {0}", 4);
        }

#if !NET_2_0
        [Test]
        public void WarningPasses_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(2 + 2 == 4, getExceptionMessage);
        }

        [Test]
        public void WarningPasses_BooleanLambda()
        {
            Warn.Unless(() => 2 + 2 == 4);
        }

        [Test]
        public void WarningPasses_BooleanLambdaWithMessage()
        {
            Warn.Unless(() => 2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void WarningPasses_BooleanLambdaWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void WarningPasses_BooleanLambdaWithWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(() => 2 + 2 == 4, getExceptionMessage);
        }
#endif

        [Test]
        public void WarningPasses_ActualAndConstraint()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void WarningPasses_ActualAndConstraintWithMessage()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarningPasses_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

#if !NET_2_0
        [Test]
        public void WarningPasses_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(2 + 2, Is.EqualTo(4), getExceptionMessage);
        }

        [Test]
        public void WarningPasses_ActualLambdaAndConstraint()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void WarningPasses_ActualLambdaAndConstraintWithMessage()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void WarningPasses_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void WarningPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(() => 2 + 2, Is.EqualTo(4), getExceptionMessage);
        }
#endif

        [Test]
        public void WarningPasses_DelegateAndConstraint()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4));
        }

        [Test]
        public void WarningPasses_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Message");
        }

        [Test]
        public void WarningPasses_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), "Should be {0}", 4);
        }

#if !NET_2_0
        [Test]
        public void WarningPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => string.Format("Not Equal to {0}", 4);
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFour), Is.EqualTo(4), getExceptionMessage);
        }
#endif

        private int ReturnsFour()
        {
            return 4;
        }

        [Test]
        public void CallAssertWarnWithMessage()
        {
            Assert.Warn("MESSAGE");
        }

        [Test]
        public void CallAssertWarnWithMessageAndArgs()
        {
            Assert.Warn("MESSAGE: {0}+{1}={2}", 2, 2, 4);
        }

        [Test]
        public void WarnUnless_Boolean()
        {
            Warn.Unless(2 + 2 == 5);
        }

        [Test]
        public void WarnUnless_BooleanWithMessage()
        {
            Warn.Unless(2 + 2 == 5, "message");
        }

        [Test]
        public void WarnUnless_BooleanWithMessageAndArgs()
        {
            Warn.Unless(2 + 2 == 5, "got {0}", 5);
        }

#if !NET_2_0
        [Test]
        public void WarnUnless_BooleanWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.Unless(2 + 2 == 5, getExceptionMessage);
        }

        [Test]
        public void WarnUnless_BooleanLambda()
        {
            Warn.Unless(() => 2 + 2 == 5);
        }

        [Test]
        public void WarnUnless_BooleanLambdaWithMessage()
        {
            Warn.Unless(() => 2 + 2 == 5, "message");
        }

        [Test]
        public void WarnUnless_BooleanLambdaWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2 == 5, "got {0}", 5);
        }

        [Test]
        public void WarnUnless_BooleanLambdaWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "got 5";
            Warn.Unless(() => 2 + 2 == 5, getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_ActualAndConstraint()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));
        }

        [Test]
        public void WarnUnless_ActualAndConstraintWithMessage()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5), "Error");
        }

        [Test]
        public void WarnUnless_ActualAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5), "Should be {0}", 5);
        }

#if !NET_2_0
        [Test]
        public void WarnUnless_ActualAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.Unless(2 + 2, Is.EqualTo(5), getExceptionMessage);
        }

        [Test]
        public void WarnUnless_ActualLambdaAndConstraint()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5));
        }

        [Test]
        public void WarnUnless_ActualLambdaAndConstraintWithMessage()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), "Error");
        }

        [Test]
        public void WarnUnless_ActualLambdaAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), "Should be {0}", 5);
        }

        [Test]
        public void WarnUnless_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 5";
            Warn.Unless(() => 2 + 2, Is.EqualTo(5), getExceptionMessage);
        }
#endif

        [Test]
        public void WarnUnless_DelegateAndConstraint()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4));
        }

        [Test]
        public void WarnUnless_DelegateAndConstraintWithMessage()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Error");
        }

        [Test]
        public void WarnUnless_DelegateAndConstraintWithMessageAndArgs()
        {
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), "Should be {0}", 4);
        }

#if !NET_2_0
        [Test]
        public void WarnUnless_DelegateAndConstraintWithMessageStringFunc()
        {
            Func<string> getExceptionMessage = () => "Should be 4";
            Warn.Unless(new ActualValueDelegate<int>(ReturnsFive), Is.EqualTo(4), getExceptionMessage);
        }
#endif

        private int ReturnsFive()
        {
            return 5;
        }

#if ASYNC
        [Test]
        public void WarningFails_Async()
        {
            Warn.Unless(async () => await One(), Is.EqualTo(2));
        }

        private static Task<int> One()
        {
            return Task.Run(() => 1);
        }
#endif
    }
}
