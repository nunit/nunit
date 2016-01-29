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
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	/// <summary>
	/// GlobalSettings is a place for setting default values used
	/// by the framework in performing asserts. Anything set through
    /// this class applies to the entire test run. It should not normally
    /// be used from within a test, since it is not thread-safe.
	/// </summary>
	public static class GlobalSettings
	{
		/// <summary>
		/// Default tolerance for floating point equality
		/// </summary>
		public static double DefaultFloatingPointTolerance = 0.0d;

        /// <summary>
        /// This method adds the a new ValueFormatterFactory to the
        /// global chain of responsibility maintained by MsgUtils.
        /// </summary>
        /// <param name="formatterFactory">The factory delegate</param>
        public static void AddFormatter(ValueFormatterFactory formatterFactory)
        {
            MsgUtils.AddFormatter(formatterFactory);
        }

        /// <summary>
        /// This method provides a simplified way to add a ValueFormatter
        /// delegate to the chain of responsibility, creating the factory
        /// delegate internally. It is useful when the Type of the object
        /// is the only criterion for selection of the formatter, since
        /// it can be used without getting involved with a compould function.
        /// </summary>
        /// <typeparam name="TSUPPORTED">The type supported by this formatter</typeparam>
        /// <param name="formatter">The ValueFormatter delegate</param>
        public static void AddFormatter<TSUPPORTED>(ValueFormatter formatter)
        {
            AddFormatter(next => val => (val is TSUPPORTED) ? formatter(val) : next(val));
        }
    }
}
