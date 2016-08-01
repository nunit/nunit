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

#if NET_4_0 || NET_4_5 || PORTABLE
using System.Threading.Tasks;
using NUnit.Framework;
using System;

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.TestData
{
    public class AsyncDummyFixture
    {
        [Test]
        public async void AsyncVoid()
        {
            await Task.Delay(1); // To avoid warning message
        }

        [Test]
        public async System.Threading.Tasks.Task AsyncTask()
        {
            await Task.Delay(1);
        }

        [Test]
        public async Task<int> AsyncGenericTask()
        {
            return await Task.FromResult(1);
        }

        [Test]
        public System.Threading.Tasks.Task NonAsyncTask()
        {
            return Task.Delay(0);
        }

        [Test]
        public Task<int> NonAsyncGenericTask()
        {
            return Task.FromResult(1);
        }

        [TestCase(4)]
        public async void AsyncVoidTestCase(int x)
        {
            await Task.Delay(0);
        }
        
        [TestCase(ExpectedResult = 1)]
        public async void AsyncVoidTestCaseWithExpectedResult()
        {
            await Task.Run(() => 1);
        }

        [TestCase(4)]
        public async System.Threading.Tasks.Task AsyncTaskTestCase(int x)
        {
            await Task.Delay(0);
        }

        [TestCase(ExpectedResult = 1)]
        public async System.Threading.Tasks.Task AsyncTaskTestCaseWithExpectedResult()
        {
            await Task.Run(() => 1);
        }

        [TestCase(4)]
        public async Task<int> AsyncGenericTaskTestCase()
        {
            return await Task.Run(() => 1);
        }

        [TestCase(ExpectedResult = 1)]
        public async Task<int> AsyncGenericTaskTestCaseWithExpectedResult()
        {
            return await Task.Run(() => 1);
        }

        [TestCase(4)]
        public System.Threading.Tasks.Task TaskTestCase(int x)
        {
            return Task.Delay(0);
        }

        [TestCase(ExpectedResult = 1)]
        public System.Threading.Tasks.Task TaskTestCaseWithExpectedResult()
        {
            return Task.Run(() => 1);
        }

        [TestCase(4)]
        public Task<int> GenericTaskTestCase()
        {
            return Task.Run(() => 1);
        }

        [TestCase(ExpectedResult = 1)]
        public Task<int> GenericTaskTestCaseWithExpectedResult()
        {
            return Task.Run(() => 1);
        }

        private async Task<int> Throw()
        {
            Func<int> thrower = () => { throw new InvalidOperationException(); };
            return await Task.Run( thrower );
        }
    }
}
#endif