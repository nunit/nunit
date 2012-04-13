using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Engine
{
    public abstract class TestListener
    {
        public static ITestEventHandler Null
        {
            get { return new NullTestListener(); }
        }

        private class NullTestListener : ITestEventHandler
        {
            #region ITestEventHandler Members

            public void  OnTestEvent(string report)
            {
            }

            #endregion
        }
    }
}
