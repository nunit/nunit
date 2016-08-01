﻿// ***********************************************************************
// Copyright (c) 2010-2014 Charlie Poole
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
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace NUnit.Engine
{
    public class RunTestsCallbackHandler : CallbackHandler
    {
        private ITestEventListener listener;

        public RunTestsCallbackHandler(ITestEventListener listener)
        {
            // TODO: Move this substitution into the framework?
            this.listener = listener ?? new NullListener();
        }

        public override void ReportProgress(string state)
        {
            listener.OnTestEvent(state);
        }

        #region Nested NullListener class
        class NullListener : ITestEventListener
        {
            public void OnTestEvent(string report)
            {
            }
        }
        #endregion
    }
}
