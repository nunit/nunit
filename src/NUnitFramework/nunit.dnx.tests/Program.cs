using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NUnitLite.Runner
{
    public class Program
    {
        public void Main(string[] args)
        {            
            return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
        }
    }
}
