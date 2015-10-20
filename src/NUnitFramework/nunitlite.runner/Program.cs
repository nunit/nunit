using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Common;

namespace NUnitLite
{
    static class Program
    {
        [STAThread]
        public static int Main(string[] args) 
        {
            return new AutoRun().Execute(args);
        }
    }
}
