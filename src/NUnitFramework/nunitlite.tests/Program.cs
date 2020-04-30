using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Common;

namespace NUnitLite.Tests
{
    class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun().Execute(args);
        }
    }
}
