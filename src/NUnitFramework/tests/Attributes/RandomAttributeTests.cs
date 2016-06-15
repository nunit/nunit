// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using System.Collections;
using System.Reflection;
using System.Text;
using System.Linq;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.TestData.RandomAttributeTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    public class RandomAttributeTests
    {

        [TestCaseSource(typeof(MethodNames))]
        public void CheckRandomResult(string methodName)
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(RandomAttributeFixture), methodName);
            Assert.That(result.Children.Count(), Is.EqualTo(RandomAttributeFixture.COUNT));

            if (result.ResultState != ResultState.Success)
            {
                var msg = new StringBuilder();
                msg.AppendFormat("Unexpected Result: {0}\n", result.ResultState);

                if (result.ResultState.Site == FailureSite.Child)
                    foreach (var child in result.Children)
                    {
                        msg.AppendFormat(" {0}: {1}\n", child.Name, child.ResultState);
                        msg.AppendFormat("{0}\n", child.Message);
                    }

                Assert.Fail(msg.ToString());
            }
        }

        class MethodNames : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                foreach (var method in typeof(RandomAttributeFixture).GetMethods())
                    if (method.IsDefined(typeof(TestAttribute), false))
                        yield return new TestCaseData(method.Name).SetName(method.Name);
            }
        }
    }
}
