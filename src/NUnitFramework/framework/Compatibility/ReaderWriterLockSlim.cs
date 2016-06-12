using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NUnit.Compatibility
{
#if NETCF
    /// <summary>
    /// 
    /// </summary>
    public class ReaderWriterLockSlim
    {
        private object _lockObject = new object();

        /// <summary>
        /// 
        /// </summary>
        public void EnterReadLock()
        {
            Monitor.Enter (_lockObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnterWriteLock()
        {
            Monitor.Enter(_lockObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExitReadLock()
        {
            Monitor.Exit(_lockObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExitWriteLock()
        {
            Monitor.Exit(_lockObject);
        }
    }
#endif

#if NET_2_0
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
