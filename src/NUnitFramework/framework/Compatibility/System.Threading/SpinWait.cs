// SpinWait.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Diagnostics;

namespace System.Threading
{
    /// <summary>
    /// 
    /// </summary>
	internal struct SpinWait
	{
		// The number of step until SpinOnce yield on multicore machine
		const           int  step = 10;
		const           int  maxTime = 200;
		static readonly bool isSingleCpu = (Environment.ProcessorCount == 1);
		int ntime;

        /// <summary>
        /// 
        /// </summary>
		public void SpinOnce ()
		{
			ntime += 1;

			if (isSingleCpu) {
                // On a single-CPU system, spinning does no good
#if NET_2_0 || NET_3_5
                Thread.Sleep(ntime % step == 0 ? 1 : 0);
#else
                Thread.Yield ();
#endif
			} 
			else {
				if (ntime % step == 0)
#if NET_2_0 || NET_3_5
                    Thread.Sleep(1);
#else
                    Thread.Yield ();
#endif
                else
                    // Multi-CPU system might be hyper-threaded, let other thread run
                    Thread.SpinWait (Math.Min (ntime, maxTime) << 1);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
		public static void SpinUntil (Func<bool> condition)
		{
			SpinWait sw = new SpinWait ();
			while (!condition ())
				sw.SpinOnce ();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
		public static bool SpinUntil (Func<bool> condition, TimeSpan timeout)
		{
			return SpinUntil (condition, (int)timeout.TotalMilliseconds);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
		public static bool SpinUntil (Func<bool> condition, int millisecondsTimeout)
		{
			SpinWait sw = new SpinWait ();
			Stopwatch watch = Stopwatch.StartNew ();

			while (!condition ()) {
				if (watch.ElapsedMilliseconds > millisecondsTimeout)
					return false;
				sw.SpinOnce ();
			}

			return true;
		}

        /// <summary>
        /// 
        /// </summary>
		public void Reset ()
		{
			ntime = 0;
		}

        /// <summary>
        /// 
        /// </summary>
		public bool NextSpinWillYield {
			get {
				return isSingleCpu ? true : ntime % step == 0;
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public int Count {
			get {
				return ntime;
			}
		}
	}
}
