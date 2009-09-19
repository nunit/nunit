// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Threading;

namespace NUnit.Framework
{
    /// <summary>
    /// WUsed on a method, marks the test with a timeout value in milliseconds. 
    /// The test will be run in a separate thread and is cancelled if the timeout 
    /// is exceeded. Used on a method or assembly, sets the default timeout 
    /// for all contained test methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class TimeoutAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a TimeoutAttribute given a time in milliseconds
        /// </summary>
        /// <param name="timeout">The timeout value in milliseconds</param>
        public TimeoutAttribute(int timeout)
            : base(timeout) { }
    }

    /// <summary>
    /// Marks a test that must run in the STA, causing it
    /// to run in a separate thread if necessary.
    /// 
    /// On methods, you may also use STAThreadAttribute
    /// to serve the same purpose.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class RequiresSTAAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a RequiresSTAAttribute
        /// </summary>
        public RequiresSTAAttribute()
        {
            this.Properties.Add("APARTMENT_STATE", ApartmentState.STA);
        }
    }

    /// <summary>
    /// Marks a test that must run in the MTA, causing it
    /// to run in a separate thread if necessary.
    /// 
    /// On methods, you may also use MTAThreadAttribute
    /// to serve the same purpose.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class RequiresMTAAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a RequiresMTAAttribute
        /// </summary>
        public RequiresMTAAttribute()
        {
            this.Properties.Add("APARTMENT_STATE", ApartmentState.MTA);
        }
    }

    /// <summary>
    /// Marks a test that must run on a separate thread.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class RequiresThreadAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a RequiresThreadAttribute
        /// </summary>
        public RequiresThreadAttribute()
            : base(true) { }

        /// <summary>
        /// Construct a RequiresThreadAttribute, specifying the apartment
        /// </summary>
        public RequiresThreadAttribute(ApartmentState apartment)
            : base(true)
        {
            this.Properties.Add("APARTMENT_STATE", apartment);
        }
    }
}
