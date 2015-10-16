using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NUnitLite.Runner
{
    public class Program
    {
        public int Main(string[] args)
        {   
            return new AutoRun().Execute(typeof(NUnit.Framework.Tests.ThrowsTests).GetTypeInfo().Assembly, Console.Out, Console.In, args);
        }
    }
}
