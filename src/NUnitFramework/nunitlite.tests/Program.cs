using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Common;

namespace NUnitLite.Tests
{
    class Program
    {
        public static int Main(string[] args)
        {
#if PORTABLE
            return new AutoRun().Execute(typeof(Program).Assembly, args, new ColorConsoleWriter(), Console.In);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
