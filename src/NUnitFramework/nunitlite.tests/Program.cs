using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitLite.Tests
{
    class Program
    {
        public static int Main(string[] args)
        {
#if PORTABLE
            return new AutoRun().Execute(typeof(Program).Assembly, Console.Out, Console.In, args);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
