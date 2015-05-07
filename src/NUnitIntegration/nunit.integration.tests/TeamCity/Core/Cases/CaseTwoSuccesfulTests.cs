
// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    internal sealed class CaseTwoSuccesfulTests : CaseBase
    {
        public CaseTwoSuccesfulTests()
            : base(CertType.TestFramework, "TwoSuccesfulTests", "2 succesful tests")
        {
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = GetTestMessages(messages).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 4, out result))
            {
                return result;
            }

            var testStarted1 = testMessages[0];
            var testFinished1 = testMessages[1];
            var testStarted2 = testMessages[2];
            var testFinished2 = testMessages[3];

            if (!CheckMessageType(testStarted1, ServiceMessageConstants.TestStartedMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testFinished1, ServiceMessageConstants.TestFinishedMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testStarted2, ServiceMessageConstants.TestStartedMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testFinished2, ServiceMessageConstants.TestFinishedMessageName, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted1, testFinished1, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted2, testFinished2, out result))
            {
                return result;
            }
            
            return new ValidationResult(ValidationState.Valid, new Details());
        }
    }
}