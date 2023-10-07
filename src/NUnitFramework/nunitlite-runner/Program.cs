// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnitLite
{
    /// <summary>
    /// NUnitLite-Runner is a console program that is able to run NUnitLite
    /// tests as an alternative to using an executable NUnitLite test.
    ///
    /// Since it references a particular version of NUnitLite, it may only
    /// be used for test assemblies built against that framework version.
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            return new TextRunner().Execute(args);
        }
    }
}
