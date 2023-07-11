// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
            Framework.Classic.Assert.AreEqual(5, 2 + 2);
        }
        #endregion
    }
}
