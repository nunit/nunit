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
#if NETCOREAPP1_1
            return new AutoRun(Assembly.GetEntryAssembly()).Execute(args, new ColorConsoleWriter(), Console.In);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
