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

namespace NUnit.Framework.Internal
{
    internal interface IInvocationDescriptor
    {
        Delegate Delegate { get; }
        object Invoke();
    }

    internal class InvocationDescriptorExtensions
    {
        public static IInvocationDescriptor GetInvocationDescriptor(object actual)
        {
            var invocationDescriptor = actual as IInvocationDescriptor;

            if (invocationDescriptor == null)
            {
                var testDelegate = actual as TestDelegate;

                if (testDelegate != null)
                {
                    invocationDescriptor = new VoidInvocationDescriptor(testDelegate);
                }

#if ASYNC
                else
                {
                    var asyncTestDelegate = actual as AsyncTestDelegate;
                    if (asyncTestDelegate != null)
                    {
                        invocationDescriptor = new GenericInvocationDescriptor<System.Threading.Tasks.Task>(() => asyncTestDelegate());
                    }
                }
#endif
            }
            if (invocationDescriptor == null)
                throw new ArgumentException(
                    string.Format(
                        "The actual value must be a TestDelegate or AsyncTestDelegate but was {0}",
                        actual.GetType().Name),
                    "actual");

            return invocationDescriptor;
        }
    }
}
