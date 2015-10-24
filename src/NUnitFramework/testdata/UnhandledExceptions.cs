// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

#if !PORTABLE
using System;
using System.Collections;
using System.Text;
using NUnit.Framework;

namespace NUnit.TestData.UnhandledExceptionData
{
    [TestFixture]
    public class UnhandledExceptions
    {
        #region Normal
        [NUnit.Framework.Test]
        public void Normal()
        {
            throw new Exception("Test exception");
        }
        #endregion Normal

        #region Threaded
        [NUnit.Framework.Test]
        public void Threaded()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Normal));
            thread.Start();
            System.Threading.Thread.Sleep(100);
        }
        #endregion Threaded

        #region ThreadedAndForget
        [NUnit.Framework.Test]
        public void ThreadedAndForget()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Normal));
            thread.Start();
        }
        #endregion ThreadedAndForget

        #region ThreadedAndWait
        [NUnit.Framework.Test]
        public void ThreadedAndWait()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Normal));
            thread.Start();
            thread.Join();
        }
        #endregion ThreadedAndWait

        #region ThreadedAssert
        [Test]
        public void ThreadedAssert()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadedAssertProc));
            thread.Start();
            thread.Join();
        }

        private void ThreadedAssertProc()
        {
            Assert.AreEqual(5, 2 + 2);
        }
        #endregion
    }
}
#endif
