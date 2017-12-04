using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NUnit.Compatibility
{
#if NET20
    /// <summary>
    /// 
    /// </summary>
    public static class ReaderWriterLockExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        public static void EnterReadLock(this ReaderWriterLock rwLock)
        {
            rwLock.AcquireReaderLock(Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        public static void EnterWriteLock(this ReaderWriterLock rwLock)
        {
            rwLock.AcquireWriterLock(Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        public static void ExitReadLock(this ReaderWriterLock rwLock)
        {
            rwLock.ReleaseReaderLock();
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        public static void ExitWriteLock(this ReaderWriterLock rwLock)
        {
            rwLock.ReleaseWriterLock();
        }
   }
#endif
}
